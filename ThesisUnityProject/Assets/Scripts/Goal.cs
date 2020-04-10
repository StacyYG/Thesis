using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private Rigidbody2D _targetRB;
    private bool _constantVelocity;
    private Vector2 _lastVelocity;

    public bool isDetect;

    private int _failTimes;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDetect) return;
        
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            _constantVelocity = true;
            _targetRB = other.gameObject.GetComponent<Rigidbody2D>();
            _lastVelocity = _targetRB.velocity;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isDetect) return;

        if (other.gameObject.CompareTag("TargetSquare"))
        {
            if (_targetRB.velocity != _lastVelocity && _constantVelocity)
            {
                _constantVelocity = false;
                Services.EventManager.Fire(new Fail(_failTimes));
                _failTimes++;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isDetect) return;
        
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            if (_constantVelocity)
            {
                Services.EventManager.Fire(new Success());
            }
        }
    }
}

public class Success : AGPEvent{}

public class Fail : AGPEvent
{
    public int _failTimes;

    public Fail(int failTimes)
    {
        _failTimes = failTimes;
    }
}