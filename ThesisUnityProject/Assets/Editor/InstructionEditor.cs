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
    private List<GameObject> _textObjects = new List<GameObject>();
    public int toRemove = 0;
    private List<string> _names = new List<string>();

    [MenuItem("Tools/Instruction Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(InstructionEditor));
    }
    
    private void OnGUI()
    {
        if (EditorApplication.isPlaying) return;

        GUILayout.Label("Edit Instruction", EditorStyles.boldLabel);
        container =
            EditorGUILayout.ObjectField("Container", container, typeof(Transform), true) as Transform;
        tmpPrefab = EditorGUILayout.ObjectField("TMP Prefab", tmpPrefab, typeof(GameObject), false) as GameObject;
        levelData = EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelCfg), false) as LevelCfg;
        
        EditorGUI.BeginDisabledGroup(container == null || tmpPrefab == null || levelData == null || container.childCount != _textObjects.Count);
        textPosition = EditorGUILayout.Vector3Field("Position", textPosition);
        content = EditorGUILayout.TextField("Content", content);
        duration = EditorGUILayout.FloatField("Duration", duration);
        whenToShow = (Options)EditorGUILayout.EnumPopup("When to show", whenToShow);
        if (whenToShow == Options.EventBased)
        {
            GUILayout.Label("something", EditorStyles.popup);
        }
        if (GUILayout.Button("Add"))
        {
            var textObj = Instantiate(tmpPrefab, textPosition, Quaternion.identity, container);
            var tmp = textObj.GetComponent<TextMeshPro>();
            tmp.text = content;
            textObj.name = content.Substring(0, Mathf.Min(10, content.Length));
            var instruction = CreateInstruction(textObj.name);
            levelData.instructions.Add(instruction);
            _textObjects.Add(textObj);
            _names.Add(textObj.name);
        }
        

        if (GUILayout.Button("Update"))
        {
            _names = new List<string>();
            foreach (var textObject in _textObjects)
            {
                var text = textObject.GetComponent<TextMeshPro>().text;
                textObject.name = text.Substring(0, Mathf.Min(10, text.Length));
                _names.Add(textObject.name);
            }

            if(levelData != null) 
                for (int i = 0; i < levelData.instructions.Count; i++)
                {
                    var data = levelData.instructions[i];
                    data.textObjName = _textObjects[i].name;
                    levelData.instructions[i] = data;
                }
        }

        if (_textObjects.Count > 0)
        {
            toRemove = EditorGUILayout.Popup("To Remove", toRemove, _names.ToArray());
            if (GUILayout.Button("Remove"))
            {
                DestroyImmediate(_textObjects[toRemove]);
                _textObjects.RemoveAt(toRemove);
                if(levelData != null) 
                    levelData.instructions.RemoveAt(toRemove);
                _names.RemoveAt(toRemove);
            }
        }

        if(container != null)
            if (container.childCount != _textObjects.Count)
            {
                EditorGUILayout.HelpBox("Please use the Remove button instead of deleting game objects directly", MessageType.Warning);
            }
        
        EditorGUI.EndDisabledGroup();
        
        if (GUILayout.Button("Clear"))
        {
            for (int i = 0; i < _textObjects.Count; i++)
                DestroyImmediate(_textObjects[i]);
            _textObjects.Clear();
            if(levelData != null) 
                levelData.instructions.Clear();
            _names.Clear();
        }
        
        EditorGUILayout.Space();
        if (container == null)
        {
            EditorGUILayout.HelpBox("Assign a container game object for the instructions", MessageType.Warning);
        }
        if (tmpPrefab == null)
        {
            EditorGUILayout.HelpBox("Assign TextMeshPro prefab", MessageType.Warning);
        }
        if (levelData == null)
        {
            EditorGUILayout.HelpBox("Assign a level data scriptable object", MessageType.Warning);
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

    private void Update()
    {
        if (EditorApplication.isPlaying) return;
    }
}