using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Dictionary<Rigidbody2D, float> _lastVelocity;
    private List<TrackVelocity> _trackVelocities;
    private bool _waitingToFire;
    private Vector2 _gateSize;
    private ParticleSystem _particle;

    void Start()
    {
        _trackVelocities = new List<TrackVelocity>();
        _gateSize = GetComponent<BoxCollider2D>().size;
        _particle = GetComponent<ParticleSystem>();
        var shape = _particle.shape;
        shape.scale = _gateSize;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.attachedRigidbody) return;

        var trackV = new TrackVelocity();
        trackV.gameObject = other.gameObject;
        trackV.rb = other.attachedRigidbody;
        trackV.enterVelocity = trackV.rb.velocity;
        trackV.collider = other;
        trackV.transform = other.transform;
        trackV.enterPosition = other.transform.position;
        _trackVelocities.Add(trackV);
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        foreach (var trackV in _trackVelocities)
        {
            if (trackV.rb.velocity != trackV.enterVelocity)
                trackV.keptVelocity = false;

            if (trackV.keptVelocity) continue;

            // When the tracked item starts from within the gate
            var distance = trackV.enterPosition - transform.position;
            if (Mathf.Abs(distance.x) < _gateSize.x/2f - 0.01f && Mathf.Abs(distance.y) < _gateSize.y/2f - 0.01f)
                trackV.rb.velocity = 7f / distance.magnitude * distance.normalized;
            
            else // When the tracked item enters from outside
                trackV.rb.velocity = -trackV.enterVelocity;

            // if (trackV.enterVelocity.magnitude > Mathf.Epsilon) // When the tracked object enters from outside the gate
            // {
            //     trackV.transform.position = trackV.enterPosition + new Vector3(trackV.enterPosition.x - transform.position.x, 0f, 0f);
            // }
            // else // When the tracked object is a still object in the gate from the beginning
            // {
            //     trackV.rb.velocity = 30f * (trackV.enterPosition - transform.position);
            // }
                
            //trackV.rb.velocity = Vector2.zero;
            if (trackV.gameObject.CompareTag("TargetSquare"))
            {
                Services.TargetSquare.isHurt = true;
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        for (int i = 0; i < _trackVelocities.Count; i++)
            if (ReferenceEquals(_trackVelocities[i].collider, other))
                _trackVelocities.RemoveAt(i);
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            Services.EventManager.Fire(new Hurt());
        }
    }
}

public class TrackVelocity
{
    public Rigidbody2D rb;
    public Vector2 enterVelocity;
    public Collider2D collider;
    public GameObject gameObject;
    public Transform transform;
    public Vector3 enterPosition;
    public bool keptVelocity = true;
}
