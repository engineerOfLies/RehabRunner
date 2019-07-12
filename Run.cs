using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Run : MonoBehaviour {

    private const float QUICK_FALL = 18f;
    
    public GameObject player;
    public Rigidbody pBody;
    float speedMod, stumbleTimer, slideTimer, jumpTimer, pointsPerBar;
    Vector3 zVec, xVec, targVec, vel3, velStep, velZero;
    double score = 0;

    [Range(0,3)]
    public float speed;
    public float thrust;
    [Range(0,5)]
    public float strafeSpeed;

    public string left, right;

    [SerializeField]
    private float strafeDistance = 50f, stumbleTimerSet, slideTimerSet, jumpTimerSet;

    bool jumping = false, climbing = false, sliding;

    private GameObject[] buildings;

	// Use this for initialization
	void Start ()
    {
        
        xVec.Set(strafeDistance, 0, 0);
        zVec = Vector3.zero;

        velStep.Set(0, -.1f, 0);
        velZero.Set(0, 0, 0);
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!climbing)
        {
            vel3 = pBody.velocity;
            vel3.y -= QUICK_FALL * Time.deltaTime;
            pBody.velocity = vel3;
        }

        //speedMod = (Time.deltaTime / 100000) + 1; //Too fast, change before reimplementing

        //Constantly moves the player forward, slowly increasing speed over time
        zVec.z = (speed + speedMod);
        
        targVec.y = player.transform.position.y; //Keeps the y value the players

        if (climbing)
        {
            zVec.z = 0f;
            player.transform.position -= zVec;
            targVec.y += 1.8f; // >:(
        }
        

        //Debug.Log(stumbleTimer);

        if(stumbleTimer>0)
        {
            zVec.z = zVec.z * .5f;
            stumbleTimer -= Time.deltaTime;
        }

        if(slideTimer>0)
        {
            slideTimer -= Time.deltaTime;
        }

        if(jumpTimer>0)
        {
            jumpTimer -= Time.deltaTime;
        }

        buildings = GameObject.FindGameObjectsWithTag("Building");

        foreach (GameObject building in buildings)
        {
            building.transform.position -= zVec;
        }
        


        //Adds horizontal commands to the target position if inputed
        //CHANGE TO SET POSITIONS--LESS ROOM FOR ERROR
        if(Input.GetKeyDown(right))
            targVec += xVec;
        else if(Input.GetKeyDown(left))
            targVec -= xVec;


        //Moves the player position to the target position (potentially change)
        player.transform.position = Vector3.MoveTowards(player.transform.position, targVec, strafeSpeed);

        //Allows the player to jump if jump is not on cooldown
        if (Input.GetAxis("Jump")!= 0 && !jumping)
        {
            
            //play jump animation
            
            //Debug.Log("Yump");

            jumping = true;
            jumpTimer = jumpTimerSet; //tie to animation time
        }

        if (Input.GetAxis("Slide")!= 0 && !sliding && !jumping)
        {
            //play slide animation
            sliding = true;
            slideTimer = slideTimerSet;
        }
        
	}

    void StopRunning()
    {


    }

    //Player stumbles over an obstacle and slows down for a moment
    void Stumble()
    {
        stumbleTimer = stumbleTimerSet;
        //call stumble animation also
        //slow speed after?
    }
    
    void OnCollisionExit(Collision collision)
    {

        if(collision.gameObject.tag == "Facade")
        {
            climbing = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Facade")
        {
            //you fell
            //do something with climbing back up, idk
            //CHANGE TO SET POSITIONS REEEE
            climbing = true;

        }
        else if (collision.gameObject.tag == "Wall")
        {
            //the player hit a wall
            //stop until they move back over
        }
        else;
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Slide")
        {
            if (!sliding) Stumble(); // Maybe make a different animation for this?
            else; //good job
            //the player hit an obstacle they had to slide under
            //fall down under it and get up on the other side, slower
        }
        else if (collision.gameObject.tag == "HalfWall")
        {

            if (!jumping) Stumble();
            else; //good job
            //stumble, be it a hurdle of half wall
            //slow down a bit as you trip over obstacle
        }
        else if (collision.gameObject.tag == "Hurdle")
        {
            if (!sliding && !jumping) Stumble(); //the player stumbles if they aren't sliding or jumping
            else; //good job
        }
        else if (collision.gameObject.tag == "GoldBar")
        {
            //get points, add score, good jorb? GO FASTER??
            score += pointsPerBar;
            //def pickup noise, like uh... a 'brring' or somethin, yeah
            //oh and kill the bar
            Destroy(collision.gameObject);
        }
    }
}
