using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level1", order = 2)]
public class LevelCfg1 : ScriptableObject
{
    public Vector2 v0;
    [Header("Cancel Button Instructions")] 
    public float showCancelButtonTime;

    public float waitTime;
    public string cancelInstruction;    
    public List<InstructionItem> cancelInstructions;
    [Header("Responses-Second half")] 
    public string whenFirstCancel;
    public float duration;
    [Header("Chase")]
    public List<Vector3> chaseItemPositions;

    public GameObject chaseItem;
    [Header("Goal")]
    public string goalExplanation;
    public List<string> errorHints;
    public string whenSuccess;
}
