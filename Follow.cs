using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public GameObject player,cam1;

    Vector3 oldPos, newPos;

	// Use this for initialization
	void Start ()
    {
        
        newPos = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
    {
        oldPos = player.transform.position;

        newPos.z = player.transform.position.z - oldPos.z;
        cam1.transform.position+= newPos;
	}
}
