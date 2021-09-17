using System.Collections;
using UnityEngine;

public class NoEnter : MonoBehaviour // GameObjects entering this field will be deactivated
{
    private void Start()
    {
        Services.EventManager.Register<Success>(OnSuccess);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the target square enters this field, the game reloads
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
