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
        foreach (var instruction in _levelData.initialInstructions)
        {
            toReturn.InitialInstructions.Add(instruction);
        }

        foreach (var instruction in _levelData.cancelInstructions)
        {
            toReturn.LastInstructions.Add(instruction);
        }
        
        return toReturn;
    }
}

[System.Serializable]
public struct InstructionItem
{
    public int startTime;
    public string content;
}
