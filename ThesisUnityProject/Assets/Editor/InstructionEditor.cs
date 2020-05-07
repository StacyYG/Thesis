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
    public Transform container;
    public GameObject tmpPrefab;
    public LevelCfg levelData;
    private List<GameObject> _textObjects = new List<GameObject>();
    //public int toRemove = 0;
    private List<string> _names = new List<string>();
    public int eventIndex = 0;
    public List<InstructionData> instructions;
    public int selectedInstruction = 0;
    private InstructionData _selected = new InstructionData();
    private InstructionData _new = new InstructionData();
    [MenuItem("Tools/Instruction Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(InstructionEditor));
    }
    
    private void OnGUI()
    {
        if (EditorApplication.isPlaying) return;
        container =
            EditorGUILayout.ObjectField("Container", container, typeof(Transform), true) as Transform;
        tmpPrefab = EditorGUILayout.ObjectField("TMP Prefab", tmpPrefab, typeof(GameObject), false) as GameObject;
        levelData = EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelCfg), false) as LevelCfg;
                
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
        
        EditorGUI.BeginDisabledGroup(container == null || tmpPrefab == null || levelData == null || container.childCount != _textObjects.Count);
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        GUILayout.Label("Add New", EditorStyles.boldLabel);
        
        _new.textPosition = EditorGUILayout.Vector3Field("Position", _new.textPosition);
        _new.content = EditorGUILayout.TextField("Content", _new.content);
        _new.duration = EditorGUILayout.FloatField("Duration", _new.duration);
        _new.whenToShow = (Options) EditorGUILayout.EnumPopup("When to show", _new.whenToShow);

        var typeList = typeof(AGPEvent).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(AGPEvent)))
            .ToList();
        var stringList = new List<string>();
        for (int i = 0; i < typeList.Count; i++)
        {
            stringList.Add(typeList[i].ToString());
        }
        if (_selected.whenToShow == Options.EventBased)
        {
            eventIndex = EditorGUILayout.Popup("Event", eventIndex, stringList.ToArray());
        }
        
        if (GUILayout.Button("Add"))
        {
            var textObj = Instantiate(tmpPrefab, _new.textPosition, Quaternion.identity, container);
            var tmp = textObj.GetComponent<TextMeshPro>();
            tmp.text = _new.content;
            textObj.name = _new.content.Substring(0, Mathf.Min(10, _new.content.Length));
            levelData.instructions.Add(_new);
            instructions.Add(_new);
            _textObjects.Add(textObj);
            _names.Add(textObj.name);
            selectedInstruction = instructions.Count - 1;
        }
        EditorGUILayout.Space();
        GUILayout.Label("Edit", EditorStyles.boldLabel);
        selectedInstruction = EditorGUILayout.Popup("Selected", selectedInstruction, _names.ToArray());
        if (instructions.Count != 0)
        {
            _selected = instructions[selectedInstruction];
            _selected.textPosition = EditorGUILayout.Vector3Field("Position", _selected.textPosition);
            _selected.content = EditorGUILayout.TextField("Content", _selected.content);
            _selected.duration = EditorGUILayout.FloatField("Duration", _selected.duration);
            _selected.whenToShow = (Options) EditorGUILayout.EnumPopup("When to show", _selected.whenToShow);
            instructions[selectedInstruction] = _selected;
        }
        
        if(container != null) 
            if (container.childCount != _textObjects.Count)
            {
                EditorGUILayout.HelpBox("container: " + container.childCount + "; textObjects: " + _textObjects.Count, MessageType.Warning);
            }
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Update"))
        {
            _names = new List<string>();
            for (int i = 0; i < instructions.Count; i++)
            {
                var content = instructions[i].content;
                var objName = content.Substring(0, Mathf.Min(10, content.Length));
                _textObjects[i].GetComponent<TextMeshPro>().text = content;
                _textObjects[i].name = objName;
                _names.Add(objName);
                
                var data = levelData.instructions[i];
                data.content = instructions[i].content;
                data.duration = instructions[i].duration;
                data.myEvent = instructions[i].myEvent;
                data.textPosition = instructions[i].textPosition;
                data.whenToShow = instructions[i].whenToShow;
                levelData.instructions[i] = data;
            }
        }
        EditorGUILayout.Space();
        if (_textObjects.Count > 0)
        {
            if (GUILayout.Button("Remove"))
            {
                DestroyImmediate(_textObjects[selectedInstruction]);
                _textObjects.RemoveAt(selectedInstruction);
                levelData.instructions.RemoveAt(selectedInstruction);
                instructions.RemoveAt(selectedInstruction);
                _names.RemoveAt(selectedInstruction);
                selectedInstruction = selectedInstruction == 0 ? 0 : selectedInstruction - 1;
            }
        }
        EditorGUILayout.Space();
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
            selectedInstruction = 0;
        }

    }

    private void Update()
    {
        if (EditorApplication.isPlaying) return;
        for (int i = 0; i < instructions.Count; i++)
        {
            _textObjects[i].transform.position = instructions[i].textPosition;
        }
    }
}