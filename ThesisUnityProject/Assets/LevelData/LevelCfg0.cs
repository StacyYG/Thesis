using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level0", order = 1)]
public class LevelCfg0 : LevelCfg
{
    public Vector2 v0 = new Vector2(2f, 0f);
    [Header("Initial Instructions")]
    public float showCtrlSqrTime;
    public List<InstructionItem> initialInstructions;
    [Header("Responses")]
    public string whenFirstForce;
    public float secondForceRemindTime;
    public string secondForceReminder;
    public string whenSecondForce;
    public float secondForceInstructionDuration;
    public string chaseExplanation;
    [Header("Goal")]
    public string goalExplanation;
}

public class LevelCfg : ScriptableObject
{
    [Header("New Instructions")] 
    public List<InstructionData> instructions;
}
