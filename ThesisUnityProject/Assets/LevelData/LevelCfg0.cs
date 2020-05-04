using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level0", order = 1)]
public class LevelCfg0 : LevelCfg
{
    public Vector2 v0 = new Vector2(2f, 0f);
    [Header("Initial Instructions")]
    public float showTargetSqrTime;
    public float showCtrlSqrTime;
    public List<InstructionItem> initialInstructions;
    [Header("Responses-First half")]
    public string whenFirstForce;
    public float secondForceRemindTime;
    public string secondForceReminder;
    public string whenSecondForce;
    public float secondForceInstructionDuration;
    [Header("Chase")]
    public List<Vector3> chaseItemPositions;
    public GameObject chaseItem;
    public string chaseExplanation;
    [Header("Goal")]
    public string goalExplanation;
    public float failThreshold;
    public string lost;
    public string whenSuccess;
    public float nextLevelLoadTime;
    public string nextLevelText;
}

public class LevelCfg : ScriptableObject
{
    [Header("New Instructions")] 
    public List<InstructionItem> instructions;
}
