using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkController : MonoBehaviour {
	private Vector3 cameraSpeed;
	private float allGravity = 0.0f;
	private float distance = 0.0f;
	private float jump = 0.0f;
	private bool foundPoint = false;
	private float prevAllGravity = 0.0f;
	private float previousSystemTime = 0.0f;
	private int stepNumber = 0;
	float freq = 0.0f;
	public float thrust = 2.0f;

	private Rigidbody rigidbody; 
	
	[SerializeField] private float m_RenderScale = 2f;
	// Use this for initialization
	void Start ()
	{
//		UnityEngine.XR.XRSettings.renderViewportScale = m_RenderScale;
		Input.gyro.enabled = true;
		rigidbody = GetComponent<Rigidbody>();
		
		data = new List<double>();
		count = 0;
	}

	private List<double> data;
	private int count;
	
	// Update is called once per frame
	void Update () 
	{
		//transform.position += Camera.main.transform.forward * thrust * Time.deltaTime;
//		cameraSpeed = Camera.main.transform.forward * thrust * Time.deltaTime;	//get camera looking position as a Vector3 * thrust speed

//		CharacterController controller = GetComponent<CharacterController>();
		
		float acc1 = Input.gyro.userAcceleration.y;

		Vector3 gravityVector = Input.gyro.gravity;
		Vector3 accVector = Input.gyro.userAcceleration;		
		float projection = Vector3.Project(accVector, gravityVector).sqrMagnitude;
		projection = Vector3.Dot(accVector, gravityVector) < 0 ? -projection : projection;

//		Debug.Log (string.Format ("Projection, AccY: {0:0.000} {1:0.000}", projection, acc1));

//		if (count > 150)
//		{
//			count = 0;
//			Debug.LogError("Samples Taken");
//
//			string printValues = "[";
//			data.ForEach(d =>
//			{
//				printValues += string.Format("{0:0.000}, ", d);
//			});
//			
//			Debug.LogError(printValues);			
//			data.Clear();
//		}
//		
//		count++;
//		data.Add(projection);
		
		
		allGravity = projection;
		AccelAnalysis();
	}
	
	//add physics in here
	void FixedUpdate ()
	{
//		transform.position += transform.forward * thrust * Time.deltaTime * distance;
//		rigidbody.MovePosition(transform.position + Camera.main.transform.forward * thrust * Time.deltaTime * distance);

//		Vector3 movement = transform.position + Camera.main.transform.forward * thrust * Time.deltaTime * distance;
		
		Vector3 movement = transform.position + Camera.main.transform.forward * thrust * Time.deltaTime * distance;
//		rigidbody.velocity = movement;
		
//		RaycastHit hit;
//		if (Physics.Raycast(transform.position, -transform.up, out hit, 3))
//		{
//			rigidbody.AddForce(transform.up*(10.0f/(hit.distance/2)));
//		}
		
//		rigidbody.MovePosition(movement);
		
		Vector3 force = Camera.main.transform.forward * thrust * Time.deltaTime * distance;
		
//		rigidbody.MovePosition(movement);
			
//		rigidbody.velocity = movement;
		
		if (Input.touchCount > 0 || Input.GetMouseButton(0))
		{
			force = Camera.main.transform.forward * thrust * Time.deltaTime;
		}

		// change to IsGrounded later
		if (transform.position.y < 3.5)
		{
			force.y = thrust * Time.deltaTime * jump;
		}

		rigidbody.AddForce(force);
		
		RaycastHit hit;
		Ray downRay = new Ray(transform.position, -Vector3.up);
		if (Physics.Raycast(downRay, out hit)) {
			float hoverError = hoverHeight - hit.distance;
			if (hoverError > 0) {
				float upwardSpeed = rigidbody.velocity.y;
				float lift = hoverError * hoverForce - upwardSpeed * hoverDamp;
				rigidbody.AddForce(lift * Vector3.up);
			}
		}
		
//		transform.position += Vector3.forward * thrust * Time.deltaTime * distance;
//		Camera.main.transform.forward;
//		Vector3 velocity = cameraSpeed * distance;
//		transform.position += transform.position + velocity;
	}

	public float hoverHeight = 3.0F;
	public float hoverForce = 5.0F;
	public float hoverDamp = 0.5F;
	
	void OnApplicationQuit() {
		Input.gyro.enabled = false;
	}
	
	private void AccelAnalysis ()
	{
//		float threshold = 0.07f;
		float threshold = 0.05f;
		bool isRising;

		//Check if the curve is rising or falling
		if (prevAllGravity < allGravity)
			isRising = true;
		else
			isRising = false;
		
		//finds the first point
		if ( allGravity > threshold && isRising && !foundPoint ){
			foundPoint = true;
//			distance += Mathf.Abs(allGravity - prevAllGravity) * freq;
//			distance = (allGravity)*10;
			distance = 1;
			jump = distance / 3;
//			previousSystemTime = Time.time*1000f;
		}
		
		if ( allGravity > 6*threshold && isRising ){
			jump = -0.5f;
		} 
		
		if ( allGravity > 6*threshold && !isRising ){
			jump = 2;
		}
		

			
//		if (Time.time*1000f - previousSystemTime > 500.0f) {
//			distance = 0.0f;
//			freq = 0.0f;
//		}
		
		// find the second point => this previously was set to -therhold,
		// but might be useful to use zero as well 
		if ( allGravity < threshold && !isRising && foundPoint ) {
			foundPoint = false;
//			float dT = Time.time*1000.0f - previousSystemTime;
//			freq = 1000.0f / dT;
			//checks for noise
//			
//			if (freq < 15.0) {
			stepNumber++;
			Debug.LogError (string.Format ("Steps: {0}", stepNumber ));
//				distance = freq / 2.0;
			distance = 0;
			jump = 0;
//			}

		}

		prevAllGravity = allGravity;
	}
}
