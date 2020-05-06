using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEditor;
using System.Linq;

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
    public int eventIndex = 0;
    public List<InstructionData> instructions;
    public int selectedInstruction = 0;
    [MenuItem("Tools/Instruction Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(InstructionEditor));
    }
    
    private void OnGUI()
    {
        if (EditorApplication.isPlaying) return;

        GUILayout.Label("Add new", EditorStyles.boldLabel);
        container =
            EditorGUILayout.ObjectField("Container", container, typeof(Transform), true) as Transform;
        tmpPrefab = EditorGUILayout.ObjectField("TMP Prefab", tmpPrefab, typeof(GameObject), false) as GameObject;
        levelData = EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelCfg), false) as LevelCfg;
        
        EditorGUI.BeginDisabledGroup(container == null || tmpPrefab == null || levelData == null || container.childCount != _textObjects.Count);
        textPosition = EditorGUILayout.Vector3Field("Position", textPosition);
        content = EditorGUILayout.TextField("Content", content);
        duration = EditorGUILayout.FloatField("Duration", duration);
        whenToShow = (Options)EditorGUILayout.EnumPopup("When to show", whenToShow);
        var typeList = typeof(AGPEvent).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(AGPEvent)))
            .ToList();
        var stringList = new List<string>();
        for (int i = 0; i < typeList.Count; i++)
        {
            stringList.Add(typeList[i].ToString());
        }
        if (whenToShow == Options.EventBased)
        {
            eventIndex = EditorGUILayout.Popup("Event", eventIndex, stringList.ToArray());
        }
        if (GUILayout.Button("Add"))
        {
            var textObj = Instantiate(tmpPrefab, textPosition, Quaternion.identity, container);
            var tmp = textObj.GetComponent<TextMeshPro>();
            tmp.text = content;
            textObj.name = content.Substring(0, Mathf.Min(10, content.Length));
            var instruction = CreateInstruction(textObj.name, typeList[eventIndex]);
            levelData.instructions.Add(instruction);
            instructions.Add(instruction);
            _textObjects.Add(textObj);
            _names.Add(textObj.name);
        }
        
        EditorGUILayout.Space();
        GUILayout.Label("Remove", EditorStyles.boldLabel);
        if (_textObjects.Count > 0)
        {
            //toRemove = 0;
            toRemove = EditorGUILayout.Popup("To Remove", toRemove, _names.ToArray());
            if (GUILayout.Button("Remove"))
            {
                DestroyImmediate(_textObjects[toRemove]);
                _textObjects.RemoveAt(toRemove);
                if(levelData != null) 
                    levelData.instructions.RemoveAt(toRemove);
                instructions.RemoveAt(toRemove);
                _names.RemoveAt(toRemove);
            }
        }
        EditorGUILayout.Space();
        
        GUILayout.Label("Edit", EditorStyles.boldLabel);
        selectedInstruction = EditorGUILayout.Popup("To Edit", selectedInstruction, _names.ToArray());
        
        if(container != null)
            if (container.childCount != _textObjects.Count)
            {
                EditorGUILayout.HelpBox("Please use the Remove button instead of deleting game objects directly", MessageType.Warning);
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
        EditorGUI.EndDisabledGroup();
        
        if (GUILayout.Button("Clear"))
        {
            for (int i = 0; i < _textObjects.Count; i++)
                DestroyImmediate(_textObjects[i]);
            _textObjects.Clear();
            if(levelData != null) 
                levelData.instructions.Clear();
            instructions.Clear();
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

    private InstructionData CreateInstruction(string textObjName, Type myEvent)
    {
        var instruction = new InstructionData();
        instruction.whenToShow = whenToShow;
        instruction.duration = duration;
        instruction.textObjName = textObjName;
        instruction.myEvent = myEvent;
        return instruction;
    }

    private void Update()
    {
        if (EditorApplication.isPlaying) return;
    }
}