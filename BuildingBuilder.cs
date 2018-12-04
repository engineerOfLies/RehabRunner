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

        public struct Building
        {
            public int[,] floor;
            public int number, lanes, length;

        };

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //Method for reading the conf file and creating the next building
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
            int offX, offZ, newX, newZ;
            int newY = 3;
            Vector3 vec3 = new Vector3();

            //Sets the X offset to 25 if the number of lanes in the building is an even number
            if ((plan.lanes % 2) == 1)
                offX = 25;
            else
                offX = 0;

            //Does the same for the Z offset with building length (number of rows)
            if ((plan.length % 2) == 1)
                offZ = 75;
            else
                offZ = 0;

            newX = ((x * laneWidth) - ((int)(plan.lanes / 2) * laneWidth) + offX);
            newZ = ((z * nodeLength + 300) - ((int)(plan.length / 2) * nodeLength + offZ));
            vec3.Set(newX, newY, newZ);

            return vec3;
        }

        public GameObject PlaceObstacles(Building build, GameObject[] obstaclePrefabs)
        {
            GameObject building = new GameObject();
            building = Instantiate(obstaclePrefabs[0]);

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

                    }
                }
            }

            return building;

        }

    }

