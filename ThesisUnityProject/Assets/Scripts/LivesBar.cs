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
    private SpriteRenderer _newLifeSprRdr;
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
                _newLifeSprRdr.color = Services.GameCfg.liveColor;
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
        if(_lifeBoxes.Count == 0) return;
        var gainLife = (GainLife) e;
        _newLife = gainLife.NewLifeObj.transform;
        _newLifeSprRdr = gainLife.LifeSpriteRenderer;
        _newLife.parent = _transform;
        _newLifeMoving = true;
        _nextLifePos = _lifeBoxes.Last().transform.localPosition + new Vector3(GapSize, 0f, 0f);
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
    public readonly SpriteRenderer LifeSpriteRenderer;
    public GainLife(GameObject gameObject, SpriteRenderer spriteRenderer)
    {
        NewLifeObj = gameObject;
        LifeSpriteRenderer = spriteRenderer;
    }
}

public class VelocityBar
{
    private readonly Transform _speedBar;
    private readonly Transform _directionBar;
    private readonly Rigidbody2D _targetRb;
    public readonly SpriteRenderer speedSprRdr;
    public readonly SpriteRenderer directionSprRdr;
    private const float MaxSpeed = 12f;
    private readonly GameObject _speedWarning;

    public VelocityBar(GameObject speedBarObj, GameObject directionBarObj, Rigidbody2D targetRb, GameObject speedWarningObj)
    {
        _speedBar = speedBarObj.transform;
        _directionBar = directionBarObj.transform;
        _targetRb = targetRb;
        speedSprRdr = speedBarObj.GetComponent<SpriteRenderer>();
        directionSprRdr = directionBarObj.GetComponent<SpriteRenderer>();
        _speedWarning = speedWarningObj;
    }
    public void Update()
    {
        var velocity = _targetRb.velocity;
        _speedBar.localScale = new Vector3(velocity.magnitude, 0.5f, 1f);
        _directionBar.transform.up = velocity.normalized;
        if (!ReferenceEquals(_speedWarning, null))
        {
            if (velocity.magnitude > MaxSpeed)
                _speedWarning.SetActive(true);
            else
                _speedWarning.SetActive(false);
        }

    }
}




