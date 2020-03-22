﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Services
{

    private static Camera _myCamera;

    public static Camera MyCamera
    {
        get
        {
            if (_myCamera == null)
            {
                _myCamera = Camera.main;
            }

            return _myCamera;
        }
        set => _myCamera = value;
    }

    private static GameController _gameController;

    public static GameController GameController
    {
        get
        {
            if (_gameController == null)
            {
                _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            }

            return _gameController;
        }
        set => _gameController = value;
    }

    private static ControllerSquare _controllerSquare;

    public static ControllerSquare ControllerSquare
    {
        get
        {
            if (_controllerSquare == null)
            {
                _controllerSquare = GameObject.FindGameObjectWithTag("ControllerSquare")
                    .GetComponent<ControllerSquare>();
            }

            return _controllerSquare;
        }
        set => _controllerSquare = value;
    }

    private static TargetSquare _targetSquare;

    public static TargetSquare TargetSquare
    {
        get
        {
            if (_targetSquare == null)
            {
                _targetSquare = GameObject.FindGameObjectWithTag("TargetSquare").GetComponent<TargetSquare>();
            }

            return _targetSquare;
        }
        
        set => _targetSquare = value;
    }
}
