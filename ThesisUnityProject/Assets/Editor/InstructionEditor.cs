using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InstructionEditor : EditorWindow
{
    public Vector3 textPosition = new Vector3(0f, 1.5f, 0f);
    public string content = "";
    public Options whenToShow = Options.FollowPrior;
    public float duration = 3f;

    [MenuItem("Tools/Instruction Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(InstructionEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Edit Instruction", EditorStyles.boldLabel);
        textPosition = EditorGUILayout.Vector3Field("Position", textPosition);
        content = EditorGUILayout.TextField("Content", content);
        whenToShow = (Options)EditorGUILayout.EnumPopup("When to show", whenToShow);
        duration = EditorGUILayout.FloatField("Duration", duration);
        if (GUILayout.Button("Put something here later"))
        {
            
        }
    }
}
public enum Options
{
    EventBased,
    FollowPrior
}