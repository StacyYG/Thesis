using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DetectMouseClick : MonoBehaviour
{
    private LineRenderer _currentLineRenderer;
    private LineRenderer _netForceLineRenderer;
    private MeshRenderer _netForceMeshRenderer;

    private bool _holdingMouse;

    private Camera _myCamera;

    public Material lineMaterial;

    private List<GameObject> _lines;

    private GameObject _currentLine;

    private GameObject _netForceGameObject;

    private Vector3 _netForceVector;

    private Vector3 _startPos;
    
    public Color netForceColor = new Color(1,1,1,1);
    public Color forceColor = new Color(1,1,1,1);

    private Rigidbody2D _rb;

    public float forceMultiplier = 0.01f;

    private bool _squareStopped;

    private Vector2 _velocityBeforePause;
    
    // Start is called before the first frame update
    void Start()
    {
        _myCamera = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
        _lines = new List<GameObject>();
        
        lineMaterial.renderQueue = 3001;

        _netForceVector = new Vector3();
        
        SetUpNetForce();
    }

    private void SetUpNetForce()
    {
        _netForceGameObject = new GameObject();
        _netForceGameObject.transform.parent = transform;
        _netForceGameObject.transform.name = "netForce";

        _netForceLineRenderer = _netForceGameObject.AddComponent<LineRenderer>();
        _netForceLineRenderer.material = lineMaterial;
        _netForceLineRenderer.startColor = Color.grey;
        _netForceLineRenderer.endColor = Color.grey;
        _netForceLineRenderer.widthMultiplier = 0.1f;
        
        var startPos = transform.position;
        startPos.z = 1;
        _netForceLineRenderer.SetPosition(0, startPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (_holdingMouse)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 11;
            var endPos = _myCamera.ScreenToWorldPoint(mousePos);
            _currentLineRenderer.SetPosition(1,endPos);
            UpdateNetForce(endPos);
        }
        
        if (!_squareStopped)
        {
            _rb.AddForce(_netForceVector * forceMultiplier);
        }

        
    }

    private void UpdateNetForce(Vector3 endPos)
    {
        var netFPreview = new Vector3();
        netFPreview = _netForceVector + endPos - _currentLineRenderer.GetPosition(0);
        netFPreview.z = 0;
        if (!_netForceLineRenderer.enabled) _netForceLineRenderer.enabled = true;
        _netForceLineRenderer.SetPosition(1, _startPos + netFPreview);
        if (!_netForceMeshRenderer || !_netForceMeshRenderer.enabled) return; _netForceMeshRenderer.enabled = false;
    }

    private void OnMouseDown()
    {
        if (!_squareStopped)
        {
            PauseMoving();
            return;
        }

        CreateNewForce();
    }

    private void CreateNewForce()
    {
        _holdingMouse = true;

        _currentLine = new GameObject();
        _currentLine.transform.parent = transform;
        _currentLine.transform.name = "line" + _lines.Count;

        _currentLineRenderer = _currentLine.AddComponent<LineRenderer>();
        _currentLineRenderer.material = lineMaterial;
        _currentLineRenderer.startColor = forceColor;
        _currentLineRenderer.endColor = forceColor;
        _currentLineRenderer.widthMultiplier = 0.1f;

        _startPos = transform.position;
        _startPos.z = 1;
        _currentLineRenderer.SetPosition(0, _startPos);

        _lines.Add(_currentLine);

        _netForceLineRenderer.SetPosition(0, _startPos);
    }

    private void OnMouseUp()
    {
        if (!_holdingMouse) return;

        _holdingMouse = false;
        
        var lineRenderer = _currentLine.GetComponent<LineRenderer>();
        var lineVector = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
        _netForceVector += lineVector;
        BakeMesh(_currentLine);
    }

    public void PauseMoving()
    {
        _squareStopped = true;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _velocityBeforePause = _rb.velocity;
        _rb.velocity = Vector2.zero;

        foreach (var line in _lines)
        {
            line.SetActive(true);
        }
    }

    public void ResumeMoving()
    {
        //resume rb movement and apply force
        _squareStopped = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.velocity = _velocityBeforePause;
        _rb.AddForce(_netForceVector * forceMultiplier);

        BakeMesh(_netForceGameObject);
        _netForceMeshRenderer = _netForceGameObject.GetComponent<MeshRenderer>();
        _netForceMeshRenderer.enabled = true;

        foreach (var line in _lines)
        {
            line.SetActive(false);
        }
    }

    private void BakeMesh(GameObject lineGameObject)
    {
        var lineRenderer = lineGameObject.GetComponent<LineRenderer>();
        MeshFilter meshFilter;
        if (lineGameObject.GetComponent<MeshFilter>())
        {
            meshFilter = lineGameObject.GetComponent<MeshFilter>();
        }
        else meshFilter = lineGameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer;
        if (lineGameObject.GetComponent<MeshRenderer>())
        {
            meshRenderer = lineGameObject.GetComponent<MeshRenderer>();
        }
        else meshRenderer = lineGameObject.AddComponent<MeshRenderer>();

        if (meshRenderer.material != lineMaterial) meshRenderer.material = lineMaterial;
        lineRenderer.enabled = false;

        lineGameObject.transform.localPosition = new Vector3(-transform.position.x, -transform.position.y, 0);
    }
}
