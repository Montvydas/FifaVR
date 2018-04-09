using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeviceCamera : MonoBehaviour 
{
	static WebCamTexture backCam;

	void Start()
	{
		if (backCam == null)
			backCam = new WebCamTexture();

		GetComponent<Renderer>().material.mainTexture = backCam;

		if (!backCam.isPlaying)
			backCam.Play();

	}

	void Update()
	{
	}

	private void FixedUpdate()
	{
		var colors = backCam.GetPixels();
		var greyscale = colors.Select(color => color.grayscale).ToArray();
		
//		greyscale.GetEnumerator().
		Debug.Log(string.Format("color length={0}",greyscale.Length));
	}
}
