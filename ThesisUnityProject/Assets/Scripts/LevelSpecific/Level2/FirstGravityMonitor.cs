using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstGravityMonitor : MonoBehaviour
{
    private void OnMouseDown()
    {
        Services.EventManager.Fire(new FirstGravity());
        Destroy(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Services.EventManager.Fire(new FirstGravity());
            Destroy(this);
        }
    }
}

public class FirstGravity : AGPEvent{}
