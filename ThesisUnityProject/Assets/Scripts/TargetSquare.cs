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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        NormalForce normalForce;
        if (!_normalForces.TryGetValue(collision.collider, out normalForce))
        {
            normalForce = new NormalForce(gameObject, collision, _normalForces.Count);
            _normalForces.Add(collision.collider, normalForce);
        }


        Friction friction;
        if (!_frictions.TryGetValue(collision.collider, out friction))
        {
            friction = new Friction(gameObject, collision, _frictions.Count);
            _frictions.Add(collision.collider, friction);
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        NormalForce normalForce;
        if (_normalForces.TryGetValue(collision.collider, out normalForce))
        {
            normalForce.Change(collision);
        }

        Friction friction;
        if (_frictions.TryGetValue(collision.collider, out friction))
        {
            friction.Change(collision);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        NormalForce normalForce;
        if (_normalForces.TryGetValue(collision.collider, out normalForce))
        {
            normalForce.Reset();
            
        }

        Friction friction;
        if (_frictions.TryGetValue(collision.collider, out friction))
        {
            friction.Reset();
            
        }
    }
    
}
