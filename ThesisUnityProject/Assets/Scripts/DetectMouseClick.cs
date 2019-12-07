using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DetectMouseClick : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private LineRenderer _netForceLineRenderer;

    private bool _holdingMouse;

    private Camera _myCamera;

    public Material lineMaterial;
    
    public Material netForceMaterial;

    private List<GameObject> _lines;

    private GameObject _currentLine;

    private GameObject _netForceGameObject;

    private Vector3 _netForceVector;

    private Vector3 _startPos;
    
    public Color netForceColor = new Color(1,1,1,1);
    public Color forceColor = new Color(1,1,1,1);

    private Rigidbody2D _rb;

    public float forceMultiplier = 0.01f;

    public bool _squareStopped;

    private Vector2 _velocityBeforePause;
    
    // Start is called before the first frame update
    void Start()
    {
        _myCamera = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
        _lines = new List<GameObject>();
        
        lineMaterial.renderQueue = 3001;
        netForceMaterial.renderQueue = 3001;

        
        _netForceVector = new Vector3();
        
        SetUpNetForce();
    }

    private void SetUpNetForce()
    {
        _netForceGameObject = new GameObject();
        _netForceGameObject.transform.parent = transform;
        _netForceGameObject.transform.name = "netForce";

        _netForceLineRenderer = _netForceGameObject.AddComponent<LineRenderer>();
        _netForceLineRenderer.material = netForceMaterial;
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
            _lineRenderer.SetPosition(1,endPos);
        }

        UpdateNetForce();

        if (!_squareStopped)
        {
            _rb.AddForce(_netForceVector * forceMultiplier);
        }

        
    }

    private void UpdateNetForce()
    {
        _netForceVector = new Vector3();
        for (int i = 0; i < _lines.Count; i++)
        {
            var lineRenderer = _lines[i].GetComponent<LineRenderer>();
            var lineVector = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
            _netForceVector += lineVector;
        }

        _netForceVector.z = 0;
        _netForceLineRenderer.SetPosition(1, _startPos + _netForceVector);
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

        _lineRenderer = _currentLine.AddComponent<LineRenderer>();
        _lineRenderer.material = lineMaterial;
        _lineRenderer.startColor = forceColor;
        _lineRenderer.endColor = forceColor;
        _lineRenderer.widthMultiplier = 0.1f;

        _startPos = transform.position;
        _startPos.z = 1;
        _lineRenderer.SetPosition(0, _startPos);

        _lines.Add(_currentLine);

        _netForceLineRenderer.SetPosition(0, _startPos);
    }

    private void OnMouseUp()
    {
        _holdingMouse = false;
    }

    public void PauseMoving()
    {
        _squareStopped = true;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _velocityBeforePause = _rb.velocity;
        _rb.velocity = Vector2.zero;
    }

    public void ResumeMoving()
    {
        _squareStopped = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.velocity = _velocityBeforePause;
        _rb.AddForce(_netForceVector * forceMultiplier);

        var meshFilter = _netForceGameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        _netForceLineRenderer.BakeMesh(mesh);
        meshFilter.mesh = mesh;
        var meshRenderer = _netForceGameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = netForceMaterial;
        _netForceLineRenderer.enabled = false;
        
        _netForceGameObject.transform.localPosition = new Vector3(0, -transform.position.y, 0);
    }
}
