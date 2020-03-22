using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level0 : MonoBehaviour
{
    public float speed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        var rb = Services.TargetSquare.gameObject.GetComponent<Rigidbody2D>();
        //rb.velocity = speed * Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
