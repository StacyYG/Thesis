using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameCfg gameCfg;
    protected Rigidbody2D targetRb;
    protected GameObject targetSqr, ctrlButton, cxlButton, flagObj;
    protected TaskManager taskManager;

    public virtual void Awake()
    {
        Services.GameCfg = gameCfg;
        Arrow.SetUp();
        Services.MainCamera = Camera.main;
        Services.Input = new InputManager();
        taskManager = new TaskManager();
        
        ctrlButton = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerButton = new ControllerButton(ctrlButton);

        cxlButton = GameObject.FindGameObjectWithTag("CancelButton");
        Services.CancelButton = new CancelButton(cxlButton);
        
        targetSqr = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = targetSqr.GetComponent<TargetSquare>();
        targetRb = targetSqr.GetComponent<Rigidbody2D>();
        Services.CameraController = new CameraController(Services.MainCamera, true, targetSqr.transform);

        flagObj = GameObject.FindGameObjectWithTag("Goal");

        Services.EventManager = new EventManager();
        Services.EventManager.Register<Success>(OnSuccess);

        var speedWarning = GameObject.FindGameObjectWithTag("SpeedWarning");
        speedWarning.SetActive(false);
        
        Services.Forces = new List<Force>();
    }
    
    public virtual void Start()
    {
        Services.GameController.ShowButtons(false);
        Services.ControllerButton.Start();
        Services.CancelButton.Start();
        Services.VelocityLine = new VelocityLine(targetSqr);
        taskManager.Do(Services.ControllerButton.boundCircle.GrowUp);
        taskManager.Do(Services.CancelButton.boundCircle.GrowUp);
        if (targetRb.gravityScale > Mathf.Epsilon)
            new Gravity(targetSqr, Services.GameCfg.gravityColor);
    }

    public virtual void FixedUpdate()
    {
        if(ctrlButton.activeSelf && Services.ControllerButton.respond) 
            targetRb.AddForce(Services.ControllerButton.PlayerForce);
        foreach (var force in Services.Forces)
            force.Update();
        Services.CameraController.Update();
    }

    public virtual void Update()
    {
        Services.Input.Update();
        taskManager.Update();
    }

    public virtual void LateUpdate()
    {
        if(ctrlButton.activeSelf && Services.ControllerButton.respond) 
            Services.ControllerButton.LateUpdate();
        foreach (var force in Services.Forces)
            force.Draw();
        Services.VelocityLine.LateUpdate();
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
            ctrlButton.SetActive(false);
            Services.ControllerButton.respond = false;
            Services.ControllerButton.ResetPlayerForce();
            Services.ControllerButton.boundCircle.Clear();
            Services.CancelButton.boundCircle.Clear();
            Services.GameController.ShowButtons(true);
            foreach (var force in Services.Forces)
                force.HideLine(true);
            Services.VelocityLine.Hide(true);
        });
        waitToShowButton.Then(showButton);
        taskManager.Do(waitToShowButton);
    }

    private void OnDestroy()
    {
        Services.Input = null;
    }
}
