using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions0
{
    private static LevelCfg0 _data0;
    public List<InstructionItem> InitialInstructions;

    public static Instructions0 Load(LevelCfg0 data)
    {
        var toReturn = new Instructions0();
        _data0 = data;
        toReturn.InitialInstructions = new List<InstructionItem>();
        foreach (var instruction in _data0.initialInstructions)
        {
            toReturn.InitialInstructions.Add(instruction);
        }
        return toReturn;
    }
}

[System.Serializable]
public struct InstructionItem
{
    public float startTime;
    public string content;
    public float duration;
    public Vector3 textPosition;
    public Options whenToShow;
}

public enum Options
{
    EventBased,
    FollowPrior
}