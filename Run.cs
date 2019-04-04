using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Run : MonoBehaviour {

    private const float QUICK_FALL = 18f;
    
    public GameObject player;
    public Rigidbody pBody;
    float speedMod, stumbleTimer;
    Vector3 zVec, xVec, targVec, vel3, velStep, velZero;

    [Range(0,3)]
    public float speed;
    public float thrust;
    [Range(0,5)]
    public float strafeSpeed;

    public string left, right;

    [SerializeField]
    private float strafeDistance = 50f, stumbleTimerSet;

    bool grounded, climbing = false;

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
        if (!grounded)
        {
            vel3 = pBody.velocity;
            vel3.y -= QUICK_FALL * Time.deltaTime;
            pBody.velocity = vel3;
        }

        //speedMod = (Time.deltaTime / 100000) + 1; //Too fast, change before reimplementing

        //Constantly moves the player forward, slowly increasing speed over time
        zVec.z = (speed + speedMod);
        //targVec += zVec;
        targVec.y = player.transform.position.y; //Keeps the y value the players, allowing them to jump naturally still

        if (climbing)
        {
            zVec.z = 0f;
            player.transform.position -= zVec;
            targVec.y += .8f;
        }
        

        Debug.Log(stumbleTimer);

        if(stumbleTimer>0)
        {
            zVec.z = zVec.z * .5f;
            stumbleTimer -= Time.deltaTime;
        }

        buildings = GameObject.FindGameObjectsWithTag("Building");

        foreach (GameObject building in buildings)
        {
            building.transform.position -= zVec;
        }
        

        //Adds horizontal commands to the target position if inputed
        if(Input.GetKeyDown(right))
            targVec += xVec;
        else if(Input.GetKeyDown(left))
            targVec -= xVec;


        //Moves the player position to the target position
        player.transform.position = Vector3.MoveTowards(player.transform.position, targVec, strafeSpeed);

        //Allows the player to jump if theyre colliding with a gameObject with the tag "floor"
        if (Input.GetAxis("Jump")!= 0 && grounded)
        {
            //pBody.constraints = RigidbodyConstraints.None;
            //pBody.constraints = RigidbodyConstraints.FreezeRotation;
            //vel3.Set(0, 2, 0);
            //player.transform.position += vel3;

            pBody.velocity = (transform.up * thrust);
            Debug.Log("Yump");

            grounded = false;
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
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            grounded = false;
            //pBody.constraints = RigidbodyConstraints.None;
            //pBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if(collision.gameObject.tag == "Facade")
        {
            climbing = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            grounded = true;
            //pBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            pBody.velocity = velZero;
            //climbing = false;
        }

        if (collision.gameObject.tag == "Facade")
        {
            //you fell
            //do something with climbing back up, idk
            climbing = true;
            
        }
        else if (collision.gameObject.tag == "Wall")
        {
            //the player hit a wall
            //stop until they move back over
        }
        else if (collision.gameObject.tag == "Slide")
        {
            //the player hit an obstacle they had to slide under
            //fall down under it and get up on the other side, slower
        }
        else if (collision.gameObject.tag == "HalfWall" || collision.gameObject.tag == "Hurdle")
        {
            Stumble();
            //stumble, be it a hurdle of half wall
            //slow down a bit as you trip over obstacle
        }
    }
}
