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
    private const int Segments = 20;
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

public class GravityButton
{
    public BoundCircle boundCircle;
    private GameObject _triangle;
    private Rigidbody2D[] _rbs;
    private float _boundCircleRadius;
    private const int Segments = 10;
    public bool GravityOn { get; private set; }

    public GravityButton(GameObject triangleObj)
    {
        _triangle = triangleObj;
        _boundCircleRadius = triangleObj.GetComponent<CircleCollider2D>().radius;
    }

    public void Start()
    {
        boundCircle = new BoundCircle(_boundCircleRadius, Segments, _triangle.transform);
        _rbs = GameObject.FindObjectsOfType<Rigidbody2D>();
    }

    public void GravitySwitch()
    {
        if (GravityOn)
        {
            GravityOn = false;
            for (int i = 0; i < _rbs.Length; i++)
                _rbs[i].gravityScale = 0f;
            _triangle.transform.localScale = new Vector3(1f, 0f, 1f);
            
        }
        else
        {
            GravityOn = true;
            for (int i = 0; i < _rbs.Length; i++)
                _rbs[i].gravityScale = 1f;
            _triangle.transform.localScale = new Vector3(1f, 1f, 1f);
        }
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
