using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBoxManager : MonoBehaviour
{
    private Rigidbody2D _targetRB;
    private GameObject _controlSqr;
    private GameObject _targetSqr;

    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        Services.MainCamera = Camera.main;
        _controlSqr = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = _controlSqr.GetComponent<ControllerSquare>();
        _targetSqr = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = _targetSqr.GetComponent<TargetSquare>();
        _targetRB = _targetSqr.GetComponent<Rigidbody2D>();
        Services.CameraController = new CameraController(Services.MainCamera, false, Services.TargetSquare.transform);
    }
}
