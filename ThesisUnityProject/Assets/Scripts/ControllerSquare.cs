using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSquare : MonoBehaviour
{
    private GameObject _currentLine;
    private Vector3 _currentForceVector;
    private Vector3 _PreviousNetForceVector;
    public bool holdingMouse;


    
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (holdingMouse)
        {
            SetLineEnd(MousePosition());
        }
        
    }

    private void OnMouseDown()
    {
        holdingMouse = true;
        SetLineStart(transform.position);

    }



    private void OnMouseUp()
    {
        holdingMouse = false;
        _PreviousNetForceVector += _currentForceVector;

    }
    
    private void SetLineStart(Vector3 startPosition)
    {
        _currentLine = Instantiate(Resources.Load<GameObject>("square10"));
        var spriteRenderer = _currentLine.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sortingOrder = ServiceLocator.GameController.orderInLayer;
        ServiceLocator.GameController.orderInLayer++;
        _currentLine.transform.position = startPosition;
    }

    private void SetLineEnd(Vector3 endPosition)
    {
        if(ReferenceEquals(_currentLine,null)) return;
        var toDraw = endPosition - _currentLine.transform.position;
        _currentLine.transform.localScale = new Vector3(1f, toDraw.magnitude * 10f, 1f);
        _currentLine.transform.up = toDraw.normalized;
        _currentForceVector = toDraw;
    }

    private Vector3 MousePosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -ServiceLocator.MyCamera.transform.position.z;
        return ServiceLocator.MyCamera.ScreenToWorldPoint(mousePos);
    }

    public Vector3 netForceByPlayer()
    {
        if (holdingMouse) return _PreviousNetForceVector + _currentForceVector;
        return _PreviousNetForceVector;
    }
}
