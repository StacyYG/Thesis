using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vectrosity;
using Object = UnityEngine.Object;

public class CancelButton
{
    private float _boundCircleRadius;
    private VectorLine _circleLine;
    public bool Respond = true;
    public BoundCircle boundCircle;
    private GameObject _gameObject;

    public CancelButton(GameObject gameObject)
    {
        _gameObject = gameObject;
        _boundCircleRadius = gameObject.GetComponent<CircleCollider2D>().radius;
    }

    public void Start()
    {
        boundCircle = new BoundCircle(_boundCircleRadius, _gameObject.transform);
    }
}

public class GravityButton
{
    public BoundCircle boundCircle;
    private GameObject _triangle;
    private List<Rigidbody2D> _rbs;
    private float _boundCircleRadius;
    private List<Rigidbody2D> _allRbs, _activeRbs;
    public bool GravityOn { get; private set; }

    public GravityButton(GameObject circleObj)
    {
        _boundCircleRadius = circleObj.GetComponent<CircleCollider2D>().radius;
        _triangle = circleObj.transform.GetChild(0).gameObject;
        Init();
    }

    public void Start()
    {
        boundCircle = new BoundCircle(_boundCircleRadius, _triangle.transform);
        for (int i = 0; i < _allRbs.Count; i++)
        {
            if (!_allRbs[i].CompareTag("TargetSquare"))
                if (_allRbs[i].gameObject.GetComponent<SpriteRenderer>())
                    new Gravity(_allRbs[i].gameObject,
                        _allRbs[i].gameObject.GetComponent<SpriteRenderer>().color + new Color(0.2f, 0.2f, 0.2f, 1f));
        }
    }

    public void GravitySwitch()
    {
        if (GravityOn)
        {
            GravityOn = false;
            _triangle.transform.localScale = new Vector3(1f, 0f, 1f);
        }
        else
        {
            GravityOn = true;
            _triangle.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void Init()
    {
        _allRbs = new List<Rigidbody2D>();
        _activeRbs = new List<Rigidbody2D>();
        _allRbs = Object.FindObjectsOfType<Rigidbody2D>().ToList();
        _activeRbs = Object.FindObjectsOfType<Rigidbody2D>().ToList();
    }
    private bool _isInCameraView(Vector2 position)
    {
        if (position.x > Services.CameraController.viewMargin.right) return false;
        if (position.x < Services.CameraController.viewMargin.left) return false;
        if (position.y > Services.CameraController.viewMargin.up) return false;
        if (position.y < Services.CameraController.viewMargin.down) return false;
        return true;
    }

    public void Update()
    {
        UpdateRbs();
        UpdateGravity();
    }

    private void UpdateRbs()
    {
        for (int i = 0; i < _allRbs.Count; i++)
        {
            if (_isInCameraView(_allRbs[i].position))
            {
                if (!_allRbs[i].gameObject.activeSelf)
                {
                    _allRbs[i].gameObject.SetActive(true);
                    _activeRbs.Add(_allRbs[i]);
                }
            }
            else
            {
                if (_allRbs[i].gameObject.activeSelf)
                {
                    _allRbs[i].gameObject.SetActive(false);
                    _activeRbs.Remove(_allRbs[i]);
                }
            }
        }
    }
    private void UpdateGravity()
    {
        for (int i = 0; i < _activeRbs.Count; i++)
        {
            _activeRbs[i].gravityScale = GravityOn ? 1f : 0f;
        }
    }
}

public class BoundCircle
{
    private VectorLine _circle;
    public DelegateTask GrowUp;
    private float _elapsedTime;
    public BoundCircle(float radius, Transform anchor, float duration = 1f)
    {
        var segmentNum = (int) (Services.GameCfg.segmentsPerRadius * radius);
        _circle = new VectorLine("circle", new List<Vector3>(segmentNum), 8f * Screen.height / 1080f, LineType.Points);
        _circle.color = Services.GameCfg.boundCircleColor;
        GrowUp = new DelegateTask(() => {}, () =>
        {
            _elapsedTime += Time.deltaTime;
            _circle.MakeCircle(anchor.position, _elapsedTime / duration * radius, segmentNum);
            _circle.Draw();
            return _elapsedTime >= duration;
        });
    }

    public void Clear()
    {
        VectorLine.Destroy(ref _circle);
    }
}
