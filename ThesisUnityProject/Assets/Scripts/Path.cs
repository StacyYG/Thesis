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
        _myColor = GetComponent<SpriteRenderer>().color;
        _pathLine.color = _myColor;
        StartCoroutine(WaitAndSamplePointsNew());
    }

    private IEnumerator WaitAndSamplePointsNew()
    {
        yield return new WaitForSeconds(_waitTime);

        _pathLine.points3.Add(transform.position);
        if (_pathLine.points3.Count >= maxPoints)
        {
            _pathLine.points3.RemoveAt(0);
        }

        _pathLine.Draw();
        
        StartCoroutine(WaitAndSamplePointsNew());
    }
    
}
