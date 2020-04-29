using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RightBound : MonoBehaviour
{
    private TextMeshPro _tmp;
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<TextMeshPro>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TargetSquare"))
            _tmp.text = "Why are you here? Go back.";
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TargetSquare"))
            _tmp.text = "";
    }
}
