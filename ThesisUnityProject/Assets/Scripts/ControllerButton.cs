using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class ControllerButton
{
    private VectorLine _currentLine, _netForceLine, _previousNetForceLine, _circleLine;
    private Vector2 _currentForce, _sum, _netForce, _myWorldPosition;
    private const float MaxForceSize = 1.5f;
    public bool respond = true;
    public Vector2 PlayerForce => 1f / _vectorMultiplier * _netForce;
    private readonly Transform _transform;
    public BoundCircle boundCircle;
    private GameObject _gameObject;
    private readonly float _vectorMultiplier;
    public ControllerButton(GameObject gameObject)
    {
        _gameObject = gameObject;
        _transform = gameObject.transform;
        gameObject.GetComponent<CircleCollider2D>().radius = MaxForceSize;
        _vectorMultiplier = Services.GameCfg.vectorMultiplier;
    }

    // Start is called before the first frame update
    public void Start()
    {
        boundCircle = new BoundCircle(MaxForceSize, _gameObject.transform);
        SetUpVectorLines();
    }

    private void SetUpVectorLines()
    {
        var realLineWidth = Services.GameCfg.forceLineWidth * Screen.height / 1080f;
        _netForceLine = new VectorLine("currentNetForce", new List<Vector3> {Vector2.zero, Vector2.zero}, realLineWidth);
        _netForceLine.color = Services.GameCfg.currentNetForceColor;

        _previousNetForceLine =
            new VectorLine("previousNetForce", new List<Vector3> {Vector2.zero, Vector2.zero}, realLineWidth);
        _previousNetForceLine.color = Services.GameCfg.previousNetForceColor;

        _currentLine = new VectorLine("forceBeingDrawn", new List<Vector3> {Vector2.zero, Vector2.zero}, realLineWidth);
        _currentLine.color = Services.GameCfg.currentForceColor;

        _netForceLine.drawTransform =
            _previousNetForceLine.drawTransform = _currentLine.drawTransform = _transform;
        _netForceLine.endCap = "fullArrow";
        _previousNetForceLine.endCap = _currentLine.endCap = "dashedArrow";
        _previousNetForceLine.textureScale = _currentLine.textureScale = 1.0f;
    }

    public void UpdateCurrentPlayerForce(Vector2 mouseWorldPos)
    {
        if(!respond) return;
        _currentForce =
            Vector2.ClampMagnitude(mouseWorldPos - (Vector2) _transform.position,
                MaxForceSize);
        _currentLine.points3[0] = _currentForce;
        _netForce = _sum + _currentForce;
        _netForceLine.points3[0] = _netForce;
    }

    public void LateUpdate()
    {
        _previousNetForceLine.Draw();
        _currentLine.Draw();
        _netForceLine.Draw();
    }
    
    public void OnMouseOrTouchDown()
    {
        if (!respond) return;
        _currentLine.active = true;
        _previousNetForceLine.active = true;
    }

    public void OnMouseOrTouchUp()
    {
        if(!respond) return;
        _sum = _netForce;
        _currentLine.active = false;
        _previousNetForceLine.active = false;
        _previousNetForceLine.points3[0] = _sum;
    }

    public void ResetPlayerForce()
    {
        _sum = _netForce = _currentForce = Vector2.zero;
        _currentLine.points3[0] = _netForceLine.points3[0] = _previousNetForceLine.points3[0] = Vector2.zero;
        _previousNetForceLine.Draw();
        _currentLine.Draw();
        _netForceLine.Draw();
    }
}

public static class Arrow
{
    private static bool _isSet;
    public static void SetUp()
    {
        if(_isSet)
            return;
        VectorLine.SetEndCap("dashedArrow", EndCap.Front, -0.5f, Services.GameCfg.dashedLineTexture, Services.GameCfg.frontTexture);
        VectorLine.SetEndCap("fullArrow", EndCap.Front, -0.5f, Services.GameCfg.fullLineTexture, Services.GameCfg.frontTexture);
        _isSet = true;
    }
}
