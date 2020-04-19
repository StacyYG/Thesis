using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class CancelButton
{
    private float _boundCircleRadius;
    private VectorLine _circleLine;
    public bool Respond = true;
    private const int Segments = 15;
    public BoundCircle boundCircle;

    public CancelButton(GameObject gameObject)
    {
        _boundCircleRadius = gameObject.GetComponent<CircleCollider2D>().radius;
        boundCircle = new BoundCircle(_boundCircleRadius, Segments, gameObject.transform);
    }
    
}

public class BoundCircle
{
    private VectorLine _circle;
    private int _segments;
    private Transform _anchor;
    private float _radius;
    private float _growDuration;
    public BoundCircle(float radius, int segments, Transform anchor, float growDuration = 1f)
    {
        _radius = radius;
        _segments = segments;
        _anchor = anchor;
        _circle = new VectorLine("circle", new List<Vector3>(segments), 8f, LineType.Points);
        _growDuration = growDuration;
        Services.TotalLineNumber++;
    }
    public bool GrownUp(float growTime)
    {
        if (growTime > _growDuration) return true;
        _circle.MakeCircle(_anchor.position, growTime / _growDuration * _radius, _segments);
        _circle.Draw();
        return false;
    }
    
}
