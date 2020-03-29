using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level0 : MonoBehaviour
{
    public float speed = 2f;

    public void Awake()
    {
        Init();
    }

    private void Init()
    {
        Services.MyCamera = Camera.main;
        Services.ControllerSquare = GameObject.FindGameObjectWithTag("ControllerSquare")
            .GetComponent<ControllerSquare>();
        Services.TargetSquare = GameObject.FindGameObjectWithTag("TargetSquare").GetComponent<TargetSquare>();
        Services.CameraController = new CameraController(Services.MyCamera, true, Services.TargetSquare.transform);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        var rb = Services.TargetSquare.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = speed * Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        Services.CameraController.Update();
    }
    
}

