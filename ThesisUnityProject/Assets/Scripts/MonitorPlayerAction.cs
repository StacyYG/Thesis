using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorPlayerAction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool _isFirstTime = true; 
    private void OnMouseDown()
    {
        if (_isFirstTime)
        {
            Services.EventManager.Fire(new FirstForce());
            _isFirstTime = false;
        }
    }
}

public class FirstForce : AGPEvent{}
