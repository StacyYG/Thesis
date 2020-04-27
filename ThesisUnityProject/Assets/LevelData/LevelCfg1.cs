using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level1", order = 2)]
public class LevelCfg1 : ScriptableObject
{
    [Header("Wait For First Force")] 
    public float waitTime;
    public string firstForceReminder;
    [Header("Cancel Button Instructions")] 
    public float showCancelButtonTime;
    public string cancelInstruction;
    [Header("First Cancel Response")] 
    public string whenFirstCancel;
    public float duration;
    [Header("Fail Instructions")]
    public List<InstructionItem> failInstructions;
}
