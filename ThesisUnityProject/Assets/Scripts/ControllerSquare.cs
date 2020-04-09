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
    private bool _holdingMouse;
    public float lineWidth = 6f;
    private Vector2 _myWorldPosition;
    public Texture2D frontTexture;
    public Texture2D dashedLineTexture;
    public Texture2D fullLineTexture;
    private float _maxForceSize = 2f;
    public Color currentForceColor;
    public Color previousNetForceColor;
    public Color currentNetForceColor;
    private VectorLine _circleLine;

    private void Awake()
    {
        VectorLine.SetEndCap("dashedArrow", EndCap.Front, -0.5f, dashedLineTexture, frontTexture);
        VectorLine.SetEndCap("fullArrow", EndCap.Front, -0.5f, fullLineTexture, frontTexture);
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetUpVectorLines();
        DrawBoundCircle();
    }

    private void SetUpVectorLines()
    {
        _currentNetForceLine = new VectorLine("currentNetForce", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        Services.TotalLineNumber++;
        _currentNetForceLine.color = currentNetForceColor;

        _previousNetForceLine =
            new VectorLine("previousNetForce", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        Services.TotalLineNumber++;
        _previousNetForceLine.color = previousNetForceColor;

        _currentLine = new VectorLine("forceBeingDrawn", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        Services.TotalLineNumber++;
        _currentLine.color = currentForceColor;

        _currentNetForceLine.drawTransform =
            _previousNetForceLine.drawTransform = _currentLine.drawTransform = transform;
        _currentNetForceLine.endCap = "fullArrow";
        _previousNetForceLine.endCap = _currentLine.endCap = "dashedArrow";
        _previousNetForceLine.textureScale = _currentLine.textureScale = 1.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_holdingMouse)
        {
            _currentForceVector = Vector2.ClampMagnitude(MouseWorldPosition() - (Vector2)transform.position, _maxForceSize);
            _currentLine.points3[0] = _currentForceVector;
            _currentNetForceLine.points3[0] = _netForceVector + _currentForceVector;
        }

    }

    private void LateUpdate()
    {
        _currentLine.Draw();
        _currentNetForceLine.Draw();
    }

    // figure out a better way to replace this when on mobile
    private void OnMouseDown()
    {
        _holdingMouse = true;
        _currentLine.active = true;
        _previousNetForceLine.active = true;
        _previousNetForceLine.points3[0] = _currentNetForceLine.points3[0];
        _previousNetForceLine.Draw();
    }

    private void OnMouseUp()
    {
        _holdingMouse = false;
        _netForceVector += _currentForceVector;
        _currentLine.active = false;
        _previousNetForceLine.active = false;

    }
    
    private Vector2 MouseWorldPosition()
    {
        var mousePos = Input.mousePosition;
        var mouseWorldPos = Services.MyCamera.ScreenToWorldPoint(mousePos);
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }

    private Vector2 _playerForce;

    public Vector2 PlayerForce
    {
        get
        {
            if (_holdingMouse)
            {
                _playerForce = _netForceVector + _currentForceVector;
                
            }
            else
            {
                _playerForce = _netForceVector;
            }

            return _playerForce;
        }
        set => _playerForce = value;
    }

    public void ResetPlayerForce()
    {
        _netForceVector = Vector2.zero;
        _currentForceVector = Vector2.zero;
        _currentLine.points3[0] = Vector2.zero;
        _currentNetForceLine.points3[0] = Vector2.zero;
        _currentLine.Draw();
        _currentNetForceLine.Draw();
    }
    public void DrawBoundCircle(int segments = 30)
    {
        if (_circleLine != null)
        {
            VectorLine.Destroy(ref _circleLine);
            Services.TotalLineNumber--;
        }
        _circleLine = new VectorLine("circle", new List<Vector3>(segments), 8f, LineType.Points);
        Services.TotalLineNumber++;
        _circleLine.MakeCircle(transform.position, _maxForceSize, segments);
        _circleLine.Draw();
    }
}
