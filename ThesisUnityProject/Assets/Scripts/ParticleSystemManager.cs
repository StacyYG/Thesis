using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    private const int Num = 9;
    private List<Transform> _particleSystems;
    private CameraController _cameraController;
    private Transform _camera;
    private Transform _currentPS;
    // Start is called before the first frame update
    void Start()
    {
        _cameraController = Services.CameraController;
        _camera = Services.MyCamera.transform;
        InstantiatePS();
        SetPositions();
        _particleSystems[0].gameObject.SetActive(true);
        _currentPS = _particleSystems[0];
    }

    private void InstantiatePS()
    {
        _particleSystems = new List<Transform>();
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                _particleSystems.Add(transform.GetChild(i));
            }

            if (transform.childCount < Num)
            {
                for (int i = 0; i < Num - transform.childCount; i++)
                {
                    var p = Instantiate(_particleSystems[0]);
                    _particleSystems.Add(p.transform);
                }
            }
            else if (transform.childCount > Num)
            {
                for (int i = transform.childCount - 1; i > Num - 1; i--)
                {
                    Destroy(_particleSystems[i].gameObject);
                    _particleSystems.Remove(_particleSystems[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < Num; i++)
            {
                var p = Instantiate(Resources.Load<GameObject>("Prefabs/ParticleSystem"));
                _particleSystems.Add(p.transform);
            }
        }
    }

    private void SetPositions()
    {
        var screenSizeX = _cameraController.cameraHalfSizeX * 2f;
        var screenSizeY = _cameraController.cameraHalfSizeY * 2f;
        _particleSystems[1].position = new Vector2(-screenSizeX, screenSizeY);
        _particleSystems[2].position = new Vector2(-0f, screenSizeY);
        _particleSystems[3].position = new Vector2(screenSizeX, screenSizeY);
        _particleSystems[4].position = new Vector2(-screenSizeX, 0f);
        _particleSystems[5].position = new Vector2(screenSizeX, 0f);
        _particleSystems[6].position = new Vector2(-screenSizeX, -screenSizeY);
        _particleSystems[7].position = new Vector2(-0f, -screenSizeY);
        _particleSystems[8].position = new Vector2(screenSizeX, -screenSizeY);
    }
    // Update is called once per frame
    void Update()
    {
        var cameraPos = _camera.position;
        if (cameraPos.x > _currentPS.position.x)
        {
            
        }
    }
}
