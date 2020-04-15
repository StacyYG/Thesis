using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level1", order = 2)]
public class LevelCfg1 : ScriptableObject
{
    public string explainTriangle;
    public string explainConstantGate;
    public string explainLives;
    public string comfort;

}
