using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
	public float speed;
	private Rigidbody ball;

	// Use this for initialization
	void Start () 
	{
		ball = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	// All physics go there
	void FixedUpdate() 
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		
		Vector3 val = Input.acceleration;
		Vector3 movement = new Vector3 (Input.acceleration.x, 0, -Input.acceleration.z);
//		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		ball.AddForce (movement * speed);
	}
}
