using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeHazard : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            Destroy(gameObject);
        }
    }
}
