using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBoxManager : MonoBehaviour
{
    private Rigidbody2D _targetRB;
    private GameObject _controlSqr, _targetSqr;
    public GameCfg gameCfg;
    private bool _controlButtonGrowing = true;
    private float _controlButtonGrowTimer;

    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Services.Input.Update();
        _targetRB.AddForce(Services.ControllerSquare.PlayerForce);
        foreach (var force in Services.Forces)
            force.Update();
        Services.CameraController.Update();
    }
    // Update is called once per frame
    void Update()
    {
        if (_controlButtonGrowing)
        {
            _controlButtonGrowTimer += Time.deltaTime;
            if (Services.ControllerSquare.boundCircle.GrownUp(_controlButtonGrowTimer)) 
                _controlButtonGrowing = false;
        }
    }

    private void LateUpdate()
    {
        foreach (var force in Services.Forces)
            force.Draw();
        
        Services.ControllerSquare.LateUpdate();
    }
    private void Init()
    {
        Services.GameCfg = gameCfg;
        Arrow.SetUp();
        Services.MainCamera = Camera.main;
        Services.Input = new InputManager();
        _controlSqr = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = new ControllerSquare(_controlSqr.transform);
        _targetSqr = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = _targetSqr.GetComponent<TargetSquare>();
        _targetRB = _targetSqr.GetComponent<Rigidbody2D>();
        Services.CameraController = new CameraController(Services.MainCamera, false, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
    }

    private void OnDestroy()
    {

        
    }
}
