using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class TargetSquare : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Dictionary<Collider2D, NormalForce> _normalForces;
    private Dictionary<Collider2D, Friction> _frictions;
    public float recoverTime;
    private bool _isHurt;
    private float _hurtTimer;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _normalForces = new Dictionary<Collider2D, NormalForce>();
        _frictions = new Dictionary<Collider2D, Friction>();
        new PlayerForce(gameObject);
        if (Mathf.Abs(_rb.gravityScale) > Mathf.Epsilon)
            new Gravity(gameObject);
        Services.EventManager.Register<LoseLife>(OnLoseLife);
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<LoseLife>(OnLoseLife);
        foreach (var force in Services.Forces)
            force.Destroy();
    }

    public void Update()
    {
        if (_isHurt)
        {
            _hurtTimer += Time.deltaTime;
            _sr.color = Color.Lerp(Services.GameCfg.hurtColor, Services.GameCfg.liveColor, _hurtTimer / recoverTime);
            if (_hurtTimer >= recoverTime) 
                _isHurt = false;
        }
    }

    //private List<Vector2> points = new List<Vector2>(); //for presentation
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            if (!_normalForces.ContainsKey(other.collider))
                _normalForces.Add(other.collider, new NormalForce(gameObject, other, _normalForces.Count));

            if (!_frictions.ContainsKey(other.collider))
                _frictions.Add(other.collider, new Friction(gameObject, other, _frictions.Count));
            
            //for presentation
            // Debug.Log("contact number: " + other.contactCount);
            // for (int i = 0; i < other.contactCount; i++)
            // {
            //     var relativePos = other.GetContact(i).point - (Vector2) transform.position;
            //     Debug.Log("contact point position: " + relativePos);
            //     points.Add(other.GetContact(i).point);
            // }
        }

        if (other.gameObject.CompareTag("HazardObject"))
        {
            if(_isHurt) return;
            Services.EventManager.Fire(new LoseLife());
            Destroy(other.gameObject);
        }
    }
    
    // private void OnDrawGizmos() //for presentation
    // {
    //     foreach (var point in points)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawCube(point, 0.3f * Vector3.one);
    //     }
    // }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            NormalForce normalForce;
            if (_normalForces.TryGetValue(other.collider, out normalForce))
                normalForce.Change(other);

            Friction friction;
            if (_frictions.TryGetValue(other.collider, out friction))
                friction.Change(other);
            
            //for presentation
            // Debug.Log("contact number: " + other.contactCount);
            // points = new List<Vector2>();
            // for (int i = 0; i < other.contactCount; i++)
            // {
            //     var relativePos = other.GetContact(i).point - (Vector2) transform.position;
            //     Debug.Log("contact point position: " + relativePos);
            //     points.Add(other.GetContact(i).point);
            // }
        }

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            NormalForce normalForce;
            if (_normalForces.TryGetValue(other.collider, out normalForce))
                normalForce.Reset();
            
            Friction friction;
            if (_frictions.TryGetValue(other.collider, out friction))
                friction.Reset();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            Services.EventManager.Fire(new Success());
        }
    }

    private void OnLoseLife(AGPEvent e)
    {
        //_rb.velocity = Vector2.zero;
        _isHurt = true;
        _hurtTimer = 0f;
    }
}
