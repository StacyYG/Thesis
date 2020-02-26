using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceApplier : MonoBehaviour
{
    private GameObject forceObj;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        InstantiateForceObj();
        rb = GetComponent<Rigidbody2D>();
    }



    // Update is called once per frame
    void Update()
    {
        UpdateForceObj(ServiceLocator.ControllerSquare.netForceByPlayer());
        rb.AddForce(ServiceLocator.ControllerSquare.netForceByPlayer());
    }
    private void InstantiateForceObj()
    {
        forceObj = Instantiate(Resources.Load<GameObject>("square10"));
        var spriteRenderer = forceObj.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sortingOrder = ServiceLocator.GameController.orderInLayer;
        ServiceLocator.GameController.orderInLayer++;
        forceObj.transform.position = transform.position;
        forceObj.transform.localScale = Vector3.zero;
    }
    private void UpdateForceObj(Vector3 forceVector)
    {
        forceObj.transform.position = transform.position;
        forceObj.transform.localScale = new Vector3(1f, forceVector.magnitude * 10f, 1f);
        forceObj.transform.up = forceVector.normalized;
    }
}
