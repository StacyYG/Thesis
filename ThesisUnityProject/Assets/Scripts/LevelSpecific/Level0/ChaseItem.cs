using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A target item player chase after in Level 0
public class ChaseItem : MonoBehaviour
{
    private SpriteRenderer _lifeSpriteRenderer;
    private BoxCollider2D _collider;
    public List<Vector3> positions;
    private int _currentPosIndex;
    [SerializeField] private float maxSpeed = 10f;
    private bool _isMoving;
    private Vector3 _targetPos;
    private Rigidbody2D _myRb;

    public void Start()
    {
        _lifeSpriteRenderer = GetComponent<SpriteRenderer>();
        _lifeSpriteRenderer.color = Services.GameCfg.liveColor;
        _collider = GetComponent<BoxCollider2D>();
        _myRb = GetComponent<Rigidbody2D>();
        transform.position = TargetPosition(0);
    }

    private void Update()
    { 
        _lifeSpriteRenderer.color =
                Color.Lerp(Services.GameCfg.liveColor, Color.cyan, Mathf.PingPong(Time.time, 1));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("TargetSquare"))
        {
            _currentPosIndex++;
            StartCoroutine(MoveTo(TargetPosition(_currentPosIndex)));
        }
    }

    public Vector3 TargetPosition(int positionIndex)
    {
        var camPosition = Services.MainCamera.transform.position;
        return new Vector3(camPosition.x, camPosition.y, 0f) + positions[positionIndex];
    }
    
    private IEnumerator MoveTo(Vector3 targetPosition)
    {
        // Cancel the collider while moving
        _collider.enabled = false;
        
        while (Vector3.Distance(transform.position, targetPosition) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxSpeed * Time.deltaTime);
            yield return null;
        }
        
        _collider.enabled = true;
        _myRb.velocity = Vector2.zero; // Freeze the movement upon arrival

        if (_currentPosIndex == positions.Count - 1) // When reach the last task position show the level goal
        {
            Services.EventManager.Fire(new ShowGoal());
            gameObject.SetActive(false);
        }
    }
}

public class ShowGoal : AGPEvent{}
