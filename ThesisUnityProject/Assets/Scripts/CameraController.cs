using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class CameraController
{
	private readonly Transform _targetSquare;
	private readonly Vector2 _innerMargin = new Vector2(1f, 1f);
	private readonly Vector2 _outerMargin = new Vector2(2f, 2.2f);
	private readonly Vector2 _smoothing = new Vector2(1f, 1f);
	private Vector3 _min;
	private Vector3 _max;
	public bool IsFollowing = true;
	public readonly float CameraBoundHalfY;
	public readonly float CameraBoundHalfX;
	private readonly Transform _transform;
	public CameraController(Camera camera, bool isFollow, Transform targetSquare)
	{
		_transform = camera.transform;
		IsFollowing = isFollow;
		_targetSquare = targetSquare;
		CameraBoundHalfY = camera.orthographicSize;
		CameraBoundHalfX = CameraBoundHalfY * ((float)Screen.width / Screen.height);
		CameraBoundHalfX -= _outerMargin.x;
		CameraBoundHalfY -= _outerMargin.y;
	}



	public void Update()
	{
		var currentPos = _transform.position;
		var x = currentPos.x;
		var y = currentPos.y;
		if (IsFollowing) 
		{
			if (Mathf.Abs(x - _targetSquare.position.x)> CameraBoundHalfX)
			{
				if (x - _targetSquare.position.x > 0) x = _targetSquare.position.x + CameraBoundHalfX;
				else x = _targetSquare.position.x - CameraBoundHalfX;
			}

			if (Mathf.Abs(y - _targetSquare.position.y)> CameraBoundHalfY)
			{
				if (y - _targetSquare.position.y > 0) y = _targetSquare.position.y + CameraBoundHalfY;
				else y = _targetSquare.position.y - CameraBoundHalfY;
			}

			var targetPos = _targetSquare.position;
			if (Mathf.Abs (x - targetPos.x) > _innerMargin.x)
			{
				x = Mathf.Lerp(x, targetPos.x, (Mathf.Abs(x - targetPos.x) - _innerMargin.x) * Time.fixedDeltaTime);
			}
			if (Mathf.Abs (y - targetPos.y)> _innerMargin.y)
			{
				y = Mathf.Lerp(y, targetPos.y, (Mathf.Abs(y - targetPos.y) - _innerMargin.y) * Time.fixedDeltaTime);
			}
		}

		_transform.position = new Vector3(x, y, currentPos.z);
	}
	
}
