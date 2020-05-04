using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

public class InstructionEditor : EditorWindow
{
    public Vector3 textPosition = new Vector3(0f, 1.5f, 0f);
    public string content = "";
    public Options whenToShow = Options.FollowPrior;
    public float duration = 3f;
    public Transform container;
    public GameObject tmpPrefab;
    public LevelCfg levelData;

    [MenuItem("Tools/Instruction Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(InstructionEditor));
    }
    
    private void OnGUI()
    {
        if (EditorApplication.isPlaying) return;

        GUILayout.Label("Edit Instruction", EditorStyles.boldLabel);
        textPosition = EditorGUILayout.Vector3Field("Position", textPosition);
        content = EditorGUILayout.TextField("Content", content);
        whenToShow = (Options)EditorGUILayout.EnumPopup("When to show", whenToShow);
        duration = EditorGUILayout.FloatField("Duration", duration);
        container =
            EditorGUILayout.ObjectField("Container", container, typeof(Transform), true) as Transform;
        tmpPrefab = EditorGUILayout.ObjectField("TMP Prefab", tmpPrefab, typeof(GameObject), false) as GameObject;
        levelData = EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelCfg), false) as LevelCfg;
        if (GUILayout.Button("Add"))
        {
            var textObj = Instantiate(tmpPrefab, textPosition, Quaternion.identity, container);
            var tmp = textObj.GetComponent<TextMeshPro>();
            tmp.text = content;
            textObj.name = content.Substring(0, Mathf.Min(10, content.Length));
            var instruction = CreateInstruction(textObj.name);
            levelData.instructions.Add(instruction);
        }

        if (GUILayout.Button("Clear"))
        {
            var childNum = container.childCount;
            for (int i = 0; i < childNum; i++)
            {
                DestroyImmediate(container.GetChild(0).gameObject);
            }

            levelData.instructions.Clear();
        }

        if (GUILayout.Button("Update"))
        {
            foreach (Transform child in container.transform)
            {
                var text = child.GetComponent<TextMeshPro>().text;
                child.name = text.Substring(0, Mathf.Min(10, text.Length));
            }

            for (int i = 0; i < levelData.instructions.Count; i++)
            {
                var item = levelData.instructions[i];
                item.textObjName = container.GetChild(i).name;
                levelData.instructions[i] = item;
            }
            Debug.Log("updated");
        }
    }

    private InstructionData CreateInstruction(string textObjName)
    {
        var instruction = new InstructionData();
        instruction.whenToShow = whenToShow;
        instruction.duration = duration;
        instruction.textObjName = textObjName;
        return instruction;
    }

    private List<string> _lastTmpTexts;
    private List<string> _lastItemTexts;
    private TextMeshPro[] _tmps;
    private InstructionData[] _items;
    private void Update()
    {
        if (EditorApplication.isPlaying) return;
    }
}