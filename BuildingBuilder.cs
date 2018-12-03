using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBuilder : MonoBehaviour {

    [SerializeField]
    int minLanes, minLength;
    float laneGainRate, obSpawnRate;

    private int buildingNumber = 0;

    struct Building
    {
        int[,] floor;
        int number,lanes,length;

    }

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //Method for reading the conf file and creating the next building
    Building Construct()
    {
        Building nextBuilding = new Building();

        nextBuilding.lanes = minLanes + Random.Range(0, (int)buildingNumber*laneGainRate);

        nextBuilding.length = minLength + Random.Range(0, 15); //15 is placeholder, add difficulty scaling to building length

        nextBuilding.floor = new int[nextBuilding.lanes, nextBuilding.length];


        //If lanes are an even number, offset x position in Unity by 25

        buildingNumber = nextBuilding.number;
        buildingNumber++;
        return nextBuilding;
    }
}
