using System.Collections;
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
    protected readonly float vectorMultiplier = Services.GameCfg.vectorMultiplier;
    
    protected Force(GameObject gameObject)
    {
        _targetTransform = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody2D>();
        line = new VectorLine("", new List<Vector3> {Vector2.zero, Vector2.zero}, Services.GameCfg.lineWidth * Screen.height / 1080f);
        line.drawTransform = _targetTransform;
        Services.Forces.Add(this);
    }

    public void Destroy()
    {
        VectorLine.Destroy(ref line);
    }

    public void SetVector(Vector2 forceVector)
    {
        _vector = forceVector;
        Vector2 pureForceVector = _targetTransform.InverseTransformVector(forceVector);
        pureForceVector *= vectorMultiplier;
        line.points3[0] = pureForceVector;
    }

    public void Draw()
    {
        line.Draw();
    }

    public abstract void Update();
}

public class Gravity : Force
{
    public bool ignore, use;
    private Vector2 _vector;
    public Gravity(GameObject gameObject) : base(gameObject)
    {
        line.name = "Gravity";
        line.endCap = "fullArrow";
        line.color = Color.gray;
        _vector = new Vector2(0f, Physics2D.gravity.y * rb.mass);
    }

    public override void Update()
    {
        if (ignore)
            return;
        if(use) 
            SetVector(_vector);
        else
            SetVector(Vector2.zero);
    }
}

public class PlayerForce : Force
{
    public PlayerForce(GameObject gameObject) : base(gameObject)
    {
        line.name = "PlayerForce";
        line.endCap = "fullArrow";
        line.color = Services.GameCfg.currentNetForceColor;
    }
    public override void Update()
    {
        SetVector(Services.ControllerSquare.PlayerForce);
    }
}

public class NormalForce : Force
{
    public NormalForce(GameObject gameObject, Collision2D other, int index) : base(gameObject)
    {
        line.endCap = "fullArrow";
        line.name = "N" + index;
        line.color = other.gameObject.GetComponent<SpriteRenderer>().color;
    }
    
    public override void Update() {}

    private float _prevNormalForceSize;
    
    public void Change(Collision2D other)
    {
        var direction = other.GetContact(0).normal.normalized;
        var size = other.GetContact(0).normalImpulse / Time.fixedDeltaTime;
        if (other.contactCount > 1)
        {
            size += other.GetContact(1).normalImpulse / Time.fixedDeltaTime;
        }
        
        var lerpSize = Mathf.Lerp(_prevNormalForceSize, size, 0.3f);
        _prevNormalForceSize = lerpSize;
        SetVector(lerpSize * direction);
    }

    public void Reset()
    {
        SetVector(Vector2.zero);
        _prevNormalForceSize = 0f;
    }
}

public class Friction : Force
{
    public Friction(GameObject gameObject, Collision2D other, int index) : base(gameObject)
    {
        line.endCap = "fullArrow";
        line.name = "f" + index;
        line.color = other.gameObject.GetComponent<SpriteRenderer>().color;
    }
    public override void Update() {}

    private float _prevFrictionSize;
    
    public void Change(Collision2D other)
    {
        var normalDirection = other.GetContact(0).normal.normalized;
        var direction = Quaternion.AngleAxis(-90, Vector3.forward) * normalDirection;
        var size = other.GetContact(0).tangentImpulse / Time.fixedDeltaTime;
        if (other.contactCount > 1)
        {
            size += other.GetContact(1).tangentImpulse / Time.fixedDeltaTime;
        }

        var lerpSize = Mathf.Lerp(_prevFrictionSize, size, 0.3f);
        _prevFrictionSize = lerpSize;
        SetVector(lerpSize * direction);
    }
    public void Reset()
    {
        SetVector(Vector2.zero);
        _prevFrictionSize = 0f;
    }
}
