using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkController : MonoBehaviour {
	// Used to track current acceleration towards gravity 
	// and the one in the previous frame
	private float allGravity;
	private float prevAllGravity;
	
	// defines the distance, if it's 0, we just stand and if 1, we move.
	// thrust defines how fast we move.
	private float distance;
	public float thrust = 1500;
	
	// found point is used within the step algorithm
	// to check when we crossed a specific threshold
	// we don't use step number for now anywhere
	// maybe later switch from distance to pulses
	// as in if detected a step, push the object using pulse force
	// and then let the drag do it's job of stopping it
	private bool foundPoint;
	private int stepNumber;

	// Frequency and system time are not used yet
	private float freq;
	private float previousSystemTime;	

	// This is pretty much just to be able to push and move object
	// might switch to character controller
	private Rigidbody rigidbody; 
	
	// This is used to change the direction of the player
	// and the threshold for how high to look to walk backwards
	private int direction = 1;
	public float walkBackThreshold = 0.1f;
	
	// Used to keep the object at a certain altitude
	public float hoverHeight = 3.0F;
	public float hoverForce = 5.0F;
	public float hoverDamp = 0.5F;

	// enable jump and set jump magnitude
	private bool canJump = true;
	private float jump;
	
	// Used for data collection
	private List<double> data;
	private int count;
	
	
	// Use this for initialization
	void Start ()
	{
		Input.gyro.enabled = true;
		rigidbody = GetComponent<Rigidbody>();
		
		data = new List<double>();
		count = 0;
	}
	
	void OnApplicationQuit() {
		Input.gyro.enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CharacterController controller = GetComponent<CharacterController>();
		
		// To have movement we require gravity vector and local acc vector
		// local acc also corresponds to world axis within the game
		Vector3 gravityVector = Input.gyro.gravity;
		Vector3 accVector = Input.gyro.userAcceleration;		
		
		// If we project the acc onto gravity, we can extract the motion only towards 
		// the gravity direction -> as in up/down. This is good when avoiding
		// noise in any other direction e.g. rotating a head.
		//
		// We also require getting the direction of movement -> as in if the acc vector is
		// pointing down or up. Dot product will be positive if they are pointing to the same direction
		float upProjection = Vector3.Project(accVector, gravityVector).sqrMagnitude;
		upProjection *= Vector3.Dot(accVector, gravityVector) < 0 ? -1 : 1;

		// To enable walking backwards we require checking heads orientation
		// one way you could do is to use euler angles, however projection can be used
		// to simplify the code. We project x axis -> axis pointing forwards from the phone
		// onto gravity axis. 
		//
		// To differentiate from looking down and up, if user is looking down
		// assign it to negative value and if up, then positive
		// TODO Actually instead of gravity can simply use Camera.main.transform.forward
		// TODO and then do projection onto Vector3.up
		float headProjection = Vector3.Project(Vector3.forward, gravityVector).sqrMagnitude;
		headProjection *= Vector3.Dot(Vector3.forward, gravityVector) < 0 ? -1 : 1;

		Debug.Log(string.Format("headProjection={0:0.000}", headProjection));

		// we assign a threshold which is used to determine when to walk backwards
		// For cardboard this value was best around 0.1
		direction = headProjection > walkBackThreshold ? -1 : 1;
		
//		Debug.LogError(string.Format("gravity x={0:0.00}, y={1:0.00}, z={2:0.00}", 
//			gravityVector.x, gravityVector.y, gravityVector.z));
		
//		Debug.Log (string.Format ("Projection, AccY: {0:0.000} {1:0.000}", projection, acc1));
		
		// We gonna use the projection up for walk detection
		allGravity = upProjection;
		
		StepDetectionAlgorithm();
	}
	
	
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	
	//add physics in here
	void FixedUpdate ()
	{
		// We will use a method of adding force to a rigid body here
		// Firslty get the amound of force to be applied
		// and the direction of that force.
		// we wil walk towards where the camera is pointing to, unless direction is -1, which 
		// will reverse the movement.
		// distance is received from a steps analyser
		Vector3 force = Camera.main.transform.forward * thrust * Time.deltaTime * distance * direction;
		
		// If the user is looking down and is trying to walk, the camera.forward
		// if quite a small number towards the horizontal view.
		// We require scaling this. To achieve this:
		//
		// We acquire a vector which will remove the y direction (world axis)
		// We will then multiply the force vector by magnitude of full vector 
		// divided by the magnitude of vector without Up/y axis.
		// So if we are looking down now, forward vector shows small value
		// towards horizontal axis, while the full vector is still big.
		// We divide the full vector by the horizontl values
		// to scale the vector up.
		Vector2 horizontalForce = new Vector2(force.x, force.z);
		// Avoid division by 0
		if (horizontalForce.sqrMagnitude > 0.1)
		{
			force *= force.sqrMagnitude / horizontalForce.sqrMagnitude;
			
		}
		
		// Used for testing when we touch the screen or press mouse
		if (Input.touchCount > 0 || Input.GetMouseButton(0))
		{
//			force = Camera.main.transform.forward * thrust * Time.deltaTime * direction;
			
			CharacterController controller = GetComponent<CharacterController>();
			if (controller.isGrounded) {
				moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= speed;
				if (Input.GetButton("Jump"))
					moveDirection.y = jumpSpeed;
            
			}
			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move(moveDirection * Time.deltaTime);	
		}

		// change to IsGrounded later?
		// This is used to make sure that we can jump
		// Raycast is used to check that we are grounded
		if (canJump)
		{
			force.y = thrust * Time.deltaTime * jump;
			canJump = false;
		}

		// After all this is done, add the force to the object
		// to make it move
		rigidbody.AddForce(force);
		
		// This is used to keep the object certain distance above ground
		// It is also used to set canJump -> as in if the object touches the ground
		// it can now jump. This however allows to jump in the air if you're falling
		// from a cliff
		RaycastHit hit;
		Ray downRay = new Ray(transform.position, -Vector3.up);
		if (Physics.Raycast(downRay, out hit)) {
			float hoverError = hoverHeight - hit.distance;
			if (hoverError > 0)
			{
				canJump = true;
				float upwardSpeed = rigidbody.velocity.y;
				float lift = hoverError * hoverForce - upwardSpeed * hoverDamp;
				rigidbody.AddForce(lift * Vector3.up);
			}
		}	
	}
	
	private void StepDetectionAlgorithm ()
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
		
		// If we are rising and reaching a certain big threshold,
		// we can jump. However real life jump firstly involves 
		// going slightly down and then jump up.
		if ( allGravity > 6*threshold && isRising ){
			distance = 0.7f;
			jump = -0.5f;
		} 
		
		if ( allGravity > 6*threshold && !isRising ){
			distance = 0.5f;
			jump = 4;
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

	// Used to collect acceleration towards up axis data
	// this can later be used to analyse using e.g. python scripts
	private void collectAccData(float acc)
	{
		if (count > 150)
		{
			count = 0;
			Debug.LogError("Samples Taken");

			string printValues = "[";
			data.ForEach(d =>
			{
				printValues += string.Format("{0:0.000}, ", d);
			});
			
			Debug.LogError(printValues);			
			data.Clear();
		}
		
		count++;
		data.Add(acc);
	}
	
	// These are various methods I tried to use to move the object
	// WalkWithVelocity adds velocity to a rigid body while WalkByMovingToPisition
	// interpolates a rigid body to a location. These two are best used with physics.
	// Walk to position is not great for physics as the objects is like teleporting.
	void WalkWithVelocity()
	{
		Vector3 movement = Camera.main.transform.forward * thrust * Time.deltaTime * distance;
		rigidbody.velocity = movement;
	}

	void WalkByMovingToPisition()
	{
		Vector3 movement = transform.position + Camera.main.transform.forward * thrust * Time.deltaTime * distance;
		rigidbody.MovePosition(movement);
	}
	
	void WalkToPosition()
	{
		Vector3 movement = transform.position + Camera.main.transform.forward * thrust * Time.deltaTime * distance;
		transform.position = movement;
	}
}
