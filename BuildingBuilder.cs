using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingBuilder : MonoBehaviour
{


    //[SerializeField]
    public int minLanes, minLength;
    public float laneGainRate, obSpawnRate;
    public GameObject[] obstaclePrefabs;
    public int[] availableObs;  //Set in Editor before running

    private int buildingNumber = 0;
    private int laneWidth = 50;
    private int nodeLength = 150;

    private Building currentBuilding, lastBuilding, nextBuilding;
    private GameObject helloBuilding;

    private int zStartDelay = 250;
    private float xFloorScale = 5, zfloorScale = 15;

    private float zOffset, xOffset;

    public struct Building
    {
        public int[,] floor;
        public int number, lanes, length;

    };

    // Use this for initialization
    void Start()
    {
        xOffset = laneWidth / 2;
        zOffset = nodeLength / 2;

        //nextBuilding = Construct();
        Building test;
        int[,] ma = {                                   { 3, 3, 3 }, //x = 0
                                                        { 3, 3, 3 }, //x = 1
                                                        { 3, 3, 3 }, //x = 2
                                        /*-->>*/        { 3, 3, 3 },
                                                        { 3, 3, 3 },
                                                        { 3, 3, 3 }           };
        test.floor = ma;                                                
        test.number = 0;
        test.lanes = 6;
        test.length = 3;

            helloBuilding = PlaceObstacles(test, obstaclePrefabs, 0f);
            
            //nextBuilding = Construct();
            //Instantiate(helloBuilding);

    }

        // Update is called once per frame
        void Update()
        {
        //Cleaning
        Debug.Log(buildingNumber);
        
        }

        //Method for randomly generating a building
        public Building Construct()
        {
            Building nextBuilding = new Building();

            nextBuilding.lanes = minLanes + Random.Range(0, (int)(buildingNumber * laneGainRate));

            nextBuilding.length = minLength + Random.Range(0, 15); //15 is placeholder, add difficulty scaling to building length

            nextBuilding.floor = new int[nextBuilding.lanes, nextBuilding.length];

            for (int i = 0; i < nextBuilding.lanes; i++)
            {
                for (int j = 0; j < nextBuilding.length; j++)
                {
                    nextBuilding.floor[i, j] = availableObs[Random.Range(0, availableObs.Length)];
                }
            }
            
            //If lanes are an even number, offset x position in Unity by 25

            buildingNumber = nextBuilding.number;
            buildingNumber++;
            return nextBuilding;
        }

        private Vector3 ObsPos(int x, int z, Building plan) //Obstacle position
        {
            float offZ,newZ, newX;
            //int newY = 1;
            Vector3 vec3 = new Vector3();

            //Sets the X offset to 25 if the number of lanes in the building is an even number
            /*
            if ((plan.lanes % 2) == 0)
                offX = 25;
            else
                offX = 0;
            */

            //Does the same for the Z offset with building length (number of rows)
            if ((plan.length % 2) == 0)
                offZ = zOffset;
            else
                offZ = 0;

            newX = ((x * laneWidth) - ((int)(plan.lanes / 2) * laneWidth));
            newZ = ((z * nodeLength + zStartDelay) - ((int)(plan.length / 2) * nodeLength + offZ));
            vec3.Set(newX, 3, newZ);

            return vec3;
        }

        public GameObject PlaceObstacles(Building build, GameObject[] obstaclePrefabs, float zStartPos)
        {
            GameObject building = new GameObject();
            building.tag = "Building";
            //GameObject floor = new GameObject();
            GameObject floor = Instantiate(obstaclePrefabs[0], building.transform);
            floor.transform.localScale = new Vector3((build.lanes*xFloorScale), 1, (build.length*zfloorScale));
            floor.transform.position += new Vector3(0, 0, (build.length * (nodeLength/2)));
            
            //Offsets the floor if even lanes
            float offX;
            if ((build.lanes % 2) == 0)
                offX = -xOffset;
            else
                offX = 0;

            if (build.number == 0)
                building.transform.position = new Vector3(offX, 0, 0);         
            else          
                building.transform.position = new Vector3(offX, 0, zStartPos);        

            for (int x = 0; x < build.lanes; x++)
            {
                for (int y = 0; y < build.length; y++)
                {
                    switch (build.floor[x, y])
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            //Spawn AC Unit 1
                            GameObject acUnit1;
                            acUnit1 = Instantiate(obstaclePrefabs[2], ObsPos(x, y, build), building.transform.rotation, building.transform);
                            break;
                        case 3:
                            //Spawn ClothesLine 2 -- Takes up two lanes
                            GameObject clothesLine2;
                            //Checks if its possible to spawn in location
                            if (build.lanes <= (x + 1))
                            {
                                build.floor[x, y] = 0;
                                break; 
                            }
                            //If so, sets the next node to 'Occupied' (1)
                            clothesLine2 = Instantiate(obstaclePrefabs[3], ObsPos(x, y, build), building.transform.rotation, building.transform);
                            build.floor[x + 1, y] = 1;
                            break;

                    }
                }
            }

            return building;

        }

    }

