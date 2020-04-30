using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class LevelManager2 : LevelManager
{
    private TextMeshPro _tmp;
    public GameObject barrier, gate0, gate1, gate2, randomComets0, randomComets1;
    public LevelCfg2 cfg2;
    
    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _tmp = GetComponent<TextMeshPro>();
        var circle = GameObject.FindGameObjectWithTag("GravityButton");
        Services.GravityButton = new GravityButton(circle, circle.transform.GetChild(0).gameObject);
        new Gravity(targetSqr);
        Services.EventManager.Register<ButtonObjPressed>(OnButtonPressed);
        Services.CameraController.isFollowing = false;
        Services.CameraController.lockY = true;
        flagObj.SetActive(false);
        randomComets1.SetActive(false);
        gate0.SetActive(false);
        gate1.SetActive(false);
        gate2.SetActive(false);
        Services.EventManager.Register<FirstGravity>(OnFirstGravity);
    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Services.GravityButton.Start();
        taskManager.Do(Services.GravityButton.boundCircle.GrowUp);
        foreach (var comet in randomComets0.GetComponentsInChildren<Rigidbody2D>())
        {
            comet.AddTorque(Random.Range(-0.3f, 0.3f));
        }
        var checkTargetX0 = new DelegateTask(() => {}, () =>
        {
            if (targetSqr.transform.position.x > cfg2.cameraFollowX)
            {
                Services.CameraController.isFollowing = true;
                return true;
            }

            return false;
        });
        
        var checkTargetX1 = new DelegateTask(() => {}, () =>
        {
            if (targetSqr.transform.position.x > cfg2.loadSecondPartX)
            {
                gate1.SetActive(true);
                Services.GravityButton.UpdateRbs();
                return true;
            }

            return false;
        });
        
        var checkTargetX2 = new DelegateTask(() => {}, () =>
        {
            if (targetSqr.transform.position.x > cfg2.loadThirdPartX)
            {
                randomComets1.SetActive(true);
                gate2.SetActive(true);
                flagObj.SetActive(true);
                Services.GravityButton.UpdateRbs();
                return true;
            }

            return false;
        });
        checkTargetX0.Then(checkTargetX1).Then(checkTargetX2);
        taskManager.Do(checkTargetX0);
    }
    
    
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.G))
        {
            Services.GravityButton.GravitySwitch();
        }
        Services.GravityButton.UpdateGravity();
    }

    public override void OnSuccess(AGPEvent e)
    {
        base.OnSuccess(e);
        Services.GravityButton.boundCircle.Clear();
        var moreLevels = new WaitAndPrint(_tmp, Services.GameCfg.afterSuccessWaitTime, Services.GameCfg.moreLevels);
        taskManager.Do(moreLevels);
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<ButtonObjPressed>(OnButtonPressed);
    }
    private void OnButtonPressed(AGPEvent e)
    {
        var button = (ButtonObjPressed) e;
        barrier.SetActive(!button.isPressed);
        gate0.SetActive(button.isPressed);
    }

    private void OnFirstGravity(AGPEvent e)
    {
        Services.EventManager.Unregister<FirstGravity>(OnFirstGravity);
        _tmp.text = cfg2.gravityInstruction;
        var wait = new WaitTask(cfg2.gravityInstructionDuration);
        var clear = new ActionTask(() => _tmp.text = "");
        wait.Then(clear);
        taskManager.Do(wait);
    }
}


