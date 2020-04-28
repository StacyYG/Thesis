using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Dictionary<Rigidbody2D, float> _lastVelocity;
    private List<TrackVelocity> _trackVelocities;
    public bool isDetect = true;
    public float impulseMultiplier;
    private bool _waitingToFire;

    // Start is called before the first frame update
    void Start()
    {
        _trackVelocities = new List<TrackVelocity>();
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
        _trackVelocities.Add(trackV);
    }

    int i = 0;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isDetect) return;

        foreach (var trackV in _trackVelocities)
        {
            if (trackV.rb.velocity != trackV.enterVelocity)
            {
                var impulse = impulseMultiplier * trackV.rb.mass * trackV.enterVelocity;
                trackV.rb.AddForce(-impulse, ForceMode2D.Impulse);
                Debug.Log(impulse);
                if (trackV.gameObject.CompareTag("TargetSquare"))
                    _waitingToFire = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        for (int i = 0; i < _trackVelocities.Count; i++)
            if (ReferenceEquals(_trackVelocities[i].collider, other))
                _trackVelocities.Remove(_trackVelocities[i]);

        if (other.gameObject.CompareTag("TargetSquare"))
        {
            if (_waitingToFire)
            {
                Services.EventManager.Fire(new LoseLife());
                _waitingToFire = false;
            }
        }
    }
}

public class TrackVelocity
{
    public Rigidbody2D rb;
    public Vector2 enterVelocity;
    public Collider2D collider;
    public GameObject gameObject;
}
