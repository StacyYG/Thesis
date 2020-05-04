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
    private Dictionary<TextMeshPro, InstructionData> _tmpToItem = new Dictionary<TextMeshPro, InstructionData>();
    private Dictionary<InstructionData, TextMeshPro> _itemToTmp = new Dictionary<InstructionData, TextMeshPro>();

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
            var tmp = textObj.GetComponent<TextMeshPro>();
            tmp.text = content;
            textObj.name = content.Substring(0, Mathf.Min(10, content.Length));
            var instruction = CreateInstruction(textObj.name);
            levelData.instructions.Add(instruction);
            // _itemToTmp.Add(instruction, tmp);
            // _tmpToItem.Add(tmp, instruction);
        }

        if (GUILayout.Button("Clear"))
        {
            foreach (Transform child in container.transform)
            {
                DestroyImmediate(child.gameObject);
            }

            levelData.instructions.Clear();
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
        // _tmps = container.GetComponentsInChildren<TextMeshPro>();
        // _items = levelData.instructions.ToArray();
        // for (int i = 0; i < _tmps.Length; i++)
        // {
        //     InstructionItem item;
        //     if (!_tmpToItem.TryGetValue(_tmps[i], out item))
        //     {
        //         Debug.Log("cannot get instruction item");
        //         return;
        //     }
        //     item.content = _tmps[i].text;
        //     Debug.Log(item.content);
        //
        //     var temp = levelData.instructions[i];
        //     temp.content = _tmps[i].text;
        //     levelData.instructions[i] = temp;
        //     //levelData.instructions[i].content = _tmps[i].text;
        //     return;
        // }
        //
        // for (int i = 0; i < _items.Length; i++)
        // {
        //     TextMeshPro tmp;
        //     if (!_itemToTmp.TryGetValue(_items[i], out tmp))
        //     {
        //         Debug.Log("cannot get TextMeshPro");
        //         return;
        //     }
        //
        //     tmp.text = _items[i].content;
        // }
    }
}