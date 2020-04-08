using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level0", order = 1)]
public class LevelCfg0 : ScriptableObject
{
    public Vector2 v0 = new Vector2(2f, 0f);
    public int addForceInstruction;
    public int firstForceIndex;
    public List<string> texts;
    public string whenFirstForce;
    public string whenSecondForce;
    public float showTargetSqrTime;
    public float showCtrlSqrTime;
    public float allowControlTime;
    public float secondForceRemindTime;
    public string secondForceReminder;
}
