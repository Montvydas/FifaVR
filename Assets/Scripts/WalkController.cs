using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WalkController : MonoBehaviour {
	private Vector3 cameraSpeed;
	private float allGravity = 0.0f;
	private float distance = 0.0f;
	private bool foundPoint = false;
	private float prevAllGravity = 0.0f;
	private float previousSystemTime = 0.0f;
	private int stepNumber = 0;
	float freq = 0.0f;
	public float thrust = 2.0f;
	
	// Use this for initialization
	void Start () {
		Input.gyro.enabled = true;
	}
	
	// Update is called once per frame
	private float gravityFront;
	void Update () 
	{
		//transform.position += Camera.main.transform.forward * thrust * Time.deltaTime;
		cameraSpeed = Camera.main.transform.forward * thrust * Time.deltaTime;	//get camera looking position as a Vector3 * thrust speed

		allGravity = Input.acceleration.sqrMagnitude - 1;
		
		float acc1 = Input.gyro.userAcceleration.sqrMagnitude;
//		gravityFront = Input.gyro.userAcceleration.z;
		float acc2 = Input.acceleration.sqrMagnitude;
		float acc3 = Input.gyro.gravity.y;

//		accelAnalysis();
		Debug.Log (string.Format ("userAcc, acc, gravity: {0:0.00} {1:0.00} {2:0.00}", acc1, acc2, acc3)); 
	}
	
	//add physics in here
	void FixedUpdate ()
	{
		Vector3 velocity = cameraSpeed * distance;
	}

	
	void OnApplicationQuit() {
		Input.gyro.enabled = false;
	}
	
	private void accelAnalysis ()
	{
//		float threshold = 0.07f;
		float threshold = 0.5f;
		bool isRising;

		//Check if the curve is rising or falling
		if (prevAllGravity < allGravity)
			isRising = true;
		else
			isRising = false;
		
		//finds the first point
		if ( (allGravity > threshold) && isRising && !foundPoint && Mathf.Abs(gravityFront) < 0.1f){
			foundPoint = true;
			distance += Mathf.Abs(allGravity - prevAllGravity) * freq;
			previousSystemTime = Time.time*1000f;
		}
			
		if (Time.time*1000f - previousSystemTime > 500.0f) {
			distance = 0.0f;
			freq = 0.0f;
		}
		//find the second point
		if ((allGravity < -threshold) && !isRising && foundPoint) {
			foundPoint = false;
			float dT = Time.time*1000.0f - previousSystemTime;
			freq = 1000.0f / dT;
			//checks for noise
//			
//			if (freq < 15.0) {
			stepNumber++;
//				distance = freq / 2.0;
			distance = 0.0f;
//			}

		}

		prevAllGravity = allGravity;
	}
}
