using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level0", order = 1)]
public class LevelCfg0 : ScriptableObject
{
    public Vector2 v0 = new Vector2(2f, 0f);
    [Header("Initial Instructions")]
    public float showTargetSqrTime;
    public float showCtrlSqrTime;
    public float allowControlTime;
    [FormerlySerializedAs("texts")] public List<string> initialInstructions;
    [Header("Responses")]
    public string whenFirstForce;
    public float secondForceRemindTime;
    public string secondForceReminder;
    public string whenSecondForce;
    public float secondForceInstructionDuration;
    [Header("Last Instructions")] 
    public float lastInstructionsWaitTime;
    public float showCancelButtonTime;
    public List<string> lastInstructions;
}
