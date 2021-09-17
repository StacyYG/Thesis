using System;
using System.Collections.Generic;
using UnityEngine;

public class Instructions0
{
    private static LevelCfg0 _data0;
    public List<InstructionItem> initialInstructions;

    public static Instructions0 Load(LevelCfg0 data)
    {
        var toReturn = new Instructions0();
        _data0 = data;
        toReturn.initialInstructions = new List<InstructionItem>();
        foreach (var instruction in _data0.initialInstructions)
        {
            toReturn.initialInstructions.Add(instruction);
        }
        return toReturn;
    }
}

[Serializable]
public struct InstructionItem
{
    public float startTime;
    public string content;
    public float duration;
}

public enum Options
{
    EventBased,
    FollowPrior
}

[Serializable]
public struct InstructionData
{
    public string content;
    public Vector3 textPosition;
    public Options whenToShow;
    public float duration;
    public Type myEvent;
    public int eventIndex;
    public string eventName;
}