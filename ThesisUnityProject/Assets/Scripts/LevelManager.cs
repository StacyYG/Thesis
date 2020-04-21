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
        if (ctrlSqr)
            Services.ControllerSquare = new ControllerSquare(ctrlSqr);

        cxlButton = GameObject.FindGameObjectWithTag("CancelButton");
        if(cxlButton) 
            Services.CancelButton = new CancelButton(cxlButton);
        
        targetSqr = GameObject.FindGameObjectWithTag("TargetSquare");
        if (targetSqr)
        {
            Services.TargetSquare = targetSqr.GetComponent<TargetSquare>();
            targetRb = targetSqr.GetComponent<Rigidbody2D>();
            Services.CameraController = new CameraController(Services.MainCamera, true, targetSqr.transform);
            Services.VelocityBar = new VelocityBar(targetRb);
        }
        
        Services.EventManager = new EventManager();
    }

    public virtual void Start()
    {
        if(ctrlSqr) 
            taskManager.Do(Services.ControllerSquare.boundCircle.GrowUp);
        if(cxlButton) 
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
