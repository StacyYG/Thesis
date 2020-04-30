using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameCfg gameCfg;
    protected Rigidbody2D targetRb;
    protected GameObject targetSqr, ctrlSqr, cxlButton, flagObj;
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

        flagObj = GameObject.FindGameObjectWithTag("Goal");

        Services.EventManager = new EventManager();

        var speedWarning = GameObject.FindGameObjectWithTag("SpeedWarning");
        speedWarning.SetActive(false);
        
        Services.Forces = new List<Force>();
        Services.GameController.ShowButtons(false);
        Services.EventManager.Register<Success>(OnSuccess);
    }
    
    public virtual void Start()
    {
        Services.ControllerSquare.Start();
        Services.CancelButton.Start();
        taskManager.Do(Services.ControllerSquare.boundCircle.GrowUp);
        taskManager.Do(Services.CancelButton.boundCircle.GrowUp);
    }

    public virtual void FixedUpdate()
    {
        if(ctrlSqr.activeSelf && Services.ControllerSquare.respond) 
            targetRb.AddForce(Services.ControllerSquare.PlayerForce);
        foreach (var force in Services.Forces)
            force.Update();
        if(Services.CameraController.isFollowing) 
            Services.CameraController.Update();
    }

    public virtual void Update()
    {
        Services.Input.Update();
        Services.VelocityBar.Update();
        taskManager.Update();
    }

    public virtual void LateUpdate()
    {
        if(ctrlSqr.activeSelf && Services.ControllerSquare.respond) 
            Services.ControllerSquare.LateUpdate();
        foreach (var force in Services.Forces)
            force.Draw();
    }

    public virtual void OnSuccess(AGPEvent e)
    {
        Services.EventManager.Unregister<Success>(OnSuccess);
        var p = Instantiate(Services.GameCfg.successParticles, targetSqr.transform.position, Quaternion.identity);
        var waitToShowButton = new WaitTask(Services.GameCfg.afterSuccessWaitTime);
        var showButton = new ActionTask(() =>
        {
            var cameraTransform = Services.MainCamera.transform;
            var shade = Instantiate(gameCfg.shade,
                new Vector3(cameraTransform.position.x, cameraTransform.position.y, 0f),
                Quaternion.identity, cameraTransform);
            cxlButton.SetActive(false);
            ctrlSqr.SetActive(false);
            Services.ControllerSquare.respond = false;
            Services.ControllerSquare.ResetPlayerForce();
            Services.ControllerSquare.LateUpdate();
            Services.ControllerSquare.boundCircle.Clear();
            Services.CancelButton.boundCircle.Clear();
            Services.GameController.ShowButtons(true);
        });
        waitToShowButton.Then(showButton);
        taskManager.Do(waitToShowButton);
    }

    private void OnDestroy()
    {
        Services.Input = null;
    }
}
