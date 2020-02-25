using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ServiceLocator
{

    private static Camera _myCamera;

    public static Camera myCamera
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

    public static GameController gameController
    {
        get
        {
            if (_gameController == null)
            {
                _gameController = GameObject.FindObjectOfType<GameController>();
            }

            return _gameController;
        }
        set => _gameController = value;
    }

}
