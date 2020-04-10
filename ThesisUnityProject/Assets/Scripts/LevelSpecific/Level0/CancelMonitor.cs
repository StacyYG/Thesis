using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelMonitor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Services.CancelButton.respond) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Services.EventManager.Fire(new FirstCancel());
            Destroy(this);
        }
    }

    private void OnMouseDown()
    {
        if (!Services.CancelButton.respond) return;
        
        Services.EventManager.Fire(new FirstCancel());
        Destroy(this);
    }
}

public class FirstCancel : AGPEvent{}

