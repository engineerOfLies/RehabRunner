using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Run : MonoBehaviour {

    
    public GameObject player;
    public Rigidbody pBody;
    float speedMod;
    Vector3 zVec, xVec, targVec;

    [Range(0,3)]
    public float speed;
    public float thrust;
    [Range(0,5)]
    public float strafeSpeed;

    public string left, right;

    [SerializeField]
    private float strafeDistance = 50;

    bool grounded, climbing = false;

    private GameObject[] buildings;

	// Use this for initialization
	void Start ()
    {
        
        xVec.Set(strafeDistance, 0, 0);
        zVec = Vector3.zero;

        
    }
	
	// Update is called once per frame
	void Update ()
    {
        //speedMod = (Time.deltaTime / 100000) + 1; //Too fast, change before reimplementing

        //Constantly moves the player forward, slowly increasing speed over time
        zVec.z = (speed + speedMod);
        //targVec += zVec;
        targVec.y = player.transform.position.y; //Keeps the y value the players, allowing them to jump naturally still

        if(climbing)
        {
            zVec.z = .1f;
            player.transform.position -= zVec;
            targVec.y += .3f;
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
            pBody.velocity = (transform.up * thrust);
            Debug.Log("Yump");

            grounded = false;
        }
        
	}
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            grounded = true;
            climbing = false;
        }
        if (collision.gameObject.tag == "Facade")
        {
            //you fell
            //do something with climbing back up, idk
            climbing = true;
            
        }
    }
}
