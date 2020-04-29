using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMonitor : MonoBehaviour
{
    private float _detectThreshold = 0.1f;

    private bool _firstForceFired;

    private bool _secondForceFired;

    private int _clickTimes;

    private void OnMouseDrag()
    {
        if (!Services.ControllerSquare.respond) return;
        if (Services.ControllerSquare.PlayerForce.magnitude > _detectThreshold)
        {
            if (_clickTimes == 0 && !_firstForceFired)
            {
                Services.EventManager.Fire(new FirstForce());
                _firstForceFired = true;
            }

            if (_clickTimes == 1 && !_secondForceFired)
            {
                Services.EventManager.Fire(new SecondForce());
                _secondForceFired = true;
                Destroy(this);
            }
        }
    }

    private void OnMouseUp()
    {
        if (!Services.ControllerSquare.respond) return;
        if(_firstForceFired) 
            _clickTimes++;
    }
}

public class FirstForce : AGPEvent{}

public class SecondForce : AGPEvent{}
