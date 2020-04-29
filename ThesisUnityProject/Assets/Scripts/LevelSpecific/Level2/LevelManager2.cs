using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager2 : LevelManager
{
    private TextMeshPro _tmp;
    public override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _tmp = GetComponent<TextMeshPro>();
    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }
    
    
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    
}


