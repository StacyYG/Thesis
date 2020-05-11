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
            StartCoroutine(Reload());
            return;
        }
        other.gameObject.transform.position = new Vector3(0f, -30f, 0f);
        other.gameObject.SetActive(false);
    }

    private void OnSuccess(AGPEvent e)
    {
        Services.EventManager.Unregister<Success>(OnSuccess);
        Destroy(gameObject);
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(1f);
        Services.GameController.Reload();
    }
}
