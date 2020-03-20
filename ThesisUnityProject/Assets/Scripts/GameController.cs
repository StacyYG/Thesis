using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public GameObject targetPlayerSquare;
    public int orderInLayer = 1;
    private void Awake()
    {
        InitializeServices();
    }

    // Start is called before the first frame update
    void Start()
    {
        targetPlayerSquare = GameObject.FindGameObjectWithTag("target");

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitializeServices()
    {
        Services.MyCamera = Camera.main;
    }


}
