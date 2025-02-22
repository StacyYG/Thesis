﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameCfg", order = 0)]
public class GameCfg : ScriptableObject
{
    [Header("Target Square")] 
    public Color liveColor;
    public Color hurtColor;
    [Header("VectorLines Texture")] 
    public Texture2D frontTexture;
    public Texture2D dashedLineTexture;
    public Texture2D fullLineTexture;
    [Header("Force Settings")] 
    public Color currentForceColor;
    public Color previousNetForceColor;
    public Color currentNetForceColor;
    public float forceLineWidth;
    public float vectorMultiplier;
    [Header("Velocity Line settings")] 
    public float velocityLineWidth;
    public Color velocityLineColor;
    public Color velocityLineHighlightColor;
    [Header("Bound Circle")] 
    public Color boundCircleColor;
    public int segmentsPerRadius;
    [Header("Prefabs")]
    public GameObject successParticles;
    public GameObject shade;
    [Header("Success")] 
    public string whenSuccess;
    public float afterSuccessWaitTime;
}
