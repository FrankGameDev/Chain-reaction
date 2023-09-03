using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player settings")]
public class PlayerSettings : ScriptableObject
{
    public string playerName;
    public Color sphereColor;

    public void Init(string name, Color color)
    {
        playerName = name;
        sphereColor = color;
    }

}