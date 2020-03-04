using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSquare : MonoBehaviour
{
    private GameObject _currentLine;
    private GameObject _currentNetForceLine;
    private GameObject _previousNetForceLine;
    private Vector3 _currentForceVector;
    private Vector3 _netForceVector;
    public bool holdingMouse;



    // Start is called before the first frame update
    void Start()
    {
        _currentNetForceLine = InstantiateLine(transform.position, "currentNetForce");
        _previousNetForceLine = InstantiateLine(transform.position, "previousNetForce");
    }

    // Update is called once per frame
    void Update()
    {
        if (holdingMouse)
        {
            SetCurrentLineEnd(_currentLine, MousePosition());
            UpdateForceObj(_currentNetForceLine,PlayerForce());
        }

    }

    private void OnMouseDown()
    {
        holdingMouse = true;
        if (ReferenceEquals(_currentLine, null))
        { 
            _currentLine = InstantiateLine(transform.position, "forceBeingDrawn");
        }
        else
        {
            _currentLine.SetActive(true);
        }
        _previousNetForceLine.transform.localScale = _currentNetForceLine.transform.localScale;
        _previousNetForceLine.transform.up = _currentNetForceLine.transform.up;
        _previousNetForceLine.SetActive(true);

    }



    private void OnMouseUp()
    {
        holdingMouse = false;
        _netForceVector += _currentForceVector;
        _currentLine.SetActive(false);
        _previousNetForceLine.SetActive(false);

    }
    
    private GameObject InstantiateLine(Vector3 startPosition, string name)
    {
        var line = Instantiate(Resources.Load<GameObject>("square10"));
        var spriteRenderer = line.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = ServiceLocator.GameController.orderInLayer;
        ServiceLocator.GameController.orderInLayer++;
        line.transform.position = startPosition;
        line.transform.localScale = Vector3.zero;
        line.transform.name = name;
        line.SetActive(true);
        return line;
    }

    private void SetCurrentLineEnd(GameObject lineGameObject, Vector3 endPosition)
    {
        if(ReferenceEquals(lineGameObject,null)) return;
        var toDraw = endPosition - lineGameObject.transform.position;
        lineGameObject.transform.localScale = new Vector3(1f, toDraw.magnitude * 10f, 1f);
        lineGameObject.transform.up = toDraw.normalized;
        _currentForceVector = toDraw;
    }
    
    private void UpdateForceObj(GameObject forceObj, Vector3 forceVector)
    {
        forceObj.transform.localScale = new Vector3(1f, forceVector.magnitude * 10f, 1f);
        forceObj.transform.up = forceVector.normalized;
    }
    
    private Vector3 MousePosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -ServiceLocator.MyCamera.transform.position.z;
        return ServiceLocator.MyCamera.ScreenToWorldPoint(mousePos);
    }

    public Vector3 PlayerForce()
    {
        if (holdingMouse) return _netForceVector + _currentForceVector;
        return _netForceVector;
    }
}
