using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class CancelForce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var r = GetComponent<CircleCollider2D>().radius;
        DrawBoundCircle(transform.position, r);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Services.ControllerSquare.ResetPlayerForce();
    }
    
    public void DrawBoundCircle(Vector3 origin, float radius, int segments = 15)
    {
        var circleLine = new VectorLine("circle", new List<Vector3>(segments), 8f, LineType.Points);
        Services.TotalLineNumber++;
        circleLine.MakeCircle(origin, radius, segments);
        circleLine.Draw();
    }
}
