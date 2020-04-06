using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level0", order = 1)]
public class LevelCfg0 : ScriptableObject
{
    public Vector2 v0 = new Vector2(2f, 0f);
    public int showTargetSqrIndex;
    public int showCtrlSqrIndex;
    public int alreadyForceIndex;
    public int addForceInstruction;
    public int firstForceIndex;
    public List<string> texts;
}
