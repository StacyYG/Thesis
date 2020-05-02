using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringObj : MonoBehaviour
{
    public float k = 3f;
    public Vector3 restRelativePosition = new Vector3(0f, 1f, 0f);
    public Transform fixedPart;
    private Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.AddForce(k * Time.deltaTime * (fixedPart.position + restRelativePosition - transform.position));
        
    }
}
