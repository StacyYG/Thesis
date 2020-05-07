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
    public int toRemove = 0;
    private List<string> _names = new List<string>();
    public int eventIndex = 0;
    public List<InstructionData> instructions;
    public int selectedInstruction = 0;
    private InstructionData _current = new InstructionData();
    private InstructionData _temp = new InstructionData();
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
        EditorGUI.BeginDisabledGroup(container == null || tmpPrefab == null || levelData == null || container.childCount != _textObjects.Count);
        EditorGUILayout.Space();
        selectedInstruction = EditorGUILayout.Popup("Selected", selectedInstruction, _names.ToArray());
        GUILayout.Label("Info", EditorStyles.boldLabel);
        
        if (instructions.Count == 0)
        {
            _current.textPosition = EditorGUILayout.Vector3Field("Position", _current.textPosition);
            _current.content = EditorGUILayout.TextField("Content", _current.content);
            _current.duration = EditorGUILayout.FloatField("Duration", _current.duration);
            _current.whenToShow = (Options) EditorGUILayout.EnumPopup("When to show", _current.whenToShow);
        }
        else
        {
            Debug.Log("selected: " + selectedInstruction);
            _current = instructions[selectedInstruction];
            _current.textPosition = EditorGUILayout.Vector3Field("Position", _current.textPosition);
            _current.content = EditorGUILayout.TextField("Content", _current.content);
            _current.duration = EditorGUILayout.FloatField("Duration", _current.duration);
            _current.whenToShow = (Options) EditorGUILayout.EnumPopup("When to show", _current.whenToShow);
            //instructions[selectedInstruction] = _current;
        }

        var typeList = typeof(AGPEvent).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(AGPEvent)))
            .ToList();
        var stringList = new List<string>();
        for (int i = 0; i < typeList.Count; i++)
        {
            stringList.Add(typeList[i].ToString());
        }
        if (_current.whenToShow == Options.EventBased)
        {
            eventIndex = EditorGUILayout.Popup("Event", eventIndex, stringList.ToArray());
        }
        
        if (GUILayout.Button("Add"))
        {
            Debug.Log("add loop");
            _current.textPosition = EditorGUILayout.Vector3Field("Position", _current.textPosition);
            _current.content = EditorGUILayout.TextField("Content", _current.content);
            Debug.Log("content: " + _current.content);
            _current.duration = EditorGUILayout.FloatField("Duration", _current.duration);
            _current.whenToShow = (Options) EditorGUILayout.EnumPopup("When to show", _current.whenToShow);
            var textObj = Instantiate(tmpPrefab, _current.textPosition, Quaternion.identity, container);
            var tmp = textObj.GetComponent<TextMeshPro>();
            tmp.text = _current.content;
            //textObj.name = _temp.content.Substring(0, Mathf.Min(10, _temp.content.Length));
            Debug.Assert(levelData != null);
            levelData.instructions.Add(_current);
            instructions.Add(_current);
            _textObjects.Add(textObj);
            _names.Add(textObj.name);
            selectedInstruction = instructions.Count - 1;
        }
        
        EditorGUILayout.Space();
        GUILayout.Label("Remove", EditorStyles.boldLabel);
        if (_textObjects.Count > 0)
        {
            toRemove = EditorGUILayout.Popup("To Remove", toRemove, _names.ToArray());
            if (GUILayout.Button("Remove"))
            {
                DestroyImmediate(_textObjects[toRemove]);
                _textObjects.RemoveAt(toRemove);
                Debug.Assert(levelData != null); 
                levelData.instructions.RemoveAt(toRemove);
                instructions.RemoveAt(toRemove);
                _names.RemoveAt(toRemove);
            }
        }
        EditorGUILayout.Space();
        
        GUILayout.Label("Edit", EditorStyles.boldLabel);
        if (GUILayout.Button("Edit"))
        {
            
        }
        
        Debug.Assert(container != null);
        if (container.childCount != _textObjects.Count)
        {
            EditorGUILayout.HelpBox("container: " + container.childCount + "; textObjects: " + _textObjects.Count, MessageType.Warning);
        }
        
        if (GUILayout.Button("Update"))
        {
            selectedInstruction = 0;
            _names = new List<string>();
            foreach (var textObject in _textObjects)
            {
                var text = textObject.GetComponent<TextMeshPro>().text;
                textObject.name = text.Substring(0, Mathf.Min(10, text.Length));
                _names.Add(textObject.name);
            }

            
            Debug.Assert(levelData != null); 
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
            Debug.Assert(levelData != null); 
            levelData.instructions.Clear();
            instructions.Clear();
            _names.Clear();
            selectedInstruction = 0;
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

    private void Update()
    {
        if (EditorApplication.isPlaying) return;
    }
}