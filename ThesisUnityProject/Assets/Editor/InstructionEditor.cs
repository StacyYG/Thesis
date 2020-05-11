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
    private List<string> _names = new List<string>();
    public List<InstructionData> instructions;
    public int selectedIndex = 0;
    private InstructionData _selected = new InstructionData();
    private InstructionData _new = new InstructionData();
    private List<string> _eventNames = new List<string>();
    private List<Type> _events = new List<Type>();
    //public IEnumerable<MyEnum> myEvents;

    [MenuItem("Tools/Instruction Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(InstructionEditor));
    }

    private void Awake()
    {
        GetEvents();
    }

    private void OnEnable()
    {
        GetEvents();
    }

    private void OnGUI()
    {
        if (EditorApplication.isPlaying) return;
        if (GUILayout.Button("Get Events"))
        {
            GetEvents();
        }
        
        container = EditorGUILayout.ObjectField("Container", container, typeof(Transform), true) as Transform;
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
        
        if (_new.whenToShow == Options.EventBased)
        {
            _new.eventIndex = EditorGUILayout.Popup("Event", _new.eventIndex, _eventNames.ToArray());
            _new.myEvent = _events[_new.eventIndex];
            _new.eventName = _new.myEvent.ToString();
            //_new.eventEnum = (MyEnum)EditorGUILayout.EnumPopup("My Events", _new.eventEnum);
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
            selectedIndex = instructions.Count - 1;
        }
        EditorGUILayout.Space();
        GUILayout.Label("Edit", EditorStyles.boldLabel);
        selectedIndex = EditorGUILayout.Popup("Selected", selectedIndex, _names.ToArray());
        if (instructions.Count != 0)
        {
            _selected = instructions[selectedIndex];
            _selected.textPosition = EditorGUILayout.Vector3Field("Position", _selected.textPosition);
            _selected.content = EditorGUILayout.TextField("Content", _selected.content);
            _selected.duration = EditorGUILayout.FloatField("Duration", _selected.duration);
            _selected.whenToShow = (Options) EditorGUILayout.EnumPopup("When to show", _selected.whenToShow);
            if (_selected.whenToShow == Options.EventBased)
            {
                _selected.eventIndex = EditorGUILayout.Popup("Event", _selected.eventIndex, _eventNames.ToArray());
                _selected.myEvent = _events[_selected.eventIndex];
                _selected.eventName = _selected.myEvent.ToString();
                //_selected.eventEnum = (MyEnum)EditorGUILayout.EnumPopup("My Events", _selected.eventEnum);
            }
            instructions[selectedIndex] = _selected;
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
                
                levelData.instructions[i] = instructions[i];
            }
        }
        EditorGUILayout.Space();
        if (_textObjects.Count > 0)
        {
            if (GUILayout.Button("Remove"))
            {
                DestroyImmediate(_textObjects[selectedIndex]);
                _textObjects.RemoveAt(selectedIndex);
                levelData.instructions.RemoveAt(selectedIndex);
                instructions.RemoveAt(selectedIndex);
                _names.RemoveAt(selectedIndex);
                selectedIndex = selectedIndex == 0 ? 0 : selectedIndex - 1;
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
            selectedIndex = 0;
        }

    }

    private void GetEvents()
    {
        _events = new List<Type>();
        _eventNames = new List<string>();
        _events = typeof(AGPEvent).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(AGPEvent)))
            .ToList();
        for (int i = 0; i < _events.Count; i++)
            _eventNames.Add(_events[i].ToString());
        //myEvents = _eventNames.ToArray().Select(a => (MyEnum) Enum.Parse(typeof(MyEnum), a));
    }

    private void Update()
    {
        if (EditorApplication.isPlaying) return;
        if(instructions.Count == 0 || _textObjects.Count == 0) return;
        for (int i = 0; i < instructions.Count; i++)
        {
            _textObjects[i].transform.position = instructions[i].textPosition;
        }
    }
}

