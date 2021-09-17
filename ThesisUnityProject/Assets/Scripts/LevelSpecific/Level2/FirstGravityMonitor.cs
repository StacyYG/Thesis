using UnityEngine;

public class FirstGravityMonitor : MonoBehaviour
{
    private GameObject _gravityButton;

    private void Start()
    {
        _gravityButton = GameObject.FindWithTag("GravityButton");
    }

    private void OnMouseDown()
    {
        Services.EventManager.Fire(new FirstGravity());
        Destroy(this);
    }

    private void Update()
    {
        if (!_gravityButton.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.G))
        {
            Services.EventManager.Fire(new FirstGravity());
            Destroy(this);
        }
    }
}

public class FirstGravity : AGPEvent{}
