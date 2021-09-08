using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseItem : MonoBehaviour
{
    private SpriteRenderer _lifeSpriteRenderer;
    private BoxCollider2D _collider;
    public List<Vector3> positions;
    private int _currentPosIndex;
    private const float MaxSpeed = 20f;
    private bool _isMoving;
    private Vector3 _targetPos = new Vector3();
    private Rigidbody2D _myRb;
    private Rigidbody2D _targetRb;

    public void Start()
    {
        _lifeSpriteRenderer = GetComponent<SpriteRenderer>();
        _lifeSpriteRenderer.color = Services.GameCfg.liveColor;
        _collider = GetComponent<BoxCollider2D>();
        _myRb = GetComponent<Rigidbody2D>();
        _targetRb = Services.TargetSquare.GetComponent<Rigidbody2D>();
        ResetPosition();
    }

    public void ResetPosition()
    {
        _currentPosIndex = 0;
        transform.position = Services.MainCamera.transform.position + positions[_currentPosIndex] + new Vector3(0f, 0f, -Services.MainCamera.transform.position.z);
    }

    private void Update()
    { 
        _lifeSpriteRenderer.color =
                Color.Lerp(Services.GameCfg.liveColor, Color.cyan, Mathf.PingPong(Time.time, 1));
        if(_isMoving)
        {
            if (transform.position == _targetPos)
            {
                if (_currentPosIndex == positions.Count - 1)
                {
                    Services.EventManager.Fire(new ShowGoal());
                    gameObject.SetActive(false);
                }
                else
                {
                    _collider.enabled = true;
                    _isMoving = false;
                }
            }
            
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, MaxSpeed * Time.deltaTime);
        }

        var pos = transform.position;
        if (pos.x > Services.CameraController.viewMargin.right)
            transform.position = new Vector3(Services.CameraController.viewMargin.left, pos.y, pos.z);
        if (pos.x < Services.CameraController.viewMargin.left)
            transform.position = new Vector3(Services.CameraController.viewMargin.right, pos.y, pos.z);
        if (pos.y > Services.CameraController.viewMargin.up)
            transform.position = new Vector3(pos.x, Services.CameraController.viewMargin.down, pos.z);
        if (pos.y < Services.CameraController.viewMargin.down)
            transform.position = new Vector3(pos.x, Services.CameraController.viewMargin.up, pos.z);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            _collider.enabled = false;
            _currentPosIndex++;
            _targetPos = Services.MainCamera.transform.position + positions[_currentPosIndex] + new Vector3(0f, 0f, -Services.MainCamera.transform.position.z);
            _isMoving = true;
            _myRb.velocity = Vector2.zero;
            _targetRb.velocity = Vector2.zero;
            
        }
    }
}

public class ShowGoal : AGPEvent{}
