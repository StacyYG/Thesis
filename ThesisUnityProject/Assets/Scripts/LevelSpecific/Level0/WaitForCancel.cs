using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForCancel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Services.EventManager.Fire(new FirstCancel());
            Destroy(this);
        }
    }

    private void OnMouseDown()
    {
        Services.EventManager.Fire(new FirstCancel());
        Destroy(this);
    }
}

public class FirstCancel : AGPEvent{}

