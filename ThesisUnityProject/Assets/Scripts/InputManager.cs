using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public List<KeyCode> KeysDown { get; private set; }
    public List<KeyCode> KeysUp { get; private set; }
    public List<KeyCode> KeysStay { get; private set; }

    public bool mouseDown  { get; private set; }
    public bool mouseUp  { get; private set; }
    public bool mouseStay { get; private set; }

    public Vector2 MousePositionWorldUnits { get; private set; }

    private int _fingerTouching = -1;
    public void Update()
    {
        mouseDown = false;
        mouseUp = false;
        mouseStay = false;
        
        if (Input.touchSupported)
        {
            foreach (var touch in Input.touches)
            {
                if (touch.fingerId > 1) return;
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero,
                            Mathf.Infinity, 5);
                        if (hit.collider.gameObject.CompareTag("ControllerSquare"))
                        {
                            mouseDown = true;
                            MousePositionWorldUnits = _toWorldUnits(touch.position);
                            //call controller on mouse down
                        }

                        if (hit.collider.gameObject.CompareTag("CancelButton"))
                        {
                            //call cancel force
                        }
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        mouseStay = true;
                        MousePositionWorldUnits = _toWorldUnits(touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        mouseUp = true;
                        break;
                }
            }
        } 
        else
        {
            if (Input.GetMouseButtonDown(0))
                mouseDown = true;
            if (Input.GetMouseButton(0))
                mouseStay = true;
            if (Input.GetMouseButtonUp(0))                        
                mouseUp = true;
            
            MousePositionWorldUnits = _toWorldUnits(Input.mousePosition);
        }
        
        KeysDown = new List<KeyCode>();
        KeysUp = new List<KeyCode>();
        KeysStay = new List<KeyCode>();
        
        foreach(KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keycode))
                KeysDown.Add(keycode);
            if (Input.GetKey(keycode))
                KeysStay.Add(keycode);
            if (Input.GetKeyUp(keycode))
                KeysStay.Add(keycode);
        }
    }

    private Vector2 _toWorldUnits(Vector3 inputPosition)
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);
        return new Vector2(worldPosition.x, worldPosition.y);
    }
}