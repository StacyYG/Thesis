using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceApplier : MonoBehaviour
{
    private Rigidbody2D _rb;
    private GameObject _playerForceObj;
    private GameObject _normalForceObj0;
    private GameObject _normalForceObj1;
    private Vector3 _normalForceVector;
    private Dictionary<Collider2D, GameObject> _colliderToNormalForceObj;
    private Dictionary<Collider2D, GameObject> _colliderToFrictionObj;
    private GameObject _gravityObj;

    
    // Start is called before the first frame update
    void Start()
    {
        _playerForceObj = InstantiateForceObj("playerForceObj");
        _rb = GetComponent<Rigidbody2D>();
        _colliderToNormalForceObj = new Dictionary<Collider2D, GameObject>();
        _colliderToFrictionObj = new Dictionary<Collider2D, GameObject>();
        SetGravity();
    }

    private void Update()
    {
        if (ServiceLocator.ControllerSquare.holdingMouse)
        {
            UpdateForceObj(_playerForceObj, ServiceLocator.ControllerSquare.PlayerForce());
        }
        _playerForceObj.transform.position = transform.position;
        _gravityObj.transform.position = transform.position;

    }
    
    private void FixedUpdate()
    {
        _rb.AddForce(ServiceLocator.ControllerSquare.PlayerForce());
    }
    
    private GameObject InstantiateForceObj(string name)
    {
        var forceObj = Instantiate(Resources.Load<GameObject>("square10"));
        forceObj.name = name;
        var spriteRenderer = forceObj.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = ServiceLocator.GameController.orderInLayer;
        ServiceLocator.GameController.orderInLayer++;
        forceObj.transform.position = transform.position;
        forceObj.transform.localScale = Vector3.zero;
        return forceObj;
    }
    private void UpdateForceObj(GameObject forceObj, Vector3 forceVector)
    {
        forceObj.transform.position = transform.position;
        forceObj.transform.localScale = new Vector3(1f, forceVector.magnitude * 10f, 1f);
        forceObj.transform.up = forceVector.normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject newNormalForceObj;
        if (!_colliderToNormalForceObj.TryGetValue(collision.collider, out newNormalForceObj))
        {
            newNormalForceObj = InstantiateForceObj("N" + _colliderToNormalForceObj.Count);
            var spriteRenderer = newNormalForceObj.GetComponent<SpriteRenderer>();
            spriteRenderer.color = collision.gameObject.GetComponent<SpriteRenderer>().color;
            _colliderToNormalForceObj.Add(collision.collider, newNormalForceObj);
        }

        GameObject newFrictionObg;
        if (!_colliderToFrictionObj.TryGetValue(collision.collider, out newFrictionObg))
        {
            newFrictionObg = InstantiateForceObj("f" + _colliderToFrictionObj.Count);
            var spriteRenderer = newFrictionObg.GetComponent<SpriteRenderer>();
            spriteRenderer.color = collision.gameObject.GetComponent<SpriteRenderer>().color;
            _colliderToFrictionObj.Add(collision.collider, newFrictionObg);
        }

        UpdateForceObj(newNormalForceObj, NormalForceSetter(collision,true));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject currentNormalForce;
        if (_colliderToNormalForceObj.TryGetValue(collision.collider, out currentNormalForce))
        {
            UpdateForceObj(currentNormalForce, NormalForceSetter(collision, false));
        }

        GameObject currentFriction;
        if (_colliderToFrictionObj.TryGetValue(collision.collider, out currentFriction))
        {
            UpdateForceObj(currentFriction,FrictionSetter(collision));
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject currentNormalForce;
        if (_colliderToNormalForceObj.TryGetValue(collision.collider, out currentNormalForce))
        {
            currentNormalForce.transform.localScale = Vector3.zero;
        }

        GameObject currentFriction;
        if (_colliderToFrictionObj.TryGetValue(collision.collider, out currentFriction))
        {
            currentFriction.transform.localScale = Vector3.zero;
        }
    }

    private Vector3 NormalForceSetter(Collision2D collision, bool isImpulse)
    {
        var normalDirection = (Vector3) collision.GetContact(0).normal.normalized;
        var magnitude = collision.GetContact(0).normalImpulse;
        if (collision.contactCount > 1)
        {
            magnitude += collision.GetContact(1).normalImpulse;
        }
        
        if(isImpulse) return magnitude * normalDirection;
        return magnitude / Time.fixedDeltaTime * normalDirection;
        
    }

    private Vector3 FrictionSetter(Collision2D collision)
    {
        var normalDirection = (Vector3) collision.GetContact(0).normal.normalized;
        var direction = Quaternion.AngleAxis(-90, Vector3.forward) * normalDirection;
        var magnitude = collision.GetContact(0).tangentImpulse;
        if (collision.contactCount > 1)
        {
            magnitude += collision.GetContact(1).tangentImpulse;
        }
        
        return magnitude / Time.fixedDeltaTime * direction;
    }
    
    private void SetGravity()
    {
        _gravityObj = InstantiateForceObj("G");
        _gravityObj.transform.localScale = new Vector3(1f, _rb.gravityScale * Physics2D.gravity.y * _rb.mass * 10f, 1f);
        var spriteRenderer = _gravityObj.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.gray;
    }
}
