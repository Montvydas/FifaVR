using System;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float speed;
//    private Rigidbody robot;
    public Animator animator;

    // Use this for initialization
    void Start () 
    {
//        robot = GetComponent<Rigidbody> ();
        animator = GetComponent<Animator>();
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
        Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

        if (Math.Abs(moveVertical) > 0 || Math.Abs(moveHorizontal) > 0.0)
        {
//            animator.Play("Robot Armature|Action Walk Done", -1, 0f);
            animator.CrossFade("Walk", 0.0f, -1);
        }
        else
        {
            animator.CrossFade("Stand", 0.0f, -1);
        }
        
        transform.position = transform.position + movement * speed;
//        robot.AddForce (movement * speed);
    }
}