using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

// The dotted trail after the target square
public class Path : MonoBehaviour
{
    public int maxPoints = 16;
    public float dotSize = 15f;
    private VectorLine _pathLine;
    private float _waitTime = 0.1f, _timer;
    private Color _myColor;
    private int _pointIndex;
    
    private void OnEnable()
    {
        SetUpNewPath();
    }

    public void SetUpNewPath()
    {
        _pathLine = new VectorLine("Path", new List<Vector3>(), dotSize * Screen.height / 1080f, LineType.Discrete);
        _myColor = GetComponent<SpriteRenderer>().color;
        _pathLine.color = _myColor;
        _pathLine.endPointsUpdate = 2;
    }

    private void OnDisable()
    {
        VectorLine.Destroy(ref _pathLine);
        _pointIndex = 0;
    }
    
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _waitTime)
        {
            _timer = 0f;
            _pathLine.points3.Add(transform.position);
            _pointIndex++;
            if (_pointIndex % 2 == 0)
            {
                _pathLine.Draw3D();
                if (_pointIndex > maxPoints * 2)
                    _pathLine.drawStart += 2;
            }
        }
    }
}
