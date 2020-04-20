using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager2 : MonoBehaviour
{
    public LevelCfg1 levelCfg1;
    private TextMeshPro _tmp;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj, _targetSqrObj, _cancelButtonObj;
    private int _lastIndex;
    public GameCfg gameCfg;
    
    public void Awake()
    {
        Init();
        Services.ControllerSquare.Awake();
    }

    private void Init()
    {
        Services.MainCamera = Camera.main;
        Services.Input = new InputManager();
        Services.GameCfg = gameCfg;
        _controlSqrObj = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = new ControllerSquare(_controlSqrObj.transform);
        _targetSqrObj = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = _targetSqrObj.GetComponent<TargetSquare>();
        _targetRB = _targetSqrObj.GetComponent<Rigidbody2D>();
        _cancelButtonObj = GameObject.FindGameObjectWithTag("CancelButton");
        Services.CancelButton = new CancelButton(_cancelButtonObj);
        Services.CameraController = new CameraController(Services.MainCamera, true, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
        Services.LivesBar = new LivesBar(GameObject.FindGameObjectWithTag("LivesBar").transform);
        Services.VelocityBar = new VelocityBar(GameObject.FindGameObjectWithTag("SpeedBar").transform,
            GameObject.FindGameObjectWithTag("DirectionPointer").transform, _targetRB,
            GameObject.FindGameObjectWithTag("SpeedWarning"));
        _tmp = GetComponent<TextMeshPro>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Services.LivesBar.Init();
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
        Services.LivesBar.Update();
        Services.VelocityBar.Update();
    }

    private void LateUpdate()
    {
        foreach (var force in Services.Forces)
            force.Draw();
        
        Services.ControllerSquare.LateUpdate();
    }
}


