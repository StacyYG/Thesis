using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    private bool _useTouch = false;
    private bool _mouseStayOnCtrlSqr;

    private int _fingerOnCtrlSqr = -1;
    private RaycastHit2D[] results = new RaycastHit2D[1];
    
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
                        var hit = Physics2D.Raycast(Services.MainCamera.ScreenToWorldPoint(touch.position), Vector3.forward,
                            Mathf.Infinity, LayerMask.GetMask("Raycast"));
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
                var countHit = Physics2D.RaycastNonAlloc(Services.MainCamera.ScreenToWorldPoint(Input.mousePosition),
                    Vector3.forward, results, Mathf.Infinity, LayerMask.GetMask("Raycast"));
                var hit = results[0];
                if (!ReferenceEquals(hit.collider, null))
                {
                    if (hit.collider.gameObject.CompareTag("ControllerSquare"))
                    {
                        _mouseStayOnCtrlSqr = true;
                        Services.ControllerSquare.OnMouseOrTouchDown();
                    }

                    if (hit.collider.gameObject.CompareTag("CancelButton"))
                    {
                        if (Services.CancelButton.Respond)
                        {
                            _mouseStayOnCtrlSqr = false;
                            Services.ControllerSquare.ResetPlayerForce();
                        }
                    }

                    if (hit.collider.gameObject.CompareTag("GravityButton"))
                    {
                        Services.GravityButton.GravitySwitch();
                    }
                }
            }
            if (Input.GetMouseButton(0))
                if (_mouseStayOnCtrlSqr)
                    Services.ControllerSquare.UpdateCurrentPlayerForce(_toWorldUnits(Input.mousePosition));

            if (Input.GetMouseButtonUp(0))
            {
                if (_mouseStayOnCtrlSqr)
                {
                    _mouseStayOnCtrlSqr = false;
                    Services.ControllerSquare.OnMouseOrTouchUp();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Services.CancelButton.Respond)
                {
                    _mouseStayOnCtrlSqr = false;
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