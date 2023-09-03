using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [Header("Player count parameters")]
    public TextMeshProUGUI playerCountText;
    private int playerCount;
    private int minPlayer = 2, maxPlayer = 4;


    [Header("Player ui info customization")]
    public RectTransform playerInfoGrid;
    public PlayerInfoCustomizationSetup playerInfoCustomizationOptions;
    private List<PlayerInfoCustomizationSetup> playersInfoList = new List<PlayerInfoCustomizationSetup>();

    [Header("Player settings scriptable references")]
    public PlayerSettings[] playerSettings;

    private void Awake()
    {
        playerCount = minPlayer;
    }

    public void ChangePlayerCount(int value)
    {
        playerCount += value;
        if (playerCount < minPlayer) playerCount = minPlayer;
        else if (playerCount > maxPlayer) playerCount = maxPlayer;

        playerCountText.text = playerCount.ToString();
    }

    public void OverwritePlayerName(PlayerSettings player, string name)
    {
        player.playerName = name;
    }

    public void ShowPlayerCustomization()
    {
        for (int i = 0; i < playerCount; i++)
        {
            PlayerInfoCustomizationSetup spawnedInfos = Instantiate(playerInfoCustomizationOptions, playerInfoGrid.anchoredPosition,
                Quaternion.identity);
            spawnedInfos.transform.SetParent(playerInfoGrid, false);
            spawnedInfos.SetPlayerNumber(i + 1);
            playersInfoList.Add(spawnedInfos);
        }
    }

    public void ResetPlayerCustomizations()
    {
        playersInfoList.ForEach(x => Destroy(x.gameObject));
        playersInfoList.Clear();
    }

    public void LoadGame()
    {
        //set player amount
        PlayerPrefs.SetInt("PlayerCount", playerCount);


        //Set all the players info
        for (int i = 0; i < playersInfoList.Count; i++)
        {
            string playerName = (playersInfoList[i].GetPlayerName().Length > 0) ? playersInfoList[i].GetPlayerName() : $"Player {i + 1}";

            playerSettings[i].Init(playerName, playersInfoList[i].GetColor());
        }


        SceneManager.LoadScene("GameScene");
    }


}
