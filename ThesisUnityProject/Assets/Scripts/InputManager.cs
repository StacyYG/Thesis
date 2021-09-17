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
                            if (hit.collider.gameObject.CompareTag("ControlButton"))
                            {
                                _fingerOnCtrlSqr = touch.fingerId;
                                Services.ControlButton.OnMouseOrTouchDown();
                            }
        
                            if (hit.collider.gameObject.CompareTag("CancelButton"))
                            {
                                Services.ControlButton.ResetPlayerForce();
                            }
                        }
                        break;
                    case TouchPhase.Moved:
                        if (touch.fingerId == _fingerOnCtrlSqr)
                            Services.ControlButton.UpdatePlayerForce(_toWorldUnits(touch.position));
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == _fingerOnCtrlSqr)
                        {
                            Services.ControlButton.OnMouseOrTouchUp();
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
                if (countHit > 0)
                {
                    var hit = results[0];
                    if (hit.collider.gameObject.CompareTag("ControlButton"))
                    {
                        _mouseStayOnCtrlSqr = true;
                        Services.ControlButton.OnMouseOrTouchDown();
                    }

                    if (hit.collider.gameObject.CompareTag("CancelButton"))
                    {
                        PressCancelButton();
                    }

                    if (hit.collider.gameObject.CompareTag("GravityButton"))
                        Services.GravityButton.GravitySwitch();
                }
            }
            
            if (Input.GetMouseButton(0))
                if (_mouseStayOnCtrlSqr)
                    Services.ControlButton.UpdatePlayerForce(_toWorldUnits(Input.mousePosition));

            if (Input.GetMouseButtonUp(0))
            {
                if (_mouseStayOnCtrlSqr)
                {
                    _mouseStayOnCtrlSqr = false;
                    Services.ControlButton.OnMouseOrTouchUp();
                }
            }
        }
    }

    public void PressCancelButton()
    {
        _mouseStayOnCtrlSqr = false;
        Services.ControlButton.ResetPlayerForce();
    }

    private Vector2 _toWorldUnits(Vector3 inputPosition)
    {
        var worldPosition = Services.MainCamera.ScreenToWorldPoint(inputPosition);
        return new Vector2(worldPosition.x, worldPosition.y);
    }
}