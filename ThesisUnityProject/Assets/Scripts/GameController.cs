using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public GameObject targetPlayerSquare;
    private Vector3 mousePosition;
    private int orderInLayer =1;
    private GameObject _currentLine;
    public bool mouseIsOutOfSquare;
    public bool holdingMouse;
    private void Awake()
    {
        initializeServices();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -ServiceLocator.myCamera.transform.position.z;
        mousePosition = ServiceLocator.myCamera.ScreenToWorldPoint(mousePos);
        if (mouseIsOutOfSquare && holdingMouse && !ReferenceEquals(_currentLine,null))
        {
            SetLineEnd(mousePosition);
        }
    }

    private void initializeServices()
    {
        ServiceLocator.myCamera = Camera.main;
    }

    public void DrawVector()
    {
        _currentLine = Instantiate(Resources.Load<GameObject>("square10"));
        var spriteRenderer = _currentLine.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sortingOrder = orderInLayer;
        orderInLayer++;
        _currentLine.transform.position = targetPlayerSquare.transform.position;
        SetLineEnd(mousePosition);
    }

    private void SetLineEnd(Vector3 endPosition)
    {
        var toDraw = endPosition - _currentLine.transform.position;
        _currentLine.transform.localScale = new Vector3(1f, toDraw.magnitude * 10f, 1f);
        _currentLine.transform.up = toDraw.normalized;
    }
}
