using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreator : MonoBehaviour
{
	//configure which noise value to use
	[Range(1, 3)] public int dimensions = 2;
	
	public int width = 320;
	public int height = 180;

	public Gradient coloring;
	
	private Texture2D texture;
	public float frequency = 1f;

	private void OnEnable()
	{
		if (texture == null)
		{
			texture = new Texture2D(width, height, TextureFormat.RGB24, true);
			texture.name = "Procedural Texture";
			//texture.wrapMode = TextureWrapMode.Clamp;
			//texture.filterMode = FilterMode.Point;
			GetComponent<MeshRenderer>().material.mainTexture = texture;
			FillTexture();
		}
	}

	private void FillTexture()
	{
		if (texture.width != width || texture.height != height)
		{
			texture.Resize(width, height);
		}
		
		//define the local coordinates of the four corners, and transform the four corner points to world space
		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		Vector3 point10 = transform.TransformPoint(new Vector3(0.5f,-0.5f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f,0.5f));
		Vector3 point11 = transform.TransformPoint(new Vector3(0.5f,0.5f));

		//call this method to get the noise value
		NoiseMethod method = Noise.valueMethods[dimensions - 1];
	
		float stepsizeX = 1f / width;
		float stepsizeY = 1f / height;

		Random.InitState(42);
		for (int y = 0; y < height; y++)
		{
			//interpolate the corners first based on y then x; point0 for left side, point1 for the right side
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepsizeY);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepsizeY);
			
			for (int x = 0; x < width; x++)
			{
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepsizeX);
				float sample = method(point, frequency);
				
				texture.SetPixel(x, y, coloring.Evaluate(sample));

			}
		}
		texture.Apply(	);
	}

	
	// Update is called once per frame
	void Update () {
		if (transform.hasChanged)
		{
			transform.hasChanged = false;
			FillTexture();
		}
	}
}
