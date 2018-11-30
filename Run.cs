using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Run : MonoBehaviour {

    
    public GameObject player;
    public Rigidbody pBody;
    float speedMod;
    Vector3 zVec;

    [Range(1,3)]
    public int speed;
    public float thrust;

    bool grounded;

	// Use this for initialization
	void Start ()
    {
       

        zVec = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update ()
    {
        speedMod = (Time.deltaTime / 100000) + 1;

        if (speedMod != 0) 
        {
            zVec.z = (speed + speedMod);
            player.transform.position += zVec;
        }

        
        //Its a jetpack I guess
        
        if(Input.GetAxis("Jump")!= 0 && grounded)
        {
            pBody.AddForce(transform.up * thrust);
            Debug.Log("Yump");

            grounded = false;
        }
        
	}
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plane")
            grounded = true;
    }
}
