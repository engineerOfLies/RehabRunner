using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingBuilder : MonoBehaviour
{


    [SerializeField]
    public int minLanes, minLength;
    public float laneGainRate, obSpawnRate;
    public GameObject[] obstaclePrefabs;

    private int buildingNumber = 0;
    private int laneWidth = 50;
    private int nodeLength = 150;

    private Building currentBuilding, lastBuilding, nextBuilding;
    private GameObject helloBuilding;

    public struct Building
    {
        public int[,] floor;
        public int number, lanes, length;

    };

    // Use this for initialization
    void Start()
    {
        //nextBuilding = Construct();
        Building test;
        int[,] ma = {                                   { 3, 3, 3 }, //x = 0
              /*Player starts here, moves right ->>*/   { 3, 3, 3 }, //x = 1
                                                        { 3, 3, 3 }, // = 2
                                                        { 3, 3, 3 } };
        test.floor = ma;                                                
        test.number = 0;
        test.lanes = 4;
        test.length = 3;

            helloBuilding = PlaceObstacles(test, obstaclePrefabs);

            //Instantiate(helloBuilding);

        }

        // Update is called once per frame
        void Update()
        {
            //Cleaning

        }

        //Method for randomly generating a building
        public Building Construct()
        {
            Building nextBuilding = new Building();

            nextBuilding.lanes = minLanes + Random.Range(0, (int)(buildingNumber * laneGainRate));

            nextBuilding.length = minLength + Random.Range(0, 15); //15 is placeholder, add difficulty scaling to building length

            nextBuilding.floor = new int[nextBuilding.lanes, nextBuilding.length];


            //If lanes are an even number, offset x position in Unity by 25

            buildingNumber = nextBuilding.number;
            buildingNumber++;
            return nextBuilding;
        }

        private Vector3 ObsPos(int x, int z, Building plan) //Obstacle position
        {
            int offZ,newZ, newX;
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
                offZ = 75;
            else
                offZ = 0;

            newX = ((x * laneWidth) - ((int)(plan.lanes / 2) * laneWidth));
            newZ = ((z * nodeLength + 300) - ((int)(plan.length / 2) * nodeLength + offZ));
            vec3.Set(newX, 3, newZ);

            return vec3;
        }

        public GameObject PlaceObstacles(Building build, GameObject[] obstaclePrefabs)
        {
            GameObject building = new GameObject();
            //GameObject floor = new GameObject();
            GameObject floor = Instantiate(obstaclePrefabs[0], building.transform);
            floor.transform.localScale = new Vector3((build.lanes*5), 1, (build.length*15));
            floor.transform.position += new Vector3(0, 0, (build.length * 75));
            
            int offX;
            if ((build.lanes % 2) == 0)
                offX = -25;
            else
                offX = 0;
            

            building.transform.position = new Vector3(offX, 0, 0);

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
                            //Spawn ClothesLine 2
                            GameObject clothesLine2;
                            if (build.lanes <= (x + 1))
                            {
                                build.floor[x, y] = 0;
                                break;
                            }
                            clothesLine2 = Instantiate(obstaclePrefabs[3], ObsPos(x, y, build), building.transform.rotation, building.transform);
                            build.floor[x + 1, y] = 1;
                            break;

                    }
                }
            }

            return building;

        }

    }

