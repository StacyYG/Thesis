using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager2 : LevelManager
{
    public LevelCfg1 levelCfg1;
    private TextMeshPro _tmp;
    private Rigidbody2D _targetRB;
    private GameObject _controlSqrObj, _targetSqrObj, _cancelButtonObj;
    private int _lastIndex;
    public GameCfg gameCfg;
    
    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        Services.LivesBar = new LivesBar(GameObject.FindGameObjectWithTag("LivesBar").transform);
        _tmp = GetComponent<TextMeshPro>();
    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Services.LivesBar.Init();
    }
    
    
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Services.LivesBar.Update();
    }
    
}


