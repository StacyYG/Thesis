using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class CameraController
{
	private readonly Transform _targetSquare;
	private readonly Vector2 _innerMargin = new Vector2(1f, 1f);
	private readonly Vector2 _outerMargin = new Vector2(2f, 2.2f);
	private Vector3 _min;
	private Vector3 _max;
	public bool isFollowing;
	public readonly float cameraBoundHalfY;
	public readonly float cameraBoundHalfX;
	private float _moveBoundX, _moveBoundY;
	private readonly Transform _transform;
	public Margin viewMargin;
	public bool lockY, lockX;
	public CameraController(Camera camera, bool isFollow, Transform targetSquare)
	{
		_transform = camera.transform;
		isFollowing = isFollow;
		_targetSquare = targetSquare;
		cameraBoundHalfY = camera.orthographicSize;
		cameraBoundHalfX = cameraBoundHalfY * ((float)Screen.width / Screen.height);
		_moveBoundX = cameraBoundHalfX - _outerMargin.x;
		_moveBoundY = cameraBoundHalfY - _outerMargin.y;
	}

	public CameraController(bool isFollow) =>
		new CameraController(Services.MainCamera, isFollow, Services.TargetSquare.transform);


	public void Update()
	{
		var currentPos = _transform.position;
		var x = currentPos.x;
		var y = currentPos.y;
		if (isFollowing) 
		{
			var targetPos = _targetSquare.position;
			if (!lockX)
			{
				if (Mathf.Abs(x - _targetSquare.position.x) > _moveBoundX)
				{
					if (x - _targetSquare.position.x > 0) x = _targetSquare.position.x + _moveBoundX;
					else x = _targetSquare.position.x - _moveBoundX;
				}

				if (Mathf.Abs(x - targetPos.x) > _innerMargin.x)
				{
					x = Mathf.Lerp(x, targetPos.x, (Mathf.Abs(x - targetPos.x) - _innerMargin.x) * Time.fixedDeltaTime);
				}
			}

			if (!lockY)
			{
				if (Mathf.Abs(y - _targetSquare.position.y) > _moveBoundY)
				{
					if (y - _targetSquare.position.y > 0) y = _targetSquare.position.y + _moveBoundY;
					else y = _targetSquare.position.y - _moveBoundY;
				}

				if (Mathf.Abs(y - targetPos.y) > _innerMargin.y)
				{
					y = Mathf.Lerp(y, targetPos.y, (Mathf.Abs(y - targetPos.y) - _innerMargin.y) * Time.fixedDeltaTime);
				}
			}
			
		}

		_transform.position = new Vector3(x, y, currentPos.z);
		viewMargin.left = x - cameraBoundHalfX - 2f;
		viewMargin.right = x + cameraBoundHalfX + 2f;
		viewMargin.down = y - cameraBoundHalfY - 1f;
		viewMargin.up = y + cameraBoundHalfY + 1f;
	}
	
}

public struct Margin
{
	public float left, right, up, down;
}
