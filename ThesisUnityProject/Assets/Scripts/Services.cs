﻿using System.Collections.Generic;
using UnityEngine;


public static class Services
{
    private static InputManager _input;

    public static InputManager Input
    {
        get
        { 
            Debug.Assert(_input != null);
            return _input;
        }
        set => _input = value;
    }

    private static Camera _myCamera;

    public static Camera MainCamera
    {
        get
        {
            Debug.Assert(_myCamera != null);
            return _myCamera;
        }
        set { _myCamera = value; }
    }

    private static GameController _gameController;

    public static GameController GameController
    {
        get
        {
            Debug.Assert(_gameController != null);
            return _gameController;
        }
        set { _gameController = value; }
    }

    private static ControlButton _controlButton;

    public static ControlButton ControlButton
    {
        get
        {
            Debug.Assert(_controlButton != null);
            return _controlButton;
        }
        set { _controlButton = value; }
    }

    private static TargetSquare _targetSquare;

    public static TargetSquare TargetSquare
    {
        get
        {
            Debug.Assert(_targetSquare != null);
            return _targetSquare;
        }
        set { _targetSquare = value; }
    }

    private static CameraController _cameraController;

    public static CameraController CameraController
    {
        get
        { 
            Debug.Assert(_cameraController != null);
            return _cameraController;
        }
        set { _cameraController = value; }
    }
    
    private static EventManager _eventManager;
    public static EventManager EventManager
    {
        get
        { 
            Debug.Assert(_eventManager != null);
            return _eventManager;
        }
        set => _eventManager = value;
    }

    private static CancelButton _cancelButton;

    public static CancelButton CancelButton
    {
        get
        { 
            Debug.Assert(_cancelButton != null);
            return _cancelButton;
        }
        set => _cancelButton = value;
    }

    private static Gate _gate;

    public static Gate Gate
    {
        get
        { 
            Debug.Assert(_gate != null);
            return _gate;
        }
        set => _gate = value;
    }

    private static GameCfg _gameCfg;

    public static GameCfg GameCfg
    {
        get
        { 
            Debug.Assert(_gameCfg != null);
            return _gameCfg;
        }
        set => _gameCfg = value;
    }

    private static List<Force> _forces = new List<Force>();

    public static List<Force> Forces
    {
        get
        {
            Debug.Assert(_forces != null);
            return _forces;
        }
        set => _forces = value;
    }

    private static GravityButton _gravityButton;
    
    public static GravityButton GravityButton
    {
        get
        {
            Debug.Assert(_gravityButton != null);
            return _gravityButton;
        }
        set => _gravityButton = value;
    }

    private static VelocityLine _velocityLine;

    public static VelocityLine VelocityLine
    {
        get
        {
            Debug.Assert(_velocityLine != null);
            return _velocityLine;
        }
        set => _velocityLine = value;
    }
}
