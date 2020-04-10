using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class CancelButton : MonoBehaviour
{
    private float _boundCircleRadius;
    private VectorLine _circleLine;
    public bool respond = true;
    
    // Start is called before the first frame update
    void Start()
    {
        _boundCircleRadius = GetComponent<CircleCollider2D>().radius;
        DrawBoundCircle();
    }

    // Update is called once per frame
    void Update()
    {
        if (!respond) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Services.ControllerSquare.ResetPlayerForce();
        }
    }

    private void OnMouseDown()
    {
        if (!respond) return;
        
        Services.ControllerSquare.ResetPlayerForce();
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
        _circleLine.MakeCircle(transform.position, _boundCircleRadius, segments);
        _circleLine.Draw();
    }
}
