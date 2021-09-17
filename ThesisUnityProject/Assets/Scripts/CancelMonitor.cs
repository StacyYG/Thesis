using UnityEngine;

public class CancelMonitor : MonoBehaviour
{
    private GameObject _cancelButton;
    void Start()
    {
        _cancelButton = GameObject.FindWithTag("CancelButton");
    }
    
    void Update()
    {
        if (!_cancelButton.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Services.EventManager.Fire(new FirstCancel());
            Destroy(this);
        }
    }

    private void OnMouseDown()
    {
        Services.EventManager.Fire(new FirstCancel());
        Destroy(this);
    }
}

public class FirstCancel : AGPEvent{}

