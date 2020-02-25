using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquare : MonoBehaviour
{
    
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        ServiceLocator.gameController.holdingMouse = true;
        ServiceLocator.gameController.targetPlayerSquare = gameObject;
        ServiceLocator.gameController.DrawVector();
    }

    private void OnMouseExit()
    {
        ServiceLocator.gameController.mouseIsOutOfSquare = true;
    }

    private void OnMouseUp()
    {
        ServiceLocator.gameController.holdingMouse = false;
        
    }
    
}
