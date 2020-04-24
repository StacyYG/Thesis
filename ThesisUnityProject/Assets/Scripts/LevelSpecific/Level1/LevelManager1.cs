using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LevelManager1 : LevelManager
{
    public LevelCfg1 cfg1;
    private TextMeshPro _tmp;
    private GameObject _gateObj, _flagObj, _highlightObj;
    private int _failTimes;

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _highlightObj = GameObject.FindGameObjectWithTag("Highlight");
        _highlightObj.SetActive(false);
        _tmp = GetComponent<TextMeshPro>();
        Services.CameraController.lockY = true;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        Services.ControllerSquare.Start();
        taskManager.Do(Services.ControllerSquare.boundCircle.GrowUp);

    }


    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
    

