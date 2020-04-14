using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions0
{
    private static LevelCfg0 _levelData;
    public List<InstructionItem> InitialInstructions;
    public List<InstructionItem> LastInstructions;

    public static Instructions0 Load(LevelCfg0 data)
    {
        var toReturn = new Instructions0();
        _levelData = data;
        toReturn.InitialInstructions = new List<InstructionItem>();
        toReturn.LastInstructions = new List<InstructionItem>();
        for (int i = 0; i < _levelData.initialInstructions.Count; i++)
        {
            toReturn.InitialInstructions.Add(_levelData.initialInstructions[i]);
        }

        for (int i = 0; i < _levelData.lastInstructions.Count; i++)
        {
            toReturn.LastInstructions.Add(_levelData.lastInstructions[i]);
        }
        return toReturn;
    }
}

[System.Serializable]
public struct InstructionItem
{
    public int StartTime;
    public string Content;
}
