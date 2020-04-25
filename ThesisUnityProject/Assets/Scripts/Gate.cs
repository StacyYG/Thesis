using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Rigidbody2D _targetRb;
    private List<Rigidbody2D> _rbs;
    private Dictionary<Rigidbody2D, float> _lastVelocity;
    private List<TrackVelocity> _trackVelocities;
    private bool _constantVelocity, _isShut, _isTouching;
    public bool isDetect = true;
    private Vector2 _targetLastVelocity;
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
        _trackVelocities = new List<TrackVelocity>();
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
            if (_shutTimer >= recoverTime) OpenGate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDetect) return;
        
        // if (other.gameObject.CompareTag("TargetSquare"))
        // {
        //     _constantVelocity = true;
        //     _targetRb = other.gameObject.GetComponent<Rigidbody2D>();
        //     _targetLastVelocity = _targetRb.velocity;
        //     _isTouching = true;
        // }
        
        var trackV = new TrackVelocity();
        trackV.gameObject = other.gameObject;
        trackV.rb = other.gameObject.GetComponent<Rigidbody2D>();
        trackV.firstVelocity = trackV.rb.velocity;
        trackV.collider = other;
        _trackVelocities.Add(trackV);
        Debug.Log("added, " + _trackVelocities.Count);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isDetect) return;

        // if (other.gameObject.CompareTag("TargetSquare"))
        // {
        //     if (_targetRb.velocity != _targetLastVelocity && _constantVelocity)
        //     {
        //         _constantVelocity = false;
        //         Services.EventManager.Fire(new LoseLife());
        //         if(_targetRb.velocity.magnitude != _targetLastVelocity.magnitude)
        //             Services.VelocityBar.speedSprRdr.color = Color.red;
        //         if(_targetRb.velocity.normalized != _targetLastVelocity.normalized)
        //             Services.VelocityBar.directionSprRdr.color = Color.red;
        //         ShutGate();
        //     }
        // }

        foreach (var trackV in _trackVelocities)
        {
            if (trackV.rb.velocity != trackV.firstVelocity)
            {
                ShutGate();
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        for (int i = 0; i < _trackVelocities.Count; i++)
        {
            if (ReferenceEquals(_trackVelocities[i].collider, other))
            {
                _trackVelocities.Remove(_trackVelocities[i]);
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
        _shutTimer = 0f;
        foreach (var particleRdr in _particleRdrs) particleRdr.material = hazardMaterial;
        _gateCollider.isTrigger = false;
        for (int i = 0; i < _trackVelocities.Count; i++)
        {
            var gameObj = _trackVelocities[i].gameObject;
            if(!gameObj.CompareTag("TargetSquare")) 
                _trackVelocities[i].gameObject.SetActive(false);
        }
        _trackVelocities = new List<TrackVelocity>();
    }
    
    private void OpenGate()
    {
        _isShut = false;
        foreach (var particleRdr in _particleRdrs) particleRdr.material = restingMaterial;
        _gateCollider.isTrigger = true;
    }
}

public class Success : AGPEvent{}

public struct TrackVelocity
{
    public Rigidbody2D rb;
    public Vector2 firstVelocity;
    public Collider2D collider;
    public GameObject gameObject;
}
