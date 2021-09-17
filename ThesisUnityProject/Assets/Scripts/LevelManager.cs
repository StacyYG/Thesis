using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameCfg gameCfg;
    protected Rigidbody2D targetRb;
    protected GameObject targetSqr, ctrlButton, cxlButton, gravityButton, flagObj;
    protected TaskManager taskManager;
    public bool _useCxlButton, _useGravityButton;

    public virtual void Awake()
    {
        Services.GameCfg = gameCfg;
        Arrow.SetUp();
        Services.MainCamera = Camera.main;
        Services.Input = new InputManager();
        taskManager = new TaskManager();
        
        ctrlButton = GameObject.FindGameObjectWithTag("ControlButton");
        if (ctrlButton != null)
        {
            Services.ControlButton = new ControlButton(ctrlButton);
        }

        cxlButton = GameObject.FindGameObjectWithTag("CancelButton");
        if (cxlButton != null)
        {
            _useCxlButton = true;
            Services.CancelButton = new CancelButton(cxlButton);
        }

        gravityButton = GameObject.FindGameObjectWithTag("GravityButton");
        if (gravityButton != null)
        {
            _useGravityButton = true;
            Services.GravityButton = new GravityButton(gravityButton);
        }

        targetSqr = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = targetSqr.GetComponent<TargetSquare>();
        targetRb = targetSqr.GetComponent<Rigidbody2D>();
        Services.CameraController = new CameraController(Services.MainCamera, true, targetSqr.transform);

        flagObj = GameObject.FindGameObjectWithTag("Goal");

        Services.EventManager = new EventManager();
        Services.EventManager.Register<Success>(OnSuccess);

        Services.Forces = new List<Force>();
    }
    
    public virtual void Start()
    {
        Services.GameController.ShowMenu(false);
        Services.ControlButton.Init();
        if (_useCxlButton)
        {
            Services.CancelButton.CreateCircle();
        }

        if (_useGravityButton)
        {
            Services.GravityButton.Start();
            taskManager.Do(Services.GravityButton.boundCircle.GrowUp);
        }
        
        Services.VelocityLine = new VelocityLine(targetSqr);
        taskManager.Do(Services.ControlButton.boundCircle.GrowUp);
        taskManager.Do(Services.CancelButton.boundCircle.GrowUp);
    }

    public virtual void FixedUpdate() // Physics calculations
    {
        // Apply the PlayerForce to the target square
        if(ctrlButton.activeSelf) 
            targetRb.AddForce(Services.ControlButton.PlayerForce);

        // Update the gravity of the objects within the camera view
        if (_useGravityButton)
        {
            Services.GravityButton.UpdateObjectsAndGravity();
        }      
        
        // Move the camera in FixedUpdate to avoid the lagging
        Services.CameraController.Update();
    }

    public virtual void Update()
    {
        // Handle inputs
        Services.Input.Update();
        if (_useCxlButton)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Services.Input.PressCancelButton();
            }
        }
        if (_useGravityButton)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Services.GravityButton.GravitySwitch();
            }
        }
        
        taskManager.Update();
        
        // Update PlayerForce and Gravity vector line in the background
        foreach (var force in Services.Forces)
            force.Update();
    }

    public virtual void LateUpdate()
    {
        // Draw the vector lines for all forces
        if(ctrlButton.activeSelf) 
            Services.ControlButton.DrawForceLines();
        foreach (var force in Services.Forces)
            force.Draw();
        Services.VelocityLine.Draw();
    }

    protected virtual void OnSuccess(AGPEvent e)
    {
        // Play particles
        Services.EventManager.Unregister<Success>(OnSuccess);
        var p = Instantiate(Services.GameCfg.successParticles, targetSqr.transform.position, Quaternion.identity);
        
        // Transit to menu
        var wait = new WaitTask(Services.GameCfg.afterSuccessWaitTime);
        var showButton = new ActionTask(() =>
        {
            // Show the menu buttons
            var cameraTransform = Services.MainCamera.transform;
            Instantiate(gameCfg.shade, new Vector3(cameraTransform.position.x, cameraTransform.position.y, 0f),
                Quaternion.identity, cameraTransform);
            Services.GameController.ShowMenu(true);
            
            // Clear Control Button UI
            Services.ControlButton.ResetPlayerForce();
            Services.ControlButton.boundCircle.Clear();
            ctrlButton.SetActive(false);

            // Clear the Cancel Button UI
            if (_useCxlButton)
            {
                Services.CancelButton.boundCircle.Clear();
                cxlButton.SetActive(false);
            }

            if (_useGravityButton)
            {
                Services.GravityButton.boundCircle.Clear();
                gravityButton.SetActive(false);
            }
            
            // Clear the forces
            foreach (var force in Services.Forces)
                force.DestroyLine();
            Services.Forces.Clear();
            Services.VelocityLine.Destroy();
        });
        
        wait.Then(showButton);
        taskManager.Do(wait);
    }

    protected virtual void OnDestroy()
    {
        Services.Input = null;
    }
}
