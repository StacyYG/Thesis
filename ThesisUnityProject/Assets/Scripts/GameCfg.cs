using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameCfg", order = 0)]
public class GameCfg : ScriptableObject
{
    public Color targetSqrColor;
    public Color deadColor;
}
