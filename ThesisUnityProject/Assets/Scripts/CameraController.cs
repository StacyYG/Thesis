using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class CameraController : MonoBehaviour {

	private Transform targetSquare;
	public Vector2 Margin;
	public Vector2 smoothing;
	
	private Vector3 _min;
	private Vector3 _max;

	public bool IsFollowing;
	
	private void Start()
	{
		IsFollowing = true;
		targetSquare = Services.TargetSquare.transform;

	}
	

	private void Update()
	{
		var currentPos = transform.position;
		var x = currentPos.x;
		var y = currentPos.y;
		if (IsFollowing) {
			if (Mathf.Abs (x - targetSquare.position.x) > Margin.x)
			{
				x = Mathf.Lerp(x, targetSquare.position.x, smoothing.x * Time.deltaTime);
			}
			if (Mathf.Abs (y - targetSquare.position.y)> Margin.y)
			{
				y = Mathf.Lerp(y, targetSquare.position.y, smoothing.y * Time.deltaTime);
			}
		}

		transform.position = new Vector3(x, y, currentPos.z);
	}
	
}
