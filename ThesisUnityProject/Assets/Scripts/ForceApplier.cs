using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using Color = System.Drawing.Color;

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
        _playerForceLine =
            new VectorLine("playerForce", new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
        _playerForceLine.drawTransform = transform;
        _playerForceLine.endCap = "fullArrow";
        _gravityLine = new VectorLine("Gravity",
            new List<Vector3> {Vector2.zero, GravityVector()},
            lineWidth);
        _gravityLine.drawTransform = transform;
        _gravityLine.color = UnityEngine.Color.gray;
        _gravityLine.endCap = "fullArrow";
        _gravityLine.Draw();
    }

    private void Update()
    {
        UpdateForceLine(_playerForceLine, ServiceLocator.ControllerSquare.PlayerForce());
        UpdateForceLine(_gravityLine, GravityVector());
    }
    
    private void FixedUpdate()
    {
        _rb.AddForce(ServiceLocator.ControllerSquare.PlayerForce());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collideObjColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
        VectorLine thisNormalForceLine;
        if (!_colliderToNormalForceLine.TryGetValue(collision.collider, out thisNormalForceLine))
        {
            thisNormalForceLine = new VectorLine("N" + _colliderToNormalForceLine.Count,
                new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
            thisNormalForceLine.drawTransform = transform;
            thisNormalForceLine.color = collideObjColor;
            thisNormalForceLine.endCap = "fullArrow";
            _colliderToNormalForceLine.Add(collision.collider, thisNormalForceLine);
        }
        
        thisNormalForceLine.active = true;
        
        VectorLine thisFrictionLine;
        if (!_colliderToFrictionLine.TryGetValue(collision.collider, out thisFrictionLine))
        {
            thisFrictionLine =new VectorLine("f" + _colliderToFrictionLine.Count,
                new List<Vector3> {Vector2.zero, Vector2.zero}, lineWidth);
            thisFrictionLine.drawTransform = transform;
            thisFrictionLine.color = collideObjColor;
            thisFrictionLine.endCap = "fullArrow";
            _colliderToFrictionLine.Add(collision.collider, thisFrictionLine);
        }

        thisFrictionLine.active = true;

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        VectorLine thisNormalForce;
        if (_colliderToNormalForceLine.TryGetValue(collision.collider, out thisNormalForce))
        {
            UpdateForceLine(thisNormalForce, NormalForceVector(collision, false));
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

    private Vector2 NormalForceVector(Collision2D collision, bool isImpulse)
    {
        var normalDirection = collision.GetContact(0).normal.normalized;
        var magnitude = collision.GetContact(0).normalImpulse;
        if (collision.contactCount > 1)
        {
            magnitude += collision.GetContact(1).normalImpulse;
        }

        if (isImpulse) return magnitude * normalDirection;
        return magnitude / Time.fixedDeltaTime * normalDirection;
        
    }

    private Vector2 FrictionVector(Collision2D collision)
    {
        var normalDirection = collision.GetContact(0).normal.normalized;
        var direction = Quaternion.AngleAxis(-90, Vector3.forward) * normalDirection;
        var magnitude = collision.GetContact(0).tangentImpulse;
        if (collision.contactCount > 1)
        {
            magnitude += collision.GetContact(1).tangentImpulse;
        }
        
        return magnitude / Time.fixedDeltaTime * direction;
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
}
