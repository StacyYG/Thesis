using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseItem : MonoBehaviour
{
    private SpriteRenderer _lifeSpriteRenderer;
    private BoxCollider2D _collider;
    public LevelCfg0 cfg0;
    private int _currentPosIndex;
    private const float MaxSpeed = 20f;
    private bool _isMoving;
    private Vector3 _targetPos;
    private Rigidbody2D _myRb;
    private Rigidbody2D _targetRb;
    private void Start()
    {
        _lifeSpriteRenderer = GetComponent<SpriteRenderer>();
        _lifeSpriteRenderer.color = Services.GameCfg.liveColor;
        _collider = GetComponent<BoxCollider2D>();
        _myRb = GetComponent<Rigidbody2D>();
        _targetRb = Services.TargetSquare.GetComponent<Rigidbody2D>();
        transform.position = Services.TargetSquare.transform.position + cfg0.chaseItemPositions[_currentPosIndex];
        _myRb.velocity = 1.2f * _targetRb.velocity;
        
    }

    private void Update()
    { 
        _lifeSpriteRenderer.color =
                Color.Lerp(Services.GameCfg.liveColor, Color.cyan, Mathf.PingPong(Time.time, 1));
        if(_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, MaxSpeed * Time.deltaTime);
            if (transform.position == _targetPos)
            {
                if (_currentPosIndex == cfg0.chaseItemPositions.Count - 1)
                {
                    Services.EventManager.Fire(new ShowGate());
                    Destroy(gameObject);
                }
                else
                {
                    _collider.enabled = true;
                    _isMoving = false;
                }
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            _collider.enabled = false;
            _currentPosIndex++;
            _targetPos = Services.TargetSquare.transform.position + cfg0.chaseItemPositions[_currentPosIndex];
            _isMoving = true;
            _myRb.velocity = Vector2.zero;
            _targetRb.velocity = Vector2.zero;
            
        }
    }
}

public class ShowGate : AGPEvent{}
