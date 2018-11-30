using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Run : MonoBehaviour {

    
    public GameObject player;
    public Rigidbody pBody;
    float speedMod;
    Vector3 zVec, xVec, targVec;

    [Range(1,3)]
    public int speed;
    public float thrust, lrSpeed;

    public string left, right;

    bool grounded;

	// Use this for initialization
	void Start ()
    {
        
        xVec.Set(50, 0, 0);
        zVec = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update ()
    {
        speedMod = (Time.deltaTime / 100000) + 1;

        //Constantly moves the player forward, slowly increasing speed over time
        zVec.z = (speed + speedMod);
        targVec += zVec;
        targVec.y = player.transform.position.y; //Keeps the y value the players, allowing them to jump naturally still
        

        //Adds horizontal commands to the target position if inputed
        if(Input.GetKeyDown(right))
            targVec += xVec;
        else if(Input.GetKeyDown(left))
            targVec -= xVec;


        //Moves the player position to the target position
        player.transform.position = Vector3.MoveTowards(player.transform.position, targVec, 2.5f);

        if (Input.GetAxis("Jump")!= 0 && grounded)
        {
            pBody.velocity = (transform.up * thrust);
            Debug.Log("Yump");

            grounded = false;
        }
        
	}
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
            grounded = true;
    }
}
