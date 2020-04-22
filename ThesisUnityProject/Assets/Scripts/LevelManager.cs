using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    protected Rigidbody2D targetRb;
    protected GameObject targetSqr, ctrlSqr, cxlButton;
    public GameCfg gameCfg;
    protected TaskManager taskManager;
    
    public virtual void Awake()
    {
        Services.GameCfg = gameCfg;
        Arrow.SetUp();
        Services.MainCamera = Camera.main;
        Services.Input = new InputManager();
        taskManager = new TaskManager();
        
        ctrlSqr = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = new ControllerSquare(ctrlSqr);

        cxlButton = GameObject.FindGameObjectWithTag("CancelButton");
        Services.CancelButton = new CancelButton(cxlButton);
        
        targetSqr = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = targetSqr.GetComponent<TargetSquare>();
        targetRb = targetSqr.GetComponent<Rigidbody2D>();
        Services.CameraController = new CameraController(Services.MainCamera, true, targetSqr.transform);
        Services.VelocityBar = new VelocityBar(targetRb);

        Services.EventManager = new EventManager();

        var speedWarning = GameObject.FindGameObjectWithTag("SpeedWarning");
        speedWarning.SetActive(false);
        
    }

    public virtual void Start()
    {
        taskManager.Do(Services.ControllerSquare.boundCircle.GrowUp);
        taskManager.Do(Services.CancelButton.boundCircle.GrowUp);
    }

    public virtual void FixedUpdate()
    {
        Services.Input.Update();
        if(ctrlSqr.activeSelf && Services.ControllerSquare.Respond) 
            targetRb.AddForce(Services.ControllerSquare.PlayerForce);
        foreach (var force in Services.Forces)
            force.Update();
        if(Services.CameraController.isFollowing) 
            Services.CameraController.Update();
    }

    public virtual void Update()
    {
        Services.VelocityBar.Update();
        taskManager.Update();
    }

    public virtual void LateUpdate()
    {
        if(ctrlSqr.activeSelf && Services.ControllerSquare.Respond) 
            Services.ControllerSquare.LateUpdate();
        foreach (var force in Services.Forces)
            force.Draw();
    }
}
