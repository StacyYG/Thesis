using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class TargetSquare : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerForce _playerForce;
    private Dictionary<Collider2D, NormalForce> _normalForces;
    private Dictionary<Collider2D, Friction> _frictions;
    private Gravity _gravity;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _normalForces = new Dictionary<Collider2D, NormalForce>();
        _frictions = new Dictionary<Collider2D, Friction>();
        
        _playerForce = new PlayerForce(gameObject);

        if (Mathf.Abs(_rb.gravityScale) > Mathf.Epsilon)
        {
            _gravity = new Gravity(gameObject);
        }
        
    }
    
    private void FixedUpdate()
    {
        _playerForce.Update();
        _rb.AddForce(_playerForce.Vector);
        Services.CameraController.Update();
    }
    
    private void LateUpdate()
    {
        _playerForce.Draw();
        if (Mathf.Abs(_rb.gravityScale) > Mathf.Epsilon)
        {
            _gravity.Draw();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            NormalForce normalForce;
            if (!_normalForces.TryGetValue(other.collider, out normalForce))
            {
                normalForce = new NormalForce(gameObject, other, _normalForces.Count);
                _normalForces.Add(other.collider, normalForce);
            }


            Friction friction;
            if (!_frictions.TryGetValue(other.collider, out friction))
            {
                friction = new Friction(gameObject, other, _frictions.Count);
                _frictions.Add(other.collider, friction);
            }
        }

    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            NormalForce normalForce;
            if (_normalForces.TryGetValue(other.collider, out normalForce))
            {
                normalForce.Change(other);
            }

            Friction friction;
            if (_frictions.TryGetValue(other.collider, out friction))
            {
                friction.Change(other);
            }
        }

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            NormalForce normalForce;
            if (_normalForces.TryGetValue(other.collider, out normalForce))
            {
                normalForce.Reset();
            
            }

            Friction friction;
            if (_frictions.TryGetValue(other.collider, out friction))
            {
                friction.Reset();
            
            }
        }
        
    }
    
}
