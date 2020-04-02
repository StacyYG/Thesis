using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level0", order = 1)]
public class LevelCfg0 : ScriptableObject
{
    public string text1 = "something";
    public Vector2 v0 = new Vector2(2f, 0f);
    public List<string> texts;
    public int showTargetSqr;
    public int showCtrlSqr;
}
