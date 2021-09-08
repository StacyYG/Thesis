using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gate : MonoBehaviour
{
    private Dictionary<Rigidbody2D, float> _lastVelocity;
    private List<TrackVelocity> _trackVelocities;
    public bool isDetect = true;
    public float impulseMultiplier;
    private bool _waitingToFire;
    [SerializeField] private Vector3 throwPosition = new Vector3(-10f, 0f, 0f);
    private Vector2 _gateSize;

    // Start is called before the first frame update
    void Start()
    {
        _trackVelocities = new List<TrackVelocity>();
        _gateSize = GetComponent<BoxCollider2D>().size;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDetect) return;
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
        if (!isDetect) return;

        foreach (var trackV in _trackVelocities)
        {
            if (trackV.rb.velocity != trackV.enterVelocity)
            {
                if (trackV.enterVelocity.magnitude > Mathf.Epsilon) // When the tracked object enters from outside the gate
                {
                    trackV.transform.position = trackV.enterPosition + new Vector3(trackV.enterPosition.x - transform.position.x, 0f, 0f);
                }
                else // When the tracked object is a still object in the gate from the beginning
                {
                    trackV.transform.position =
                        trackV.enterPosition + _gateSize.magnitude * 0.5f * Random.insideUnitSphere.normalized;
                }
                
                trackV.rb.velocity = Vector2.zero;
                if (trackV.gameObject.CompareTag("TargetSquare"))
                {
                    Services.TargetSquare.isHurt = true;
                }
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
            Services.EventManager.Fire(new LoseLife());
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
}
