﻿using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level2", order = 3)]
public class LevelCfg2 : LevelCfg
{
    public float cameraFollowX, loadSecondPartX, loadThirdPartX;
    public string gravityInstruction;
    public float gravityInstructionDuration;
}
