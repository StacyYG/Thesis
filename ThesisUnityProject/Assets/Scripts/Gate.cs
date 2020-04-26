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
        var trackV = new TrackVelocity();
        trackV.gameObject = other.gameObject;
        trackV.rb = other.gameObject.GetComponent<Rigidbody2D>();
        trackV.enterVelocity = trackV.rb.velocity;
        trackV.collider = other;
        trackV.hasBeenPushed = false;
        if (trackV.gameObject.CompareTag("TargetSquare"))
        {
            Debug.Log("has been pushed: " + trackV.hasBeenPushed);
        }
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
                var impulseSize = impulseMultiplier * trackV.rb.mass * trackV.enterVelocity.x;
                trackV.rb.AddForce(new Vector2(-impulseSize, 0f), ForceMode2D.Impulse);
                if (trackV.gameObject.CompareTag("TargetSquare"))
                {
                    Services.EventManager.Fire(new LoseLife());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        for (int i = 0; i < _trackVelocities.Count; i++)
        {
            if (ReferenceEquals(_trackVelocities[i].collider, other))
            {
                _trackVelocities.Remove(_trackVelocities[i]);
            }
        }
    }
}

public class Success : AGPEvent{}

public class TrackVelocity
{
    public Rigidbody2D rb;
    public Vector2 enterVelocity;
    public Collider2D collider;
    public GameObject gameObject;
    public bool hasBeenPushed;
}
