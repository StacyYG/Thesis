using UnityEngine;

public class SpringObj : MonoBehaviour
{
    public float k = 3f;
    public Vector3 restRelativePosition = new Vector3(0f, 1f, 0f);
    public Transform fixedPart;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _rb.AddForce(k * Time.deltaTime * (fixedPart.position + restRelativePosition - transform.position));
        
    }
}
