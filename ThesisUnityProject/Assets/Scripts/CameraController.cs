using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class CameraController
{
	private Camera _myCamera;
	private Transform _targetSquare;
	private Vector2 _margin = new Vector2(1f, 1f);
	private Vector2 _smoothing = new Vector2(1f, 1f);
	private Vector3 _min;
	private Vector3 _max;
	private bool _isFollowing;
	public readonly float cameraHalfSizeY;
	public readonly float cameraHalfSizeX;
	private float edgeSize = 2f;
	public CameraController(Camera camera, bool isFollow, Transform targetSquare)
	{
		_myCamera = camera;
		_isFollowing = isFollow;
		_targetSquare = targetSquare;
		cameraHalfSizeY = _myCamera.orthographicSize;
		cameraHalfSizeX = cameraHalfSizeY * ((float)Screen.width / Screen.height);
		cameraHalfSizeX -= edgeSize;
		cameraHalfSizeY -= edgeSize;
	}



	public void Update()
	{
		var currentPos = _myCamera.transform.position;
		var x = currentPos.x;
		var y = currentPos.y;
		if (_isFollowing) {
			if (Mathf.Abs(x - _targetSquare.position.x)> cameraHalfSizeX)
			{
				if (x - _targetSquare.position.x > 0) x = _targetSquare.position.x + cameraHalfSizeX;
				else x = _targetSquare.position.x - cameraHalfSizeX;
				_myCamera.transform.position = new Vector3(x, y, currentPos.z);
			}

			if (Mathf.Abs(y - _targetSquare.position.y)> cameraHalfSizeY)
			{
				if (y - _targetSquare.position.y > 0) y = _targetSquare.position.y + cameraHalfSizeY;
				else y = _targetSquare.position.y - cameraHalfSizeY;
				_myCamera.transform.position = new Vector3(x, y, currentPos.z);
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

		_myCamera.transform.position = new Vector3(x, y, currentPos.z);
	}
	
}
