using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObj : MonoBehaviour
{
    public float smallestY = 0.2f;
    public Color restColor;
    public Color pressedColor;
    private SpriteRenderer _sr;
    private Vector3 _restScale;
    private ContactPoint2D[] _contacts = new ContactPoint2D[4];
    public float k;
    public float threshold = 0.3f;
    private bool _inCollision;
    public float tiredRecoverTime = 1f;
    private bool _isTired;
    private float _tiredTimer;

    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _restScale = transform.localScale;
    }

    private void Update()
    {
        if (!_inCollision)
        {
            if (_isTired)
            {
                _tiredTimer += Time.deltaTime;
                if (_tiredTimer >= tiredRecoverTime)
                {
                    _tiredTimer = 0f;
                    _isTired = false;
                }
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _restScale, 0.1f);
                _sr.color = Color.Lerp(_sr.color, restColor, 0.1f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _inCollision = true;
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        var contactNum = other.GetContacts(_contacts);
        float pressureSize = 0f;
        for (int i = 0; i < contactNum; i++)
        {
            pressureSize += _contacts[i].normalImpulse;
        }
        
        var y = _restScale.y;
        y *= 1f - k * pressureSize;
        transform.localScale = new Vector3(_restScale.x, Mathf.Max(y, smallestY), _restScale.z);
        _sr.color = Color.Lerp(pressedColor, restColor, y);
        if (transform.localScale.y < threshold && !_isTired)
        {
            //Services.EventManager.Fire(new ButtonObjPressed());
            _isTired = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _inCollision = false;
    }
    
}

public class ButtonObjPressed : AGPEvent{}

public class ButtonObjReleased: AGPEvent{}
