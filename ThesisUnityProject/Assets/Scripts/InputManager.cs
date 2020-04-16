using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    private bool _useTouch = false;
    public bool mouseStayOnCtrlSqr { get; private set; }

    private int _fingerOnCtrlSqr = -1;
    public void Update()
    {
        if (_useTouch)
        {
            foreach (var touch in Input.touches)
            {
                if (touch.fingerId > 1) return;
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        var hit = Physics2D.Raycast(Services.MainCamera.ScreenToWorldPoint(touch.position), Vector2.zero,
                            Mathf.Infinity, ~5);
                        if (!ReferenceEquals(hit.collider, null))
                        {
                            if (hit.collider.gameObject.CompareTag("ControllerSquare"))
                            {
                                _fingerOnCtrlSqr = touch.fingerId;
                                Services.ControllerSquare.OnMouseOrTouchDown();
                            }
        
                            if (hit.collider.gameObject.CompareTag("CancelButton"))
                            {
                                if (Services.CancelButton.Respond)
                                    Services.ControllerSquare.ResetPlayerForce();
                            }
                        }
                        break;
                    case TouchPhase.Moved:
                        if (touch.fingerId == _fingerOnCtrlSqr)
                            Services.ControllerSquare.UpdateCurrentPlayerForce(_toWorldUnits(touch.position));
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == _fingerOnCtrlSqr)
                        {
                            Services.ControllerSquare.OnMouseOrTouchUp();
                            _fingerOnCtrlSqr = -1;
                        }
                        break;
                }
            }
        } 
        
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(Services.MainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                    Mathf.Infinity, ~5);
                if (!ReferenceEquals(hit.collider, null))
                {
                    if (hit.collider.gameObject.CompareTag("ControllerSquare"))
                    {
                        mouseStayOnCtrlSqr = true;
                        Services.ControllerSquare.OnMouseOrTouchDown();
                    }

                    if (hit.collider.gameObject.CompareTag("CancelButton"))
                    {
                        if (Services.CancelButton.Respond)
                        {
                            mouseStayOnCtrlSqr = false;
                            Services.ControllerSquare.ResetPlayerForce();
                        }
                    }
                }
            }
            if (Input.GetMouseButton(0))
                if (mouseStayOnCtrlSqr)
                    Services.ControllerSquare.UpdateCurrentPlayerForce(_toWorldUnits(Input.mousePosition));
                
            if (Input.GetMouseButtonUp(0))
                if (mouseStayOnCtrlSqr)
                {
                    mouseStayOnCtrlSqr = false;
                    Services.ControllerSquare.OnMouseOrTouchUp();
                }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Services.CancelButton.Respond)
                {
                    mouseStayOnCtrlSqr = false;
                    Services.ControllerSquare.ResetPlayerForce();
                }
            }
        }
    }

    private Vector2 _toWorldUnits(Vector3 inputPosition)
    {
        var worldPosition = Services.MainCamera.ScreenToWorldPoint(inputPosition);
        return new Vector2(worldPosition.x, worldPosition.y);
    }
}