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
    public bool isHurt, showNormalForce = true, showFriction = true;
    private float _hurtTimer;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _normalForces = new Dictionary<Collider2D, NormalForce>();
        _frictions = new Dictionary<Collider2D, Friction>();
        new PlayerForce(gameObject);
    }

    private void OnDestroy()
    {
        foreach (var force in Services.Forces)
            force.DestroyLine();
    }

    public void Update()
    {
        if (isHurt)
        {
            _hurtTimer += Time.deltaTime;
            _sr.color = Color.Lerp(Services.GameCfg.hurtColor, Services.GameCfg.liveColor, _hurtTimer / recoverTime);
            if (_hurtTimer >= recoverTime)
            {
                isHurt = false;
                _hurtTimer = 0f;
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            if (showNormalForce)
            {
                if (!_normalForces.ContainsKey(other.collider))
                    _normalForces.Add(other.collider, new NormalForce(gameObject, other, _normalForces.Count));
            }

            if (showFriction)
            {
                if (!_frictions.ContainsKey(other.collider))
                    _frictions.Add(other.collider, new Friction(gameObject, other, _frictions.Count));
            }
        }

        if (other.gameObject.CompareTag("HazardObject"))
        {
            if(isHurt) return;
            Services.EventManager.Fire(new LoseLife());
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.CompareTag("Goal"))
            Services.EventManager.Fire(new Success());
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            if (showNormalForce)
            {
                NormalForce normalForce;
                if (_normalForces.TryGetValue(other.collider, out normalForce))
                    normalForce.Change(other);
            }

            if (showFriction)
            {
                Friction friction;
                if (_frictions.TryGetValue(other.collider, out friction))
                    friction.Change(other);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject"))
        {
            if (showNormalForce)
            {
                NormalForce normalForce;
                if (_normalForces.TryGetValue(other.collider, out normalForce))
                    normalForce.Reset();
            }

            if (showFriction)
            {
                Friction friction;
                if (_frictions.TryGetValue(other.collider, out friction))
                    friction.Reset();
            }
        }
    }
}

public class Success : AGPEvent{}
