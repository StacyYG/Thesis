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
    private readonly GameObject _gameObject;

    public CancelButton(GameObject gameObject)
    {
        _gameObject = gameObject;
    }
    // Start is called before the first frame update
    public void Start()
    {
        _boundCircleRadius = _gameObject.GetComponent<CircleCollider2D>().radius;
        DrawBoundCircle();
    }

    
    public void DrawBoundCircle(int segments = 15)
    {
        if (_circleLine != null)
        {
            VectorLine.Destroy(ref _circleLine);
            Services.TotalLineNumber--;
        }
        
        _circleLine= new VectorLine("circle", new List<Vector3>(segments), 8f, LineType.Points);
        Services.TotalLineNumber++;
        _circleLine.MakeCircle(_gameObject.transform.position, _boundCircleRadius, segments);
        _circleLine.Draw();
    }
}
