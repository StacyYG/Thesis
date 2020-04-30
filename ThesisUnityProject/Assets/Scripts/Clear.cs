using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : MonoBehaviour
{
    private void Start()
    {
        Services.EventManager.Register<Success>(OnSuccess);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            Debug.Log("reload");
            Services.GameController.Reload();
            return;
        }
        
        other.gameObject.SetActive(false);
    }

    private void OnSuccess(AGPEvent e)
    {
        Services.EventManager.Unregister<Success>(OnSuccess);
        Destroy(gameObject);
    }
}
