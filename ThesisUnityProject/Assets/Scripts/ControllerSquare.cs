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
    private Vector2 _currentForce, _sum, _netForce;
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
    public bool respond = true;

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

    public void UpdateCurrentPlayerForce(Vector2 mouseWorldPos)
    {
        _currentForce =
            Vector2.ClampMagnitude(mouseWorldPos - (Vector2) transform.position,
                _maxForceSize);
        _currentLine.points3[0] = _currentForce;
        _netForce = _sum + _currentForce;
        _currentNetForceLine.points3[0] = _netForce;
    }

    private void LateUpdate()
    {
        _currentLine.Draw();
        _currentNetForceLine.Draw();
    }

    // figure out a better way to replace this when on mobile
    public void OnMouseOrTouchDown()
    {
        if (!respond) return;
        
        _currentLine.active = true;
        _previousNetForceLine.active = true;
        _previousNetForceLine.Draw();
    }

    public void OnMouseOrTouchUp()
    {
        if(!respond) return;

        _sum = _netForce;
        _currentLine.active = false;
        _previousNetForceLine.active = false;
        _previousNetForceLine.points3[0] = _netForce;
    }
    
    private Vector2 MouseWorldPosition()
    {
        var mouseWorldPos = Services.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }

    public Vector2 PlayerForce => _netForce;

    public void ResetPlayerForce()
    {
        _sum = Vector2.zero;
        _currentForce = Vector2.zero;
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
