using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Path : MonoBehaviour
{
    public int maxPoints = 15;
    public float dotSize = 15f;
    private VectorLine _pathLine;

    // Start is called before the first frame update
    void Start()
    {
        _pathLine = new VectorLine("Path", new List<Vector3>(maxPoints), dotSize, LineType.Points);
        var myColor = GetComponent<SpriteRenderer>().color;
        _pathLine.SetColor(myColor);
        StartCoroutine(WaitAndSamplePoints(transform));
    }

    private IEnumerator WaitAndSamplePoints(Transform thisTransform)
    {
        for (int i = 0; i < maxPoints; i++)
        {
            yield return new WaitForSeconds(0.2f);
            _pathLine.points3[i] = thisTransform.position;
            _pathLine.Draw();
            if (i == maxPoints - 1)
            {
                i = -1;
            }
        }
        
    }
    
}
