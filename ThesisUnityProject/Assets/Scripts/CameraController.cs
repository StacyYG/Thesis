using UnityEngine;

public class CameraController
{
	private readonly Transform _targetSquare;
	private readonly Vector2 _innerMargin = new Vector2(1f, 1f); // The target always stays inside the inner margin
	private Vector3 _min, _max;
	public bool isFollowing, lockY, lockX;
	private readonly float _cameraBoundHalfY, _cameraBoundHalfX;
	private readonly Transform _transform;
	public Margin viewMargin;

	public CameraController(Camera camera, bool isFollow, Transform targetSquare)
	{
		_transform = camera.transform;
		isFollowing = isFollow;
		_targetSquare = targetSquare;
		_cameraBoundHalfY = camera.orthographicSize;
		_cameraBoundHalfX = _cameraBoundHalfY * ((float)Screen.width / Screen.height);
	}

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
				if (Mathf.Abs(x - targetPos.x) > _innerMargin.x)
					x = Mathf.Lerp(x, targetPos.x, (Mathf.Abs(x - targetPos.x) - _innerMargin.x) * Time.fixedDeltaTime);
			}

			if (!lockY)
			{
				if (Mathf.Abs(y - targetPos.y) > _innerMargin.y)
					y = Mathf.Lerp(y, targetPos.y, (Mathf.Abs(y - targetPos.y) - _innerMargin.y) * Time.fixedDeltaTime);
			}
			
		}
		
		_transform.position = new Vector3(x, y, currentPos.z);
		
		viewMargin.left = x - _cameraBoundHalfX;
		viewMargin.right = x + _cameraBoundHalfX;
		viewMargin.down = y - _cameraBoundHalfY;
		viewMargin.up = y + _cameraBoundHalfY;
	}
	
}

public struct Margin
{
	public float left, right, up, down;
}
