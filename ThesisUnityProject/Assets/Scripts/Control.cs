using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Control : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private bool _holdingMouse;

    private Camera _myCamera;

    public Material lineMaterial;

    public List<GameObject> lines;

    private GameObject _currentLine;
    // Start is called before the first frame update
    void Start()
    {
        _myCamera = Camera.main;
        lines = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _holdingMouse = true;

            _currentLine = new GameObject();
            _currentLine.transform.name = "line" + lines.Count;
            _lineRenderer = _currentLine.AddComponent<LineRenderer>();
            _lineRenderer.material = lineMaterial;
            _lineRenderer.widthMultiplier = 0.1f;
            
            var mousePos = Input.mousePosition;
            mousePos.z = 11;
            var startPos = _myCamera.ScreenToWorldPoint(mousePos);
            _lineRenderer.SetPosition(0,startPos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _holdingMouse = false;
            lines.Add(_currentLine);
        }
        
        if (_holdingMouse)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 11;
            var endPos = _myCamera.ScreenToWorldPoint(mousePos);
            _lineRenderer.SetPosition(1,endPos);
        }


    }
}
