using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameCfg", order = 0)]
public class GameCfg : ScriptableObject
{
    [Header("Target Square")] 
    public Color liveColor;
    public Color hurtColor;
    public Color deadColor;
    [Header("VectorLines Texture")] 
    public Texture2D frontTexture;
    public Texture2D dashedLineTexture;
    public Texture2D fullLineTexture;
    [Header("Force Settings")] 
    public Color currentForceColor;
    public Color previousNetForceColor;
    public Color currentNetForceColor;
    public float lineWidth;
    [Header("Life Settings")] 
    public int startLifeNum;
    [Header("Prefabs")] 
    public GameObject lifeBox;

    public GameObject rawLife;
}
