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
    private Dictionary<Collider2D, GameObject> colliderToNormalForceObj;

    
    // Start is called before the first frame update
    void Start()
    {
        _playerForceObj = InstantiateForceObj("playerForceObj");
        _rb = GetComponent<Rigidbody2D>();
        colliderToNormalForceObj = new Dictionary<Collider2D, GameObject>();
    }

    private void Update()
    {
        UpdateForceObj(_playerForceObj, ServiceLocator.ControllerSquare.PlayerForce());
    }

    // Update is called once per frame
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
        if (!colliderToNormalForceObj.TryGetValue(collision.collider, out newNormalForceObj))
        {
            newNormalForceObj = InstantiateForceObj("N" + colliderToNormalForceObj.Count);
            colliderToNormalForceObj.Add(collision.collider, newNormalForceObj);
        }
        
        UpdateForceObj(newNormalForceObj, NormalForceSetter(collision,true));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject currentNormalForce;
        if (colliderToNormalForceObj.TryGetValue(collision.collider, out currentNormalForce))
        {
            UpdateForceObj(currentNormalForce, NormalForceSetter(collision, false));
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject currentNormalForce;
        if (colliderToNormalForceObj.TryGetValue(collision.collider, out currentNormalForce))
        {
            currentNormalForce.transform.localScale = Vector3.zero;
        }
    }

    private Vector3 NormalForceSetter(Collision2D collision, bool isImpulse)
    {
        var midPoint = (Vector3)(collision.GetContact(0).point + collision.GetContact(1).point) / 2f;
        var direction = (transform.position - midPoint).normalized;
        var directionDotY = Vector3.Dot(direction, Vector3.up);
        Vector3 newDirection;
        if (Mathf.Abs(directionDotY) > 0.5f)
        {
            newDirection = new Vector3(0f, direction.y,0f);
        }
        else
        {
            newDirection = new Vector3(direction.x, 0f, 0f);
        }
        var contactPoint0 = collision.GetContact(0);
        if(isImpulse) return contactPoint0.normalImpulse * newDirection;
        return (contactPoint0.normalImpulse / Time.fixedDeltaTime) * newDirection;
    }
}
