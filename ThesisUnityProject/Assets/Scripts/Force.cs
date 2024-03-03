using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public abstract class Force
{
    protected VectorLine line;
    private readonly Transform _targetTransform;
    protected readonly Rigidbody2D rb;
    private Vector2 _vector;
    public Vector2 Vector => _vector;
    private readonly float _vectorMultiplier = Services.GameCfg.vectorMultiplier;
    
    protected Force(GameObject gameObject) // Gameobject the force is acting upon
    {
        _targetTransform = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody2D>();
        line = new VectorLine("", new List<Vector3> {Vector2.zero, Vector2.zero},
            Services.GameCfg.forceLineWidth * Screen.height / 1080f);
        line.drawTransform = _targetTransform;
        Services.Forces.Add(this);
    }

    public void DestroyLine() // Destroy the vector line
    {
        VectorLine.Destroy(ref line);
    }

    public void HideLine(bool isHide) // Set the vector line inactive
    {
        line.active = !isHide;
    }

    protected void SetVector(Vector2 forceVector) // Set the force to the given value, update the line in the background (but not draw it yet)
    {
        _vector = forceVector;
        Vector2 pureForceVector = _targetTransform.InverseTransformVector(forceVector);
        pureForceVector *= _vectorMultiplier;
        line.points3[0] = pureForceVector;
    }

    public void Draw() // Draw the line in LateUpdate
    {
        line.Draw();
    }

    public abstract void Update();
}

public class Gravity : Force // A constant force pointing down
{
    public Gravity(GameObject gameObject, Color color) : base(gameObject)
    {
        line.name = "Gravity_" + gameObject.name;
        line.endCap = "fullArrow";
        line.color = color;
    }
    
    public override void Update()
    {
        SetVector(new Vector2(0f, Physics2D.gravity.y * rb.mass * rb.gravityScale));
    }
}

public class PlayerForce : Force
{
    public PlayerForce(GameObject gameObject) : base(gameObject)
    {
        line.name = "PlayerForce";
        line.endCap = "fullArrow";
        line.color = Services.GameCfg.currentForceColor;
    }
    public override void Update()
    {
        SetVector(Services.ControlButton.PlayerForce);
    }
}

public class NormalForce : Force // Created when the target square contacts with a "BarrierObject" surface
{
    public NormalForce(GameObject gameObject, Collision2D other, int index) : base(gameObject)
    {
        line.endCap = "fullArrow";
        line.name = "N" + index;
        line.color = other.gameObject.GetComponent<SpriteRenderer>().color; // Normal force line color is the same as the contacting surface
    }
    
    public override void Update() {}

    private float _prevNormalForceSize;
    
    public void Change(Collision2D other)
    {
        var direction = other.GetContact(0).normal.normalized;
        var size = other.GetContact(0).normalImpulse / Time.fixedDeltaTime; // force = impulse / time
        
        if (other.contactCount > 1) // There will be at most two contact points at the same time when a square contacts another object
        {
            size = other.GetContact(0).normalImpulse / Time.fixedDeltaTime +
                   other.GetContact(1).normalImpulse / Time.fixedDeltaTime;
        }
        
        var lerpedSize = Mathf.Lerp(_prevNormalForceSize, size, 0.3f);
        SetVector(lerpedSize * direction);
        _prevNormalForceSize = lerpedSize;
    }

    public void Reset()
    {
        SetVector(Vector2.zero);
        _prevNormalForceSize = 0f;
    }
}

public class Friction : Force // Created when the target square contacts with a "BarrierObject" surface
{
    public Friction(GameObject gameObject, Collision2D other, int index) : base(gameObject)
    {
        line.endCap = "fullArrow";
        line.name = "f" + index;
        line.color = other.gameObject.GetComponent<SpriteRenderer>().color; // Friction force line color is the same as the contacting surface
    }
    public override void Update() {}

    private float _prevFrictionSize;
    
    public void Change(Collision2D other)
    {
        var normalDirection = other.GetContact(0).normal.normalized;
        var direction = Quaternion.AngleAxis(-90, Vector3.forward) * normalDirection; // Get friction force direction by rotating the normal for 90 degrees
        var size = other.GetContact(0).tangentImpulse / Time.fixedDeltaTime; // force = impulse / time
        
        if (other.contactCount > 1) // There will be at most two contact points at the same time when a square contacts another object
        {
            size = other.GetContact(0).tangentImpulse / Time.fixedDeltaTime +
                   other.GetContact(1).tangentImpulse / Time.fixedDeltaTime;
        }

        var lerpedSize = Mathf.Lerp(_prevFrictionSize, size, 0.3f);
        SetVector(lerpedSize * direction);
        _prevFrictionSize = lerpedSize;
    }
    public void Reset()
    {
        SetVector(Vector2.zero);
        _prevFrictionSize = 0f;
    }
}
