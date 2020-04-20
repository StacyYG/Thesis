using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class ControllerSquare
{
    private VectorLine _currentLine, _netForceLine, _previousNetForceLine, _circleLine;
    private Vector2 _currentForce, _sum, _netForce, _myWorldPosition;
    private const float MaxForceSize = 2f;
    public bool Respond = true;
    public Vector2 PlayerForce => _netForce;
    private readonly Transform _transform;
    public BoundCircle boundCircle;

    public ControllerSquare(Transform transform)
    {
        _transform = transform;
        boundCircle = new BoundCircle(MaxForceSize, 30, transform);
        SetUpVectorLines();
    }
    public void Awake()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    private void SetUpVectorLines()
    {
        _netForceLine = new VectorLine("currentNetForce", new List<Vector3> {Vector2.zero, Vector2.zero}, Services.GameCfg.lineWidth);
        Services.TotalLineNumber++;
        _netForceLine.color = Services.GameCfg.currentNetForceColor;

        _previousNetForceLine =
            new VectorLine("previousNetForce", new List<Vector3> {Vector2.zero, Vector2.zero}, Services.GameCfg.lineWidth);
        Services.TotalLineNumber++;
        _previousNetForceLine.color = Services.GameCfg.previousNetForceColor;

        _currentLine = new VectorLine("forceBeingDrawn", new List<Vector3> {Vector2.zero, Vector2.zero}, Services.GameCfg.lineWidth);
        Services.TotalLineNumber++;
        _currentLine.color = Services.GameCfg.currentForceColor;

        _netForceLine.drawTransform =
            _previousNetForceLine.drawTransform = _currentLine.drawTransform = _transform;
        _netForceLine.endCap = "fullArrow";
        _previousNetForceLine.endCap = _currentLine.endCap = "dashedArrow";
        _previousNetForceLine.textureScale = _currentLine.textureScale = 1.0f;
    }

    public void UpdateCurrentPlayerForce(Vector2 mouseWorldPos)
    {
        if(!Respond) return;
        _currentForce =
            Vector2.ClampMagnitude(mouseWorldPos - (Vector2) _transform.position,
                MaxForceSize);
        _currentLine.points3[0] = _currentForce;
        _netForce = _sum + _currentForce;
        _netForceLine.points3[0] = _netForce;
    }

    public void LateUpdate()
    {
        _currentLine.Draw();
        _netForceLine.Draw();
        _previousNetForceLine.Draw();
    }

    // figure out a better way to replace this when on mobile
    public void OnMouseOrTouchDown()
    {
        if (!Respond) return;
        _currentLine.active = true;
        _previousNetForceLine.active = true;
    }

    public void OnMouseOrTouchUp()
    {
        if(!Respond) return;
        _sum = _netForce;
        _currentLine.active = false;
        _previousNetForceLine.active = false;
        _previousNetForceLine.points3[0] = _netForce;
    }

    public void ResetPlayerForce()
    {
        _sum = _netForce = _currentForce = Vector2.zero;
        _currentLine.points3[0] = _netForceLine.points3[0] = _previousNetForceLine.points3[0] = Vector2.zero;
    }
}

public static class Arrow
{
    public static void SetUp()
    {
        VectorLine.SetEndCap("dashedArrow", EndCap.Front, -0.5f, Services.GameCfg.dashedLineTexture, Services.GameCfg.frontTexture);
        VectorLine.SetEndCap("fullArrow", EndCap.Front, -0.5f, Services.GameCfg.fullLineTexture, Services.GameCfg.frontTexture);
    }
}
