using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class ControlButton
{
    private VectorLine _currentLine;
    private Vector2 _currentForce, _myWorldPosition;
    private const float MaxForceSize = 1.5f;
    public Vector2 PlayerForce => 1f / _vectorMultiplier * _currentForce;
    private readonly Transform _transform;
    public BoundCircle boundCircle;
    private GameObject _gameObject;
    private readonly float _vectorMultiplier;
    
    public ControlButton(GameObject gameObject)
    {
        _gameObject = gameObject;
        _transform = gameObject.transform;
        gameObject.GetComponent<CircleCollider2D>().radius = MaxForceSize;
        _vectorMultiplier = Services.GameCfg.vectorMultiplier;
    }
    
    public void Init()
    {
        SetUpVectorLines();
        boundCircle = new BoundCircle(MaxForceSize, _gameObject.transform);
    }

    private void SetUpVectorLines() // Initialize for vector line utility
    {
        var realLineWidth = Services.GameCfg.forceLineWidth * Screen.height / 1080f;

        _currentLine = new VectorLine("forceBeingDrawn", new List<Vector3> {Vector2.zero, Vector2.zero}, realLineWidth);
        _currentLine.color = Services.GameCfg.currentForceColor;
        _currentLine.drawTransform = _transform;
        _currentLine.endCap = "fullArrow";
    }

    public void UpdatePlayerForce(Vector2 mouseWorldPos) // Ready the vector lines in background but not draw it yet
    {
        _currentForce = Vector2.ClampMagnitude(mouseWorldPos - (Vector2) _transform.position, MaxForceSize);
        _currentLine.points3[0] = _currentForce;
    }

    public void DrawForceLines() // Draw the vector lines in late update
    {
        _currentLine.Draw();
    }
    
    public void OnMouseOrTouchDown() // Show current line and previous net force line when player starts touching
    {
        _currentLine.active = true;
    }

    public void OnMouseOrTouchUp() // Only show the net force line when player is not touching
    {
        ResetPlayerForce();
        _currentLine.active = false;
    }

    public void ResetPlayerForce() // Set player force to zero and draw
    {
        _currentForce = Vector2.zero;
        _currentLine.points3[0] = Vector2.zero;
        _currentLine.Draw();
    }
}

public static class Arrow // Set up the Arrow looks in vector line
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
