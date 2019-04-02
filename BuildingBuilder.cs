using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingBuilder : MonoBehaviour
{


    
    public int minLanes, minLength;
    public float laneGainRate, obSpawnRate;

    [SerializeField]
    private int laneWidth = 20, nodeLength = 150;
    [SerializeField]
    private float gapLength = 75f;

    private int buildingNumber = 0;

    public GameObject[] obstaclePrefabs;
    public int[] availableObs;  //Set in Editor before running

    private Building previousBuilding;
    private GameObject bild;

    [HideInInspector]
    public GameObject currentBuild, lastBuild, nextBuild;

    private int zStartDelay = 250;
    private float yBuildingScale = 150f;
    private float xFloorScale, zFloorScale;

    private float zOffset, xOffset;
    private Vector3 newBuildPos;

    public struct Building
    {
        public int[,] floor;
        public int number, lanes, length;

    };

    // Use this for initialization
    void Start()
    {
        newBuildPos = Vector3.zero;

        xOffset = laneWidth / 2;
        zOffset = nodeLength / 2;

        xFloorScale = laneWidth / 10;
        zFloorScale = nodeLength / 10;

        //Creates a specific building for test purposes
        Building test;
        int[,] ma = {                                   { 0, 3, 3 }, //x = 0
                                                        { 0, 0, 3 }, //x = 1
                                                        { 3, 0, 4 }, //x = 2
                                        /*-->>*/        { 0, 3, 0 },
                                                        { 3, 3, 0 },
                                                        { 3, 3, 0 }           };
        test.floor = ma;                                                
        test.number = 0;
        test.lanes = 6;
        test.length = 3;
        

        bild = PlaceObstacles(test, obstaclePrefabs);
        buildingNumber++;

        currentBuild = bild;

        bild = PlaceObstacles(Construct(), obstaclePrefabs);

        nextBuild = bild;

        SpawnNewBuilding();
       
    }

    // Update is called once per frame
    void Update()
    {
        

    Debug.Log(buildingNumber);
        
    }

    //Method for randomly generating a building
    public Building Construct()
    {
        Building nextBuilding = new Building();

        nextBuilding.lanes = minLanes + Random.Range(0, 4); //(int)(buildingNumber * laneGainRate));

        nextBuilding.length = minLength + Random.Range(0, 4); //number is placeholder, add difficulty scaling to building length

        nextBuilding.floor = new int[nextBuilding.lanes, nextBuilding.length];

        for (int i = 0; i < nextBuilding.lanes; i++)
        {
            for (int j = 0; j < nextBuilding.length; j++)
            {
                nextBuilding.floor[i, j] = availableObs[Random.Range(0, availableObs.Length)];
            }
        }
            
            

        nextBuilding.number = buildingNumber;
        buildingNumber++;
        return nextBuilding;
    }

    public float FindStartPos(Building wah)
    {
        return ((wah.length * nodeLength) + gapLength + newBuildPos.z);
    }

    private Vector3 ObsPos(int x, int z, Building plan) //Obstacle position
    {
        float offZ,newZ, newX;
            
        Vector3 vec3 = new Vector3();

           

        //Changes the Z offset based off the building length (number of rows)
        if ((plan.length % 2) == 0)
            offZ = zOffset;
        else
            offZ = 0;

        newX = ((x * laneWidth) - ((int)(plan.lanes / 2) * laneWidth));
        newZ = ((z * nodeLength + zStartDelay) - (((int)(plan.length / 2) * nodeLength) + offZ));
        newZ += (newBuildPos.z + (buildingNumber)*(offZ));

        if (buildingNumber >= 1 )
        newZ += ((zOffset * plan.length) - (zOffset*3));

        vec3.Set(newX, 3, newZ);
        //Rework for longevity 


        return vec3;
    }

    public GameObject PlaceObstacles(Building build, GameObject[] obstaclePrefabs)
    {
        GameObject building = new GameObject();
        building.tag = "Building";
            
        GameObject floor = Instantiate(obstaclePrefabs[0], building.transform);
        floor.transform.localScale = new Vector3((build.lanes*xFloorScale), 1, (build.length*zFloorScale));
        floor.transform.position += new Vector3(0, 0, (build.length * (nodeLength/2)));

        GameObject facade = Instantiate(obstaclePrefabs[1], floor.transform.position, building.transform.rotation, building.transform);
        facade.transform.localScale = new Vector3(build.lanes*xFloorScale, yBuildingScale, build.length*zFloorScale);
        //GameObject cube = facade.transform.GetChild(0).gameObject;
        BoxCollider boxCollider = facade.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
            
        //Offsets the floor if even lanes
        float offX;
        if ((build.lanes % 2) == 0)
            offX = -xOffset;
        else
            offX = 0;

        if (build.number == 0)
            building.transform.position = new Vector3(offX, 0, 0);
        else
            building.transform.position = new Vector3(offX, 0, FindStartPos(previousBuilding));

        newBuildPos = building.transform.position;

        //Places obstacles on the building
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
                        //Spawn AC Unit 1 // 
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
                    case 4:
                        //Spawn GoldBar1
                        GameObject goldBar1;
                        goldBar1 = Instantiate(obstaclePrefabs[4], ObsPos(x, y, build), building.transform.rotation, building.transform);
                        break;

                }
            }
        }


        previousBuilding = build;
        return building;

    }

    public void SpawnNewBuilding()
    {
        Destroy(lastBuild.gameObject);
        lastBuild = currentBuild;
        currentBuild = nextBuild;
        bild = PlaceObstacles(Construct(), obstaclePrefabs);
        nextBuild = bild;
    }

}

