using DamageNumbersPro;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogWarning("Game manager is NULL");
            return _instance;
        }
    }

    [Header("Player info UI")]
    public PlayerInfoUI[] playersInfo;

    [Header("Chain reaction effects")]
    public DamageNumber chainReactionText;
    public RectTransform effectPosition;

    [Header("Win UI")]
    public DamageNumber winText;
    public RectTransform winTextPosition;

    private PlayerSettings currentPlayer => GameManager.Instance.GetCurrentPlayerSettings();


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
    }

    private void Start()
    {
        InitPlayerInfoUI();
    }

    public void Win()
    {
        DamageNumber tmp = winText.Spawn(winTextPosition.anchoredPosition, $"{currentPlayer.playerName} wins", currentPlayer.sphereColor);
        tmp.SetAnchoredPosition(winTextPosition.transform, Vector2.zero);
    }

    public void InitPlayerInfoUI()
    {
        for (int i = 0; i < GameManager.Instance.playerCount; i++)
        {
            PlayerSettings p = GameManager.Instance.playerContainer.players[i];
            playersInfo[i].Init(p.playerName, p.sphereColor);
        }
    }

    public void UpdateChainCounter(int playerIndex, int count)
    {
        if (playerIndex >= playersInfo.Length)
            return;

        playersInfo[playerIndex].UpdateCounter(count);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ChainReactionEffect()
    {
        var textEffect = chainReactionText.Spawn(effectPosition.anchoredPosition);
        textEffect.SetAnchoredPosition(effectPosition, Vector2.zero);
    }

    public void PlayerEliminated(int i) => playersInfo[i].deleteImage.enabled = true;

}

[Serializable]
public struct PlayerInfoUI
{
    public GameObject reference;
    public Image frame;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI chainText;
    public TextMeshProUGUI chainCount;
    public Image deleteImage;

    public int maxChainCount;

    public void Init(string text, Color color)
    {
        reference.SetActive(true);
        playerName.text = text;
        playerName.color = color;
        chainText.color = color;
        chainCount.color = color;
        chainCount.text = "0";
        frame.color = new Color(color.r * .5f, color.g * .5f, color.b * .5f) ;
        deleteImage.color = color;
        deleteImage.enabled = false;
    }

    public void UpdateCounter(int value)
    {
        if (value > maxChainCount)
        {
            chainCount.text = value.ToString();
            maxChainCount = value;
        }
    }

}