using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneLife : MonoBehaviour
{
    private SpriteRenderer _lifeSpriteRenderer;

    private BoxCollider2D _collider;

    private void Start()
    {
        _lifeSpriteRenderer = GetComponent<SpriteRenderer>();
        _lifeSpriteRenderer.color = Services.GameCfg.liveColor;
        _collider = GetComponent<BoxCollider2D>();
        
    }

    private void Update()
    { 
        _lifeSpriteRenderer.color =
                Color.Lerp(Services.GameCfg.liveColor, Color.cyan, Mathf.PingPong(Time.time, 1));

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            Services.EventManager.Fire(new GainLife(gameObject, _lifeSpriteRenderer));
            Destroy(_collider);
            Destroy(this);
        }
    }
}
