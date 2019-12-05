using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    public bool _holdingMouse;

    private Camera _myCamera;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _myCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _holdingMouse = true;

            var mousePos = Input.mousePosition;
            mousePos.z = 1;
            var startPos = _myCamera.ScreenToWorldPoint(mousePos);
            _lineRenderer.SetPosition(0,startPos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _holdingMouse = false;
        }
        
        if (_holdingMouse)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 1;
            var endPos = _myCamera.ScreenToWorldPoint(mousePos);
            _lineRenderer.SetPosition(1,endPos);
        }


    }
}
