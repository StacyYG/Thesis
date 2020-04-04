using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Path : MonoBehaviour
{
    public int maxPoints = 16;
    public float dotSize = 15f;
    private VectorLine _pathLine;
    private float _waitTime = 0.1f;
    private Color _myColor;
    
    // Start is called before the first frame update
    void Start()
    {
        _pathLine = new VectorLine("Path", new List<Vector3>(), dotSize, LineType.Discrete);
        Services.TotalLineNumber++;
        _myColor = GetComponent<SpriteRenderer>().color;
        _pathLine.color = _myColor;
        _pathLine.endPointsUpdate = 2;
        _pathLine.drawDepth = 0;
        _pathLine.Draw();
        StartCoroutine(WaitAndSamplePointsNew());
    }

    private int _pointIndex = 0;
    private IEnumerator WaitAndSamplePointsNew()
    {
        yield return new WaitForSeconds(_waitTime);
        
        _pathLine.points3.Add(transform.position);
        _pointIndex++;

        if (_pointIndex % 2 == 0)
        {
            _pathLine.Draw();
            if (_pointIndex > maxPoints * 2)
            {
                _pathLine.drawStart += 2;
            }
            
        }
        
        StartCoroutine(WaitAndSamplePointsNew());
    }
    
}
