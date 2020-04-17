using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class TargetSquare : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private PlayerForce _playerForce;
    private Dictionary<Collider2D, NormalForce> _normalForces;
    private Dictionary<Collider2D, Friction> _frictions;
    private Gravity _gravity;
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
        
        _playerForce = new PlayerForce(gameObject);

        if (Mathf.Abs(_rb.gravityScale) > Mathf.Epsilon)
        {
            _gravity = new Gravity(gameObject);
        }
        
        Services.EventManager.Register<LoseLife>(OnLoseLife);
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<LoseLife>(OnLoseLife);
    }

    public void OnFixedUpdate()
    {
        _playerForce.Update();
        _rb.AddForce(_playerForce.Vector);
    }

    public void OnUpdate()
    {
        if (_isHurt)
        {
            _hurtTimer += Time.deltaTime;
            _sr.color = Color.Lerp(Services.GameCfg.hurtColor, Services.GameCfg.liveColor, _hurtTimer / recoverTime);
            if (_hurtTimer >= recoverTime) _isHurt = false;
        }
    }

    public void OnLateUpdate()
    {
        _playerForce.Draw();
        if (Mathf.Abs(_rb.gravityScale) > Mathf.Epsilon)
            _gravity.Draw();
        
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

        if (other.gameObject.CompareTag("HazardObject"))
        {
            if(_isHurt) return;
            Services.EventManager.Fire(new LoseLife());
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("NewLife"))
        {
            Services.EventManager.Fire(new GainLife(other.gameObject));
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            Services.EventManager.Fire(new Success());
        }
    }

    private void OnLoseLife(AGPEvent e)
    {
        _rb.velocity = Vector2.zero;
        _isHurt = true;
        _hurtTimer = 0f;
    }
}
