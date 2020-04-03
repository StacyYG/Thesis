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
    public float lineWidth = 35f;

    
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

    private void LateUpdate()
    {
        _playerForce.Update();
        if (Mathf.Abs(_rb.gravityScale) > Mathf.Epsilon)
        {
            _gravity.Update();
        }
        
    }
    
    private void FixedUpdate()
    {
        _rb.AddForce(Services.ControllerSquare.PlayerForce);
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
            normalForce.SetForceVector(NormalForceVector(collision));
        }

        Friction friction;
        if (_frictions.TryGetValue(collision.collider, out friction))
        {
            friction.SetForceVector(FrictionVector(collision));
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        NormalForce normalForce;
        if (_normalForces.TryGetValue(collision.collider, out normalForce))
        {
            normalForce.Reset();
            _prevNormalForceSize = 0f;
        }

        Friction friction;
        if (_frictions.TryGetValue(collision.collider, out friction))
        {
            friction.Reset();
            _prevFrictionSize = 0f;
        }
    }

    private float _prevNormalForceSize;
    private Vector2 NormalForceVector(Collision2D collision)
    {
        var direction = collision.GetContact(0).normal.normalized;
        var size = collision.GetContact(0).normalImpulse / Time.fixedDeltaTime;
        if (collision.contactCount > 1)
        {
            size += collision.GetContact(1).normalImpulse / Time.fixedDeltaTime;
        }
        
        var lerpSize = Mathf.Lerp(_prevNormalForceSize, size, 0.3f);
        _prevNormalForceSize = lerpSize;
        return lerpSize * direction;
    }

    private float _prevFrictionSize;
    private Vector2 FrictionVector(Collision2D collision)
    {
        var normalDirection = collision.GetContact(0).normal.normalized;
        var direction = Quaternion.AngleAxis(-90, Vector3.forward) * normalDirection;
        var size = collision.GetContact(0).tangentImpulse / Time.fixedDeltaTime;
        if (collision.contactCount > 1)
        {
            size += collision.GetContact(1).tangentImpulse / Time.fixedDeltaTime;
        }

        var lerpSize = Mathf.Lerp(_prevFrictionSize, size, 0.3f);
        _prevFrictionSize = lerpSize;
        return lerpSize * direction;
    }

    private Vector2 GravityVector()
    {
        return new Vector2(0f, _rb.gravityScale * Physics2D.gravity.y * _rb.mass);
    }

    private void UpdateForceLine(VectorLine vectorLine, Vector2 forceVector)
    {
        Vector2 pureForceVector = transform.InverseTransformVector(forceVector);
        vectorLine.points3[0] = pureForceVector;
        vectorLine.Draw();
    }

    private VectorLine CreateNewLine(Color lineColor, string endCap)
    {
        var thisLine = new VectorLine("", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        thisLine.drawTransform = transform;
        thisLine.color = lineColor;
        thisLine.endCap = endCap;
        return thisLine;
    }
}
