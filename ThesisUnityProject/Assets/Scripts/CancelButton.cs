using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        boundCircle = new BoundCircle(_boundCircleRadius, _gameObject.transform);
    }
}

public class GravityButton
{
    public BoundCircle boundCircle;
    private GameObject _triangle;
    private List<Rigidbody2D> _rbs;
    private float _boundCircleRadius;
    private const int Segments = 15;
    public bool GravityOn { get; private set; }

    public GravityButton(GameObject circleObj, GameObject triangleObj)
    {
        _boundCircleRadius = circleObj.GetComponent<CircleCollider2D>().radius;
        _triangle = triangleObj;
    }

    public void Start()
    {
        boundCircle = new BoundCircle(_boundCircleRadius, _triangle.transform);
        _rbs = GameObject.FindObjectsOfType<Rigidbody2D>().ToList();
    }

    public void GravitySwitch()
    {
        if (GravityOn)
        {
            GravityOn = false;
            _triangle.transform.localScale = new Vector3(1f, 0f, 1f);
        }
        else
        {
            GravityOn = true;
            _triangle.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    
    public void UpdateRbs()
    {
        _rbs = GameObject.FindObjectsOfType<Rigidbody2D>().ToList();
    }

    public void UpdateGravity()
    {
        for (int i = 0; i < _rbs.Count; i++)
        {
            if (ReferenceEquals(_rbs[i], null))
            {
                _rbs.Remove(_rbs[i]);
                return;
            }
            _rbs[i].gravityScale = GravityOn ? 1f : 0f;
        }
    }
}

public class BoundCircle
{
    private VectorLine _circle;
    public DelegateTask GrowUp;
    private float _elapsedTime;
    public BoundCircle(float radius, Transform anchor, float duration = 1f)
    {
        var segmentNum = (int) (Services.GameCfg.segmentsPerRadius * radius);
        _circle = new VectorLine("circle", new List<Vector3>(segmentNum), 8f * Screen.height / 1080f, LineType.Points);
        _circle.color = Services.GameCfg.boundCircleColor;
        GrowUp = new DelegateTask(() => {}, () =>
        {
            _elapsedTime += Time.deltaTime;
            _circle.MakeCircle(anchor.position, _elapsedTime / duration * radius, segmentNum);
            _circle.Draw();
            return _elapsedTime >= duration;
        });
    }

    public void Clear()
    {
        VectorLine.Destroy(ref _circle);
    }
}
