using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class ForceApplier : MonoBehaviour
{
    private Rigidbody2D _rb;
    private VectorLine _playerForceLine;
    private Dictionary<Collider2D, VectorLine> _colliderToNormalForceLine;
    private Dictionary<Collider2D, VectorLine> _colliderToFrictionLine;
    private VectorLine _gravityLine;
    public float lineWidth = 6f;

    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _colliderToNormalForceLine = new Dictionary<Collider2D, VectorLine>();
        _colliderToFrictionLine = new Dictionary<Collider2D, VectorLine>();

        _playerForceLine = CreateNewLine(Color.white, "fullArrow");
        _playerForceLine.name = "PlayerForce";

        _gravityLine = CreateNewLine(Color.gray, "fullArrow");
        _gravityLine.name = "Gravity";
        _gravityLine.Draw();
    }

    private void Update()
    {
        UpdateForceLine(_playerForceLine, Services.ControllerSquare.PlayerForce());
        UpdateForceLine(_gravityLine, GravityVector());
    }
    
    private void FixedUpdate()
    {
        _rb.AddForce(Services.ControllerSquare.PlayerForce());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collideObjColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
        
        VectorLine thisNormalForceLine;
        if (!_colliderToNormalForceLine.TryGetValue(collision.collider, out thisNormalForceLine))
        {
            thisNormalForceLine = CreateNewLine(collideObjColor, "fullArrow");
            thisNormalForceLine.name = "N" + _colliderToNormalForceLine.Count;
            _colliderToNormalForceLine.Add(collision.collider, thisNormalForceLine);
        }
        
        thisNormalForceLine.active = true;
        
        VectorLine thisFrictionLine;
        if (!_colliderToFrictionLine.TryGetValue(collision.collider, out thisFrictionLine))
        {
            thisFrictionLine = CreateNewLine(collideObjColor, "fullArrow");
            thisFrictionLine.name = "f" + _colliderToFrictionLine.Count;
            _colliderToFrictionLine.Add(collision.collider, thisFrictionLine);
        }

        thisFrictionLine.active = true;

    }

    
    private void OnCollisionStay2D(Collision2D collision)
    {
        VectorLine thisNormalForce;
        if (_colliderToNormalForceLine.TryGetValue(collision.collider, out thisNormalForce))
        {
            UpdateForceLine(thisNormalForce, NormalForceVector(collision));
        }

        VectorLine thisFriction;
        if (_colliderToFrictionLine.TryGetValue(collision.collider, out thisFriction))
        {
            UpdateForceLine(thisFriction, FrictionVector(collision));
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        VectorLine thisNormalForce;
        if (_colliderToNormalForceLine.TryGetValue(collision.collider, out thisNormalForce))
        {
            thisNormalForce.active = false;
        }

        VectorLine thisFriction;
        if (_colliderToFrictionLine.TryGetValue(collision.collider, out thisFriction))
        {
            thisFriction.active = false;
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

    private VectorLine CreateNewLine(Color32 lineColor, string endCap)
    {
        var thisLine = new VectorLine("", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        thisLine.drawTransform = transform;
        thisLine.color = lineColor;
        thisLine.endCap = endCap;
        return thisLine;
    }
}
