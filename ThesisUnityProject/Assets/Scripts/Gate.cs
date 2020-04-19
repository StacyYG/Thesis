using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Rigidbody2D _targetRB;
    private bool _constantVelocity, _isShut, _isTouching;
    public bool isDetect = true;
    private Vector2 _lastVelocity;
    public Material restingMaterial, hazardMaterial;
    private ParticleSystemRenderer[] _particleRdrs;
    public float recoverTime;
    private float _shutTimer;
    private BoxCollider2D _gateCollider;

    // Start is called before the first frame update
    void Start()
    {
        _particleRdrs = GetComponentsInChildren<ParticleSystemRenderer>();
        _gateCollider = GetComponent<BoxCollider2D>();
        foreach (var particleRdr in _particleRdrs)
        {
            particleRdr.material = restingMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isShut)
        {
            _shutTimer += Time.deltaTime;
            if (_shutTimer >= recoverTime && !_isTouching) OpenGate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDetect) return;
        
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            _constantVelocity = true;
            _targetRB = other.gameObject.GetComponent<Rigidbody2D>();
            _lastVelocity = _targetRB.velocity;
            _isTouching = true;
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
                Services.EventManager.Fire(new LoseLife());
                if(_targetRB.velocity.magnitude != _lastVelocity.magnitude)
                    Services.VelocityBar.speedSprRdr.color = Color.red;
                if(_targetRB.velocity.normalized != _lastVelocity.normalized)
                    Services.VelocityBar.directionSprRdr.color = Color.red;
                ShutGate();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _isTouching = false;
        Services.VelocityBar.speedSprRdr.color = Color.white;
        Services.VelocityBar.directionSprRdr.color = Color.white;
    }

    private void ShutGate()
    {
        _isShut = true;
        foreach (var particleRdr in _particleRdrs) particleRdr.material = hazardMaterial;
        _gateCollider.isTrigger = false;
    }
    
    private void OpenGate()
    {
        _isShut = false;
        foreach (var particleRdr in _particleRdrs) particleRdr.material = restingMaterial;
        _gateCollider.isTrigger = true;
    }
}

public class Success : AGPEvent{}