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
    public Texture2D frontTexture;
    public Texture2D dashedLineTexture;
    public Texture2D fullLineTexture;


    private void Awake()
    {
        VectorLine.SetEndCap("dashedArrow", EndCap.Front, -0.5f, dashedLineTexture, frontTexture);
        VectorLine.SetEndCap("fullArrow", EndCap.Front, -0.5f, fullLineTexture, frontTexture);
    }

    // Start is called before the first frame update
    void Start()
    {
        _myWorldPosition = transform.position;
        _currentNetForceLine = new VectorLine("currentNetForce", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        _previousNetForceLine = new VectorLine("previousNetForce", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        _currentLine = new VectorLine("forceBeingDrawn", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        _currentNetForceLine.drawTransform =
            _previousNetForceLine.drawTransform = _currentLine.drawTransform = transform;
        _currentNetForceLine.endCap = "fullArrow";
        _previousNetForceLine.endCap = _currentLine.endCap = "dashedArrow";
        _previousNetForceLine.textureScale = _currentLine.textureScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (holdingMouse)
        {
            _currentForceVector = MouseWorldPosition() - _myWorldPosition;
            _currentLine.points3[0] = _currentForceVector;
            _currentLine.Draw();
            _currentNetForceLine.points3[0] = _netForceVector + _currentForceVector;
            _currentNetForceLine.Draw();
        }

    }

    // figure out a better way to replace this when on mobile
    private void OnMouseDown()
    {
        holdingMouse = true;
        _currentLine.active = true;
        _previousNetForceLine.active = true;
        _previousNetForceLine.points3[0] = _currentNetForceLine.points3[0];
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
