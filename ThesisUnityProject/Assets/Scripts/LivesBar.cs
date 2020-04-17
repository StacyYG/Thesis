using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LivesBar
{
    private List<GameObject> _lifeBoxes;
    private List<SpriteRenderer> _spriteRenderers;
    private const float GapSize = 0.5f;
    private Vector3 _startPos = new Vector3(0.5f, -0.5f, 0f);
    private const float Speed = 20f;
    private bool _newLifeMoving;
    private Transform _newLife;
    private Vector3 _nextLifePos;
    private readonly Transform _transform;

    public LivesBar(Transform transform)
    {
        _transform = transform;
    }
    // Start is called before the first frame update
    public void Init()
    {
        _lifeBoxes = new List<GameObject>();
        _spriteRenderers = new List<SpriteRenderer>();
        _startPos += new Vector3(-Services.MainCamera.orthographicSize * Screen.width / Screen.height,
            Services.MainCamera.orthographicSize, 0f);
        for (int i = 0; i < Services.GameCfg.startLifeNum; i++)
        {
            var box = GameObject.Instantiate(Services.GameCfg.lifeBox, _startPos + i * new Vector3(GapSize, 0f, 0f), Quaternion.identity,
                _transform);
            var sr = box.GetComponent<SpriteRenderer>();
            sr.color = Services.GameCfg.liveColor;
            _lifeBoxes.Add(box);
            _spriteRenderers.Add(sr);
        }
        Services.EventManager.Register<LoseLife>(OnLoseLife);
        Services.EventManager.Register<GainLife>(OnGainLife);
    }

    // Update is called once per frame
    public void Update()
    {
        if (_newLifeMoving)
        {
            _newLife.localPosition = Vector3.MoveTowards(_newLife.localPosition, _nextLifePos, Speed * Time.deltaTime);
            if (_newLife.localPosition == _nextLifePos)
            {
                _lifeBoxes.Add(_newLife.gameObject);
                var sr = _newLife.gameObject.GetComponent<SpriteRenderer>();
                sr.color = Services.GameCfg.liveColor;
                _newLifeMoving = false;
            }
        }
    }

    private void OnLoseLife(AGPEvent e)
    {
        GameObject.Destroy(_lifeBoxes.Last());
        _lifeBoxes.Remove(_lifeBoxes.Last());
        
        if (_lifeBoxes.Count == 0)
        {
            //game over stuff
            return;
        }
    }

    private void OnGainLife(AGPEvent e)
    {
        var gainLife = (GainLife) e;
        _newLife = gainLife.NewLifeObj.transform;
        _newLife.parent = _transform;
        _newLifeMoving = true;
        _nextLifePos = _lifeBoxes.Last().transform.localPosition + new Vector3(GapSize, 0f, 0f);
        var c = gainLife.NewLifeObj.GetComponent<BoxCollider2D>();
            GameObject.Destroy(c);
    }
    
    private void OnDestroy()
    {
        Services.EventManager.Unregister<LoseLife>(OnLoseLife);
        Services.EventManager.Unregister<GainLife>(OnGainLife);
    }
}

public class LoseLife : AGPEvent{}

public class GainLife : AGPEvent
{
    public readonly GameObject NewLifeObj;
    public GainLife(GameObject gameObject)
    {
        NewLifeObj = gameObject;
    }
}

public class VelocityBar
{
    private Transform _speedBar;
    private Transform _directionBar;
    private Rigidbody2D _targetRb;

    public VelocityBar(Transform speedBar, Transform directionBar, Rigidbody2D target)
    {
        _speedBar = speedBar;
        _directionBar = directionBar;
        _targetRb = target;
    }
    public void UpdateSize()
    {
        _speedBar.localScale = new Vector3(_targetRb.velocity.magnitude, 1f, 1f);
        _directionBar.transform.up = _targetRb.velocity.normalized;
    }
}


