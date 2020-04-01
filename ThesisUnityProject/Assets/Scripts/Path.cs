using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Path : MonoBehaviour
{
    public int maxPoints = 16;
    public float dotSize = 15f;
    private VectorLine _pathLine;
    private Vector3[] _history;
    private float _waitTime = 0.1f;
    private Color myColor;

    

    // Start is called before the first frame update
    void Start()
    {
        _history = new Vector3[maxPoints];
        _pathLine = new VectorLine("Path", new List<Vector3>(), dotSize, LineType.Discrete);
        myColor = GetComponent<SpriteRenderer>().color;
        StartCoroutine(DrawPathFirstRound());
    }

    private IEnumerator DrawPathFirstRound()
    {
        for (int i = 0; i < maxPoints; i++)
        {
            yield return new WaitForSeconds(_waitTime);
            var pos = transform.position;
            _pathLine.points3.Add(pos);
            _history[i] = pos;
            _pathLine.SetColor(myColor);
            _pathLine.Draw();
        }
        StartCoroutine(WaitAndSamplePoints());
    }
    private IEnumerator WaitAndSamplePoints()
    {
        yield return new WaitForSeconds(_waitTime);
        _pathLine.points3[maxPoints - 1] = transform.position;
        for (int i = 0; i < maxPoints; i++)
        {
            _history[i] = _pathLine.points3[i];
            if (i < maxPoints - 1)
            {
                _pathLine.points3[i] = _history[i + 1];
            }

        }
        
        _pathLine.Draw();
        
        StartCoroutine(WaitAndSamplePoints());
    }
    
}
