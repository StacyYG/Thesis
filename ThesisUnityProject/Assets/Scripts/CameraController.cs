using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class CameraController
{
	private Transform _targetSquare;
	private Vector2 _margin = new Vector2(1f, 1f);
	private Vector2 _smoothing = new Vector2(1f, 1f);
	private Vector3 _min;
	private Vector3 _max;
	public bool _isFollowing;
	public readonly float cameraHalfSizeY;
	public readonly float cameraHalfSizeX;
	private float edgeSize = 2f;
	private Transform _transform;
	public CameraController(Camera camera, bool isFollow, Transform targetSquare)
	{
		_transform = camera.transform;
		_isFollowing = isFollow;
		_targetSquare = targetSquare;
		cameraHalfSizeY = camera.orthographicSize;
		cameraHalfSizeX = cameraHalfSizeY * ((float)Screen.width / Screen.height);
		cameraHalfSizeX -= edgeSize;
		cameraHalfSizeY -= edgeSize;
	}



	public void Update()
	{
		var currentPos = _transform.position;
		var x = currentPos.x;
		var y = currentPos.y;
		if (_isFollowing) {
			if (Mathf.Abs(x - _targetSquare.position.x)> cameraHalfSizeX)
			{
				if (x - _targetSquare.position.x > 0) x = _targetSquare.position.x + cameraHalfSizeX;
				else x = _targetSquare.position.x - cameraHalfSizeX;
			}

			if (Mathf.Abs(y - _targetSquare.position.y)> cameraHalfSizeY)
			{
				if (y - _targetSquare.position.y > 0) y = _targetSquare.position.y + cameraHalfSizeY;
				else y = _targetSquare.position.y - cameraHalfSizeY;
			}
			
			if (Mathf.Abs (x - _targetSquare.position.x) > _margin.x)
			{
				x = Mathf.Lerp(x, _targetSquare.position.x, _smoothing.x * Time.deltaTime);
			}
			if (Mathf.Abs (y - _targetSquare.position.y)> _margin.y)
			{
				y = Mathf.Lerp(y, _targetSquare.position.y, _smoothing.y * Time.deltaTime);
			}
		}

		_transform.position = new Vector3(x, y, currentPos.z);
	}
	
}
