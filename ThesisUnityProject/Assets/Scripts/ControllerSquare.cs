using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class ControllerSquare : MonoBehaviour
{
    private VectorLine _currentLine;
    private VectorLine _currentNetForceLine;
    private VectorLine _previousNetForceLine;
    private Vector2 _currentForceVector;
    private Vector2 _netForceVector;
    public bool holdingMouse;
    public float lineWidth = 6f;
    private Vector2 _myWorldPosition;



    // Start is called before the first frame update
    void Start()
    {
        _myWorldPosition = transform.position;
        _currentNetForceLine = new VectorLine("currentNetForce", new List<Vector3> {_myWorldPosition, _myWorldPosition}, lineWidth);
        _previousNetForceLine = new VectorLine("previousNetForce", new List<Vector3> {_myWorldPosition, _myWorldPosition}, lineWidth);
        _currentLine = new VectorLine("forceBeingDrawn", new List<Vector3> {_myWorldPosition, _myWorldPosition}, lineWidth);
    }

    // Update is called once per frame
    void Update()
    {
        if (holdingMouse)
        {
            _currentLine.points3[1] = MouseWorldPosition();
            _currentLine.Draw();
            _currentForceVector = MouseWorldPosition() - _myWorldPosition;
            _currentNetForceLine.points3[1] = _netForceVector + MouseWorldPosition();
            _currentNetForceLine.Draw();
        }

    }

    // figure out a better way to replace this when on mobile
    private void OnMouseDown()
    {
        holdingMouse = true;
        _currentLine.active = true;
        _previousNetForceLine.active = true;
        _previousNetForceLine.points3[1] = _currentNetForceLine.points3[1];
        _previousNetForceLine.Draw();
    }

    private void OnMouseUp()
    {
        holdingMouse = false;
        _netForceVector += _currentForceVector;
        _currentLine.active = false;
        _previousNetForceLine.active = false;

    }

    private Vector2 MouseWorldPosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -ServiceLocator.MyCamera.transform.position.z;
        return ServiceLocator.MyCamera.ScreenToWorldPoint(mousePos);
    }

    public Vector2 PlayerForce()
    {
        if (holdingMouse) return _netForceVector + _currentForceVector;
        return _netForceVector;
    }
}
