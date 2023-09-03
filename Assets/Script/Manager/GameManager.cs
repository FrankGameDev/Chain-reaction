using DamageNumbersPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogWarning("Game manager is NULL");
            return _instance;
        }
    }

    // Game logic properties
    public static GameState currentState;
    [HideInInspector] public int chainingBlockCount;
    private int maxCurrentChainCount;
    [HideInInspector] public bool isChaining = false;

    [Header("Game Settings")]
    public PlayerContainer playerContainer;
    private List<Tuple<PlayerSettings, bool>> playerInGame;
    public int playerCount
    {
        get { return PlayerPrefs.GetInt("PlayerCount"); }
    }

    public int currentPlayerIndex;
    public PlayerSettings currentPlayerSettings => playerInGame[currentPlayerIndex].Item1;

    [Header("Grid properties")]
    public int width;
    public int height;
    public Block _blockPrefab;
    Dictionary<Vector2, Block> blockInGrid;
    public Transform gridContainer;


    [Header("Events")]
    public GameEvent switchPlayerEvent;
    public GameEvent winEvent;
    public GameEvent chainReactionEvent;

    // Utility
    private bool canWin;
    private float checkStatusMaxTimer = .75f, checkStatusTimer;

    private void Awake()
    {
        // -- Singleton
        if (_instance == null)
        {
            //First run, set the _instance
            _instance = this;
        }
        else if (_instance != this)
        {
            //_instance is not the same as the one we have
            Destroy(this);
        }

        playerInGame = new List<Tuple<PlayerSettings, bool>>();
        for (int i = 0; i < playerCount; i++)
        {
            playerInGame.Add(new Tuple<PlayerSettings, bool>(playerContainer.players[i], true));
        }
    }
    private void Start()
    {
        blockInGrid = new Dictionary<Vector2, Block>();
        GenerateGrid();
    }

    private void Update()
    {
        maxCurrentChainCount = Mathf.Max(chainingBlockCount, maxCurrentChainCount);

        CheckGameStatusOnMoveDone();
    }

    void GenerateGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 pos = new Vector2(i, j);
                var spawnedBlock = Instantiate(_blockPrefab, pos, Quaternion.identity, gridContainer);
                blockInGrid[pos] = spawnedBlock;
            }
        }

        Vector2 center = new Vector2((float)width / 2 - .5f, (float)height / 2 - .5f);
        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

        // Init blocks =================================
        foreach (var position in blockInGrid.Keys)
        {
            int x = (int)position.x;
            int y = (int)position.y;

            Block leftBlock = null, rightBlock = null, upBlock = null, downBlock = null;

            if (x - 1 >= 0)
                leftBlock = blockInGrid[new Vector2(x - 1, y)];

            if (x + 1 < width)
                rightBlock = blockInGrid[new Vector2(x + 1, y)];

            if (y + 1 < height)
                upBlock = blockInGrid[new Vector2(x, y + 1)];

            if (y - 1 >= 0)
                downBlock = blockInGrid[new Vector2(x, y - 1)];

            blockInGrid[position].InitBlock(position, leftBlock, rightBlock, upBlock, downBlock);
        }
    }

    void CheckGameStatusOnMoveDone()
    {
        if (playerCount == 2 && currentState == GameState.MOVE_DONE)
        {
            if (checkStatusTimer < checkStatusMaxTimer)
            {
                checkStatusTimer += Time.deltaTime;
                return;
            }
            bool weHaveAWinner = CheckPlayerStatus();
            if (weHaveAWinner) ChangeState(GameState.END);

            checkStatusTimer = 0;
        }
    }


    #region Game Logic

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.GAME_START:
                ChangeGridColor();
                ChangeState(GameState.WAIT_FOR_INPUT);
                break;
            case GameState.WAIT_FOR_INPUT:
                if (canWin && WinCondition())
                    ChangeState(GameState.END);
                isChaining = false;
                break;
            case GameState.MOVE_DONE:
                isChaining = true;
                break;
            case GameState.SWITCH_PLAYER:
                if (canWin && CheckPlayerStatus())
                    ChangeState(GameState.END);
                SetNextPlayer();
                switchPlayerEvent?.RaiseEvents();
                maxCurrentChainCount = 0;
                ChangeState(GameState.WAIT_FOR_INPUT);
                break;
            case GameState.CHAIN_REACTION:
                //Wait until the reaction ends
                isChaining = true;
                if (maxCurrentChainCount >= 5)
                    chainReactionEvent?.RaiseEvents();
                UIManager.Instance.UpdateChainCounter(currentPlayerIndex, maxCurrentChainCount);
                ChangeState(GameState.SWITCH_PLAYER);
                break;
            case GameState.END:
                winEvent?.RaiseEvents();
                break;
        }
    }


    #endregion

    void SetNextPlayer()
    {
        do
        {
            currentPlayerIndex += 1;
            if (currentPlayerIndex >= playerCount)
            {
                currentPlayerIndex = 0;
                canWin = true;
            }
        }
        while (!playerInGame[currentPlayerIndex].Item2);
        ChangeGridColor();
    }


    bool CheckPlayerStatus()
    {
        HashSet<PlayerSettings> playersFound = new HashSet<PlayerSettings>(
            blockInGrid.Values
                .Where(block => !block.GetBlockInfo().isEmpty)
                .Select(block => block.GetBlockInfo().player)
        );

        playerInGame = playerInGame.Select(tuple => Tuple.Create(tuple.Item1, playersFound.Contains(tuple.Item1)))
            .ToList();

        return WinCondition();
    }

    bool WinCondition()
    {
        for (int i = 0; i < playerInGame.Count; i++)
        {
            if (!playerInGame[i].Item2)
                UIManager.Instance?.PlayerEliminated(i);
        }

        return remainingPlayerAmount == 1;
    }

    void ChangeGridColor()
    {
        blockInGrid.Values.ToList()
            .ForEach(block => block.ChangeColor(GetCurrentPlayerSettings().sphereColor));
    }

    // Utility

    public PlayerSettings GetCurrentPlayerSettings() => playerInGame[currentPlayerIndex].Item1;
    public int remainingPlayerAmount => playerInGame.Count(tuple => tuple.Item2);
}

[Serializable]
public enum GameState
{
    MENU,
    OPTIONS,
    GAME_START,
    WAIT_FOR_INPUT,
    SWITCH_PLAYER,
    CHAIN_REACTION,
    MOVE_DONE,
    END
}
