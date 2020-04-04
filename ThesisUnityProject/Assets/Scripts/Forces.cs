using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public abstract class Forces
{
    protected VectorLine _line;
    private const float LineWidth = 35f;
    protected Transform _transform;
    protected Rigidbody2D _rb;

    protected Forces(GameObject gameObject)
    {
        _transform = gameObject.transform;
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _line = new VectorLine("", new List<Vector3> {Vector2.zero, Vector2.zero}, LineWidth);
        Services.TotalLineNumber++;
        _line.drawTransform = _transform;
    }

    public void Destroy()
    {
        VectorLine.Destroy(ref _line);
    }

    public void SetForceVector(Vector2 forceVector)
    {
        Vector2 pureForceVector = _transform.InverseTransformVector(forceVector);
        _line.points3[0] = pureForceVector;
        _line.Draw();
    }

    public abstract void Update();
}

public class Gravity : Forces
{
    public Gravity(GameObject gameObject) : base(gameObject)
    {
        _line.name = "Gravity";
        _line.endCap = "fullArrow";
        _line.color = Color.gray;
        var vector = new Vector2(0f, _rb.gravityScale * Physics2D.gravity.y * _rb.mass);
        SetForceVector(vector);
    }

    public override void Update()
    {
        _line.Draw();
    }
}

public class PlayerForce : Forces
{
    private ControllerSquare _controllerSquare;
    
    public PlayerForce(GameObject gameObject) : base(gameObject)
    {
        _line.name = "PlayerForce";
        _line.endCap = "fullArrow";
        _controllerSquare = Services.ControllerSquare;
        _line.color = _controllerSquare.currentNetForceColor;
    }
    public override void Update()
    {
        SetForceVector(_controllerSquare.PlayerForce);
    }
}

public class NormalForce : Forces
{
    public NormalForce(GameObject gameObject, Collision2D collision, int index) : base(gameObject)
    {
        _line.endCap = "fullArrow";
        _line.name = "N" + index;
        _line.color = collision.gameObject.GetComponent<SpriteRenderer>().color;
    }
    
    public override void Update() {}

    private float _prevNormalForceSize;
    
    public void Change(Collision2D collision)
    {
        var direction = collision.GetContact(0).normal.normalized;
        var size = collision.GetContact(0).normalImpulse / Time.fixedDeltaTime;
        if (collision.contactCount > 1)
        {
            size += collision.GetContact(1).normalImpulse / Time.fixedDeltaTime;
        }
        
        var lerpSize = Mathf.Lerp(_prevNormalForceSize, size, 0.3f);
        _prevNormalForceSize = lerpSize;
        SetForceVector(lerpSize * direction);
    }

    public void Reset()
    {
        _line.points3[0] = Vector2.zero;
        _line.Draw();
        _prevNormalForceSize = 0f;
    }
}

public class Friction : Forces
{
    public Friction(GameObject gameObject, Collision2D collision, int index) : base(gameObject)
    {
        _line.endCap = "fullArrow";
        _line.name = "f" + index;
        _line.color = collision.gameObject.GetComponent<SpriteRenderer>().color;
    }
    public override void Update() {}

    private float _prevFrictionSize;
    
    public void Change(Collision2D collision)
    {
        var normalDirection = collision.GetContact(0).normal.normalized;
        var direction = Quaternion.AngleAxis(-90, Vector3.forward) * normalDirection;
        var size = collision.GetContact(0).tangentImpulse / Time.fixedDeltaTime;
        if (collision.contactCount > 1)
        {
            size += collision.GetContact(1).tangentImpulse / Time.fixedDeltaTime;
        }

        var lerpSize = Mathf.Lerp(_prevFrictionSize, size, 0.3f);
        _prevFrictionSize = lerpSize;
        SetForceVector(lerpSize * direction);
    }
    public void Reset()
    {
        _line.points3[0] = Vector2.zero;
        _line.Draw();
        _prevFrictionSize = 0f;
    }
}
