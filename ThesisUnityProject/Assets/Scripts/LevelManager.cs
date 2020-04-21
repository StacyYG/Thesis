using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    protected Rigidbody2D targetRb;
    protected GameObject targetSqr;
    public GameCfg gameCfg;
    protected bool controlButtonGrowing = true;
    protected float controlButtonGrowTimer;
    protected TaskManager taskManager;
    
    public virtual void Awake()
    {
        Services.GameCfg = gameCfg;
        Arrow.SetUp();
        Services.MainCamera = Camera.main;
        Services.Input = new InputManager();
        taskManager = new TaskManager();
        Services.ControllerSquare =
            new ControllerSquare(GameObject.FindGameObjectWithTag("ControllerSquare"));
        Services.CancelButton = new CancelButton(GameObject.FindGameObjectWithTag("CancelButton"));
        targetSqr = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = targetSqr.GetComponent<TargetSquare>();
        targetRb = targetSqr.GetComponent<Rigidbody2D>();
        Services.CameraController = new CameraController(Services.MainCamera, true, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
        Services.VelocityBar = new VelocityBar(GameObject.FindGameObjectWithTag("SpeedBar"),
            GameObject.FindGameObjectWithTag("DirectionPointer"), targetRb,
            GameObject.FindGameObjectWithTag("SpeedWarning"));
    }

    public virtual void Start()
    {
        taskManager.Do(Services.ControllerSquare.boundCircle.GrowUp);
        taskManager.Do(Services.CancelButton.boundCircle.GrowUp);
    }

    public virtual void FixedUpdate()
    {
        Services.Input.Update();
        targetRb.AddForce(Services.ControllerSquare.PlayerForce);
        foreach (var force in Services.Forces)
            force.Update();
        Services.CameraController.Update();
    }

    public virtual void Update()
    {
        Services.VelocityBar.Update();

    }

    public virtual void LateUpdate()
    {
        Services.ControllerSquare.LateUpdate();
        foreach (var force in Services.Forces)
            force.Draw();
    }
}
