using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class VelocityLine
{
    private VectorLine _line;
    private readonly Rigidbody2D _rb;
    private readonly Transform _targetTransform;
    private const float Adjuster = 0.3f;

    public VelocityLine(GameObject gameObject)
    {
        _line = new VectorLine("velocityLine", new List<Vector3> {Vector2.zero, Vector2.zero},
            Services.GameCfg.velocityLineWidth * Screen.height / 1080f);
        _line.texture = Services.GameCfg.fullLineTexture;
        _line.color = Services.GameCfg.velocityLineColor;
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _targetTransform = gameObject.transform;
        _line.drawTransform = _targetTransform;
    }

    public void Draw()
    {
        if (_line != null)
        {
            _line.points3[0] = _targetTransform.InverseTransformVector(Adjuster * _rb.velocity);
            _line.Draw();
        }
    }

    public void SetColor(Color color)
    {
        _line.color = color;
    }

    public void MultiplyWidth(float multiplier)
    {
        var currentWidth = _line.lineWidth;
        _line.lineWidth = multiplier * currentWidth;
    }

    public void Hide(bool isHide)
    {
        _line.active = !isHide;
    }

    public void Destroy()
    {
        VectorLine.Destroy(ref _line);
    }
}




