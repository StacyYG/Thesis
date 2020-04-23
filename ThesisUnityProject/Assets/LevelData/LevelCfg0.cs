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
    [Header("Goal")]
    public string goalExplanation;
    public string whenSuccess;
    public float nextLevelLoadTime;
    public string nextLevelText;
}
