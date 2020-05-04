using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class InstructionEditor : EditorWindow
{
    public Vector3 textPosition = new Vector3(0f, 1.5f, 0f);
    public string content = "";
    public Options whenToShow = Options.FollowPrior;
    public float duration = 3f;
    public Transform container;
    public GameObject tmpPrefab;
    public LevelCfg levelData;
    private Dictionary<TextMeshPro, InstructionItem> _tmpToItem;
    private Dictionary<InstructionItem, TextMeshPro> _itemToTmp;

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
        container =
            EditorGUILayout.ObjectField("Container", container, typeof(Transform), true) as Transform;
        tmpPrefab = EditorGUILayout.ObjectField("TMPPrefab", tmpPrefab, typeof(GameObject), false) as GameObject;
        levelData = EditorGUILayout.ObjectField("Scriptable Object", levelData, typeof(LevelCfg), false) as LevelCfg;
        if (GUILayout.Button("Add"))
        {
            var textObj = Instantiate(tmpPrefab, textPosition, Quaternion.identity, container);
            textObj.GetComponent<TextMeshPro>().text = content;
            textObj.name = content.Substring(0, Mathf.Min(10, content.Length));
            var instruction = CreateInstruction();
            levelData.instructions.Add(instruction);
        }
        
    }

    private InstructionItem CreateInstruction()
    {
        var instruction = new InstructionItem();
        instruction.textPosition = textPosition;
        instruction.content = content;
        instruction.whenToShow = whenToShow;
        instruction.duration = duration;
        return instruction;
    }

    private string _lastTmpText;
    private string _lastItemText;
    private TextMeshPro[] _tmps;
    private InstructionItem[] _items;
    private void Update()
    {
        // _tmps = container.GetComponentsInChildren<TextMeshPro>();
        // //_items = 
        // for (int i = 0; i < _tmps.Length; i++)
        // {
        //     if (_tmps[i].text != _lastTmpText)
        //     {
        //         InstructionItem item;
        //         _tmpToItem.TryGetValue(_tmps[i], out item);
        //         item.content = _tmps[i].text;
        //         _lastTmpText = _tmps[i].text;
        //         return;
        //     }
        // }
    }
}