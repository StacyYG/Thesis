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
        if (_isTired)
        {
            if (transform.localScale.y > threshold)
            {
                _isTired = false;
                Services.EventManager.Fire(new ButtonObjPressed(false));
            }
        }

        _sr.color = Color.Lerp(pressedColor, restColor,
            (transform.localScale.y - smallestY) / (_restScale.y - smallestY));
    }

    private void FixedUpdate()
    {
        if (transform.localScale.y < _restScale.y)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _restScale, 0.01f);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if(!other.gameObject.CompareTag("Comet"))
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
            if (transform.localScale.y < threshold && !_isTired)
            {
                Services.EventManager.Fire(new ButtonObjPressed(true));
                _isTired = true;
            }
        }
    }
}

public class ButtonObjPressed : AGPEvent
{
    public bool isPressed;

    public ButtonObjPressed(bool pressed)
    {
        isPressed = pressed;
    }
}
