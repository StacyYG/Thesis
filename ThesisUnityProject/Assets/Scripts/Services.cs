using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Services
{

    private static Camera _myCamera;

    public static Camera MyCamera
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

    private static ControllerSquare _controllerSquare;

    public static ControllerSquare ControllerSquare
    {
        get
        {
            Debug.Assert(_controllerSquare != null);
            return _controllerSquare;
        }
        set { _controllerSquare = value; }
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

    private static int _totalLineNumber;

    public static int TotalLineNumber
    {
        get => _totalLineNumber;
        set => _totalLineNumber = value;
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

    private static Goal _goal;

    public static Goal Goal
    {
        get
        { 
            Debug.Assert(_goal != null);
            return _goal;
        }
        set => _goal = value;
    }
}
