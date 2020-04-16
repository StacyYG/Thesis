using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public abstract class Forces
{
    protected VectorLine Line;
    private readonly Transform _targetTransform;
    protected readonly Rigidbody2D Rb;
    private Vector2 _vector;

    public Vector2 Vector
    {
        get => _vector;
    }


    protected Forces(GameObject gameObject)
    {
        _targetTransform = gameObject.transform;
        Rb = gameObject.GetComponent<Rigidbody2D>();
        Line = new VectorLine("", new List<Vector3> {Vector2.zero, Vector2.zero}, Services.GameCfg.lineWidth);
        Services.TotalLineNumber++;
        Line.drawTransform = _targetTransform;
    }

    public void Destroy()
    {
        VectorLine.Destroy(ref Line);
    }

    public void SetVector(Vector2 forceVector)
    {
        _vector = forceVector;
        Vector2 pureForceVector = _targetTransform.InverseTransformVector(forceVector);
        Line.points3[0] = pureForceVector;
    }

    public void Draw()
    {
        Line.Draw();
    }

    public abstract void Update();
}

public class Gravity : Forces
{
    public Gravity(GameObject gameObject) : base(gameObject)
    {
        Line.name = "Gravity";
        Line.endCap = "fullArrow";
        Line.color = Color.gray;
        var vector = new Vector2(0f, Rb.gravityScale * Physics2D.gravity.y * Rb.mass);
        SetVector(vector);
    }
    public override void Update() {}
}

public class PlayerForce : Forces
{
    private ControllerSquare _controllerSquare;
    public PlayerForce(GameObject gameObject) : base(gameObject)
    {
        Line.name = "PlayerForce";
        Line.endCap = "fullArrow";
        _controllerSquare = Services.ControllerSquare;
        Line.color = Services.GameCfg.currentNetForceColor;
    }
    public override void Update()
    {
        SetVector(_controllerSquare.PlayerForce);
    }
    
}

public class NormalForce : Forces
{
    public NormalForce(GameObject gameObject, Collision2D other, int index) : base(gameObject)
    {
        Line.endCap = "fullArrow";
        Line.name = "N" + index;
        Line.color = other.gameObject.GetComponent<SpriteRenderer>().color;
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
        Line.Draw();
    }

    public void Reset()
    {
        SetVector(Vector2.zero);
        Line.Draw();
        _prevNormalForceSize = 0f;
    }
}

public class Friction : Forces
{
    public Friction(GameObject gameObject, Collision2D other, int index) : base(gameObject)
    {
        Line.endCap = "fullArrow";
        Line.name = "f" + index;
        Line.color = other.gameObject.GetComponent<SpriteRenderer>().color;
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
        Line.Draw();
    }
    public void Reset()
    {
        SetVector(Vector2.zero);
        Line.Draw();
        _prevFrictionSize = 0f;
    }
}
