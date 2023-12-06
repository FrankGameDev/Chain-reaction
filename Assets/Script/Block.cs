using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Block : CancellableMonoBehaviour
{

    //Block properties
    private Vector2 _blockPosition;
    private Block _leftBlock,
        _rightBlock,
        _upBlock,
        _downBlock;
    private SpriteRenderer spriteRenderer;

    //Define max number of sphere in the block
    private int MaxValue
    {
        get
        {
            int max = 0;
            if (_leftBlock != null) max += 1;
            if (_rightBlock != null) max += 1;
            if (_downBlock != null) max += 1;
            if (_upBlock != null) max += 1;
            return max;
        }
    }
    // Informazioni sul blocco
    private BlockInfo blockInfo;

    //Sphere
    [Header("Spheres section")]
    public GameObject[] spheres = new GameObject[3];
    private GameObject currentSphere;
    public GameObject explosionSphere;

    // ==== Game Logic parameters
    private PlayerSettings currentPlayer
    {
        get => GameManager.Instance.GetCurrentPlayerSettings();
    }

    [Header("Events")]
    public GameEvent explosionEvent;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChangeColor(currentPlayer.sphereColor);
        CreateCancellationToken();
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }

    public void InitBlock(Vector2 pos, Block left = null, Block right = null, Block up = null, Block down = null)
    {
        _blockPosition = pos;

        _leftBlock = left;
        _rightBlock = right;
        _upBlock = up;
        _downBlock = down;

        blockInfo = new BlockInfo();
    }

    private async void OnMouseDown()
    {
        if (GameManager.Instance.isChaining || (!blockInfo.isEmpty && !blockInfo.player.Equals(currentPlayer)))
            return;

        GameManager.Instance.ChangeState(GameState.MOVE_DONE);

        AudioManager.Instance.PlayAddSphereSound();
        await AddSphere();

        if (cancellationTokenSource.IsCancellationRequested) 
            return;

        GameManager.Instance.ChangeState(GameState.CHAIN_REACTION);
    }

    async Task AddSphere(bool chaining = false)
    {
        CreateCancellationToken();

        SetBlockInfo();

        if (blockInfo.sphereAmount >= MaxValue)
        {
            await Explode();
        }
        else
        {
            Destroy(currentSphere?.gameObject);
            currentSphere = Instantiate(spheres[blockInfo.sphereAmount - 1], transform.position, Quaternion.identity);
            currentSphere.transform.SetParent(transform);
            currentSphere.GetComponent<Sphere>().ChangeColor(blockInfo.player.sphereColor);
        }

        if (chaining)
            GameManager.Instance.chainingBlockCount -= 1;

        await Task.Yield();
        if (cancellationTokenSource.IsCancellationRequested) 
            return;
    }


    //Propaga l'esplosione verso i blocchi adiacenti
    private async Task Explode()
    {
        blockInfo.sphereAmount = 0;
        Destroy(currentSphere?.gameObject);
        currentSphere = null;

        Sequence explosions = DOTween.Sequence();

        explosions.Join(ExplodeAndAddSphere(_leftBlock))
        .Join(ExplodeAndAddSphere(_rightBlock))
        .Join(ExplodeAndAddSphere(_upBlock))
        .Join(ExplodeAndAddSphere(_downBlock));

        await explosions.Play().AsyncWaitForCompletion();

        if (cancellationTokenSource.IsCancellationRequested)
            return;

        var actions = new List<Task>
        {
            _leftBlock?.AddSphere(true),
            _rightBlock?.AddSphere(true),
            _upBlock?.AddSphere(true),
            _downBlock?.AddSphere(true)
        }
        .Where(a => a != null)
        .ToList();

        await Task.WhenAll(actions);
        if (cancellationTokenSource.IsCancellationRequested)
            return;
    }

    private Tween ExplodeAndAddSphere(Block nextBlock)
    {
        if (nextBlock == null)
            return DOTween.Sequence().AppendInterval(0);

        GameManager.Instance.chainingBlockCount += 1;
        var spawnedSphere = Instantiate(explosionSphere, transform.position, Quaternion.identity);
        spawnedSphere.transform.SetParent(transform);
        spawnedSphere.GetComponent<Sphere>().ChangeColor(blockInfo.player.sphereColor);
        explosionEvent?.RaiseEvents();

        return spawnedSphere.transform.DOMove(nextBlock._blockPosition, .15f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Destroy(spawnedSphere);
            });
    }


    private void SetBlockInfo()
    {
        blockInfo.sphereAmount += 1;
        blockInfo.player = currentPlayer;
    }

    public BlockInfo GetBlockInfo() => blockInfo;

    public void ChangeColor(Color color) => spriteRenderer.material.color = color;
}


public struct BlockInfo
{
    public PlayerSettings player;
    public int sphereAmount;
    public bool isEmpty => sphereAmount == 0 || player == null;

    public override string ToString() => $"player: {player.name}, {player.sphereColor};\nSphere amount: {sphereAmount};";
}
