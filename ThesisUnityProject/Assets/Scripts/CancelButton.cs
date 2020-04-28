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
    private GameObject _gameObject;

    public CancelButton(GameObject gameObject)
    {
        _gameObject = gameObject;
        _boundCircleRadius = gameObject.GetComponent<CircleCollider2D>().radius;
    }

    public void Start()
    {
        boundCircle = new BoundCircle(_boundCircleRadius, Segments, _gameObject.transform);
    }
    
}

public class BoundCircle
{
    private VectorLine _circle;
    public DelegateTask GrowUp;
    private float _elapsedTime;
    public BoundCircle(float radius, int segments, Transform anchor, float duration = 1f)
    {
        _circle = new VectorLine("circle", new List<Vector3>(segments), 8f * Screen.height / 1080f, LineType.Points);
        GrowUp = new DelegateTask(() => {}, () =>
        {
            _elapsedTime += Time.deltaTime;
            _circle.MakeCircle(anchor.position, _elapsedTime / duration * radius, segments);
            _circle.Draw();
            return _elapsedTime >= duration;
        });
    }

    public void Clear()
    {
        VectorLine.Destroy(ref _circle);
    }
}
