using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vectrosity;

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

    public VelocityBar(Rigidbody2D targetRb)
    {
        var findSpeedBar = GameObject.FindGameObjectWithTag("SpeedBar");
        if (findSpeedBar)
        {
            _speedBar = findSpeedBar.transform;
            speedSprRdr = findSpeedBar.GetComponent<SpriteRenderer>();
        }
        
        var findDirectionBar = GameObject.FindGameObjectWithTag("DirectionPointer");
        if (findDirectionBar)
        {
            _directionBar = findDirectionBar.transform;
            directionSprRdr = findDirectionBar.GetComponent<SpriteRenderer>();
        }

        var findSpeedWarning = GameObject.FindGameObjectWithTag("SpeedWarning");
        if(findSpeedWarning)
            _speedWarning = findSpeedWarning;
        
        _targetRb = targetRb;
    }
    public void Update()
    {
        var velocity = _targetRb.velocity;
        if(_speedBar) 
            _speedBar.localScale = new Vector3(velocity.magnitude, 0.5f, 1f);
        if(_directionBar) 
            _directionBar.transform.up = velocity.normalized;
        if (_speedWarning)
        {
            if (velocity.magnitude > MaxSpeed)
                _speedWarning.SetActive(true);
            else
                _speedWarning.SetActive(false);
        }

    }
}

public class VelocityLine
{
    private readonly VectorLine _line;
    private readonly Rigidbody2D _rb;
    private readonly Transform _targetTransform;
    private const float Adjuster = 0.3f;

    public VelocityLine(GameObject gameObject)
    {
        _line = new VectorLine("velocityLine", new List<Vector3> {Vector2.zero, Vector2.zero}, Services.GameCfg.velocityLineWidth * Screen.height / 1080f);
        _line.texture = Services.GameCfg.fullLineTexture;
        _line.color = Services.GameCfg.velocityLineColor;
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _targetTransform = gameObject.transform;
        _line.drawTransform = _targetTransform;
    }

    public void LateUpdate()
    {
        _line.points3[0] = _targetTransform.InverseTransformVector(Adjuster * _rb.velocity);
        _line.Draw();
    }

    public void SetColor(Color color)
    {
        _line.color = color;
    }

    public void MultiplyWidth(float multiplier)
    {
        var currentWidth = _line.lineWidth;
        _line.lineWidth = multiplier * currentWidth;
    }

    public void Hide(bool isHide)
    {
        _line.active = !isHide;
    }
}




