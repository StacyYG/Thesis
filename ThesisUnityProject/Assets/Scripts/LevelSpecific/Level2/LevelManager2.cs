using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class LevelManager2 : LevelManager
{
    private TextMeshPro _tmp;
    public GameObject barrier, gate0, gate1, randomComets0, randomComets1;
    public LevelCfg2 cfg2;
    
    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _tmp = GetComponent<TextMeshPro>();
        Services.GravityButton = new GravityButton(GameObject.FindGameObjectWithTag("GravityButton"));
        new Gravity(targetSqr);
        Services.EventManager.Register<ButtonObjPressed>(OnButtonPressed);
        Services.CameraController.isFollowing = false;
        Services.CameraController.lockY = true;
        flagObj.SetActive(false);
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
                randomComets1.SetActive(true);
                gate1.SetActive(true);
                flagObj.SetActive(true);
                Services.GravityButton.UpdateRbs();
                return true;
            }

            return false;
        });
        checkTargetX0.Then(checkTargetX1);
        taskManager.Do(checkTargetX0);
    }
    
    
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
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
}


