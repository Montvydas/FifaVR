using System;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float speed;
//    private Rigidbody robot;
    public Animator animator;

    private int walkHash = Animator.StringToHash("Walk");
    private int idleHash = Animator.StringToHash("Stand");
    
    // Use this for initialization
    void Start () 
    {
//        robot = GetComponent<Rigidbody> ();
        animator = GetComponent<Animator>();
        
    }
	
    // Update is called once per frame
    void Update () 
    {
//        if (Input.GetKey(KeyCode.UpArrow))
//        {
//            //Move the Rigidbody forwards constantly at speed you define (the blue arrow axis in Scene view)
//            transform.velocity = transform.forward * speed;
//            transform.position = transform.position + movement * speed;
//        }
//
//        if (Input.GetKey(KeyCode.DownArrow))
//        {
//            //Move the Rigidbody backwards constantly at the speed you define (the blue arrow axis in Scene view)
//            m_Rigidbody.velocity = -transform.forward * speed;
//        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            //Rotate the sprite about the Y axis in the positive direction
            transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * speed * 5, Space.World);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //Rotate the sprite about the Y axis in the negative direction
            transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * speed * 5, Space.World);
        }
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
            animator.CrossFade(walkHash, 0.0f, -1);
        }
        else
        {
            animator.CrossFade(idleHash, 0.0f, -1);
        }
        
        transform.position = transform.position + movement * speed;
//        robot.AddForce (movement * speed);
    }
}