using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager1 : MonoBehaviour
{
    public LevelCfg1 levelCfg1;
    private TextMeshPro _tmp;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj, _targetSqrObj, _cancelButtonObj;
    private int _lastIndex;
    
    public void Awake()
    {
        Init();
    }

    private void Init()
    {
        Services.MainCamera = Camera.main;
        _controlSqrObj = GameObject.FindGameObjectWithTag("ControllerSquare");
        Services.ControllerSquare = _controlSqrObj.GetComponent<ControllerSquare>();
        _targetSqrObj = GameObject.FindGameObjectWithTag("TargetSquare");
        Services.TargetSquare = _targetSqrObj.GetComponent<TargetSquare>();
        _targetRB = _targetSqrObj.GetComponent<Rigidbody2D>();
        _cancelButtonObj = GameObject.FindGameObjectWithTag("CancelButton");
        Services.CancelButton = _cancelButtonObj.GetComponent<CancelButton>();
        Services.CameraController = new CameraController(Services.MainCamera, true, Services.TargetSquare.transform);
        Services.EventManager = new EventManager();
        _tmp = GetComponent<TextMeshPro>();
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }
    


    
    // Update is called once per frame
    void Update()
    {
        
    }
}


