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

    public int ClickTimes { get; private set; }
    private void OnMouseDown()
    {
        ClickTimes++;
        if (ClickTimes == 1)
        {
            Services.EventManager.Fire(new FirstForce());
        }

        if (ClickTimes == 2)
        {
            Services.EventManager.Fire(new SecondForce());
        }
    }
}

public class FirstForce : AGPEvent{}

public class SecondForce : AGPEvent{}
