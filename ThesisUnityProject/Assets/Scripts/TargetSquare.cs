using System.Collections.Generic;
using UnityEngine;

public class TargetSquare : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Dictionary<Collider2D, NormalForce> _normalForces; // collider of the object contacting the target square -- the normal force
    private Dictionary<Collider2D, Friction> _frictions; // collider of the object contacting the target square -- the friction force
    public float recoverTime; // length of time for the target square to recover from the hurt state
    public bool isHurt, showNormalForce = true, showFriction = true;
    private float _hurtTimer;
    
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _normalForces = new Dictionary<Collider2D, NormalForce>();
        _frictions = new Dictionary<Collider2D, Friction>();
        new PlayerForce(gameObject);
    }

    private void OnDestroy()
    {
        // Clean out the vector lines
        foreach (var force in Services.Forces)
            force.DestroyLine();
    }

    public void Update()
    {
        if (isHurt) // Recover from the hurt state
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
        if (other.gameObject.CompareTag("BarrierObject")) // A BarrierObject visualizes normal force and friction force when contacted.
        {
            // Register the normal force and the friction force when first contacting with the object
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
        
        if (other.gameObject.CompareTag("Goal"))
            Services.EventManager.Fire(new Success());
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("BarrierObject")) // Update all the registered normal forces and friction forces when staying contacting with the object
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
        if (other.gameObject.CompareTag("BarrierObject")) // Reset all the registered normal forces and friction forces when leaving the object
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

public class Success : AGPEvent{} // When player hits the goal
