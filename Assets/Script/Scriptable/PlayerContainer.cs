using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player container")]
public class PlayerContainer : ScriptableObject
{
    public PlayerSettings[] players;
}

