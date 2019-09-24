using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
//using Newtonsoft.Json;

public class BuildingCreator : MonoBehaviour
{

    public float laneWidth, frameLength;

    const float SCALE_SCALER = 10;
    Vector3 origin, lastFramePos;

    [HideInInspector]
    public float now;

    [System.Serializable]
    public struct Frame
    {
        public string name;
        public int[] obstacles;
        public int[] collectables;
        public int frameDelay;
        public int frameCap;
        public float frequency;
        public double frequencyDelta;
        public int repeat;
        public float priority;
        public float weight;
        public float lastUsed;
    }

    [System.Serializable]
    public struct FrameTypes
    {
        public Frame[] frameTypes;
    }

    [System.Serializable]
    public struct WorldSave
    {
        public string name;
        public float timePlayerTouch;
    }

    [SerializeField]
    bool save = true;
    [SerializeField]
    bool json = true;

    bool loadLevel = false;

    [HideInInspector]
    public List<WorldSave> world;

    [HideInInspector]
    [System.Serializable]
    public struct WorldStruct
    {
        public List<WorldSave> worlds;
    }

    [HideInInspector]
    public WorldStruct jsonData;

    [HideInInspector]
    public static FrameTypes frameData;
    [HideInInspector]
    public int frameScale = 0; //Saves an int to scale the facade based on how many frames
    [HideInInspector]
    public int numOfFrames = 0;
    [HideInInspector]
    public GameObject leadingFrame, lastFrame; //Saves the first frame in a building

    bool startFrame = true;

    int spawnTrigger = 0; //Every frame created, increment by 1. At 10, spawn trigger to spawn more frames

    [Header("Tweak to perfection")]
    [SerializeField]
    private float collectibleHeightOffset;
    [SerializeField]
    private float collectibleXOffset;
    [SerializeField]
    private float obstacleXOffset;

    [Header("List of Building Prefabs")]
    public GameObject[] buildingPrefabs;

    [Header("List of Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;

    const int MAX_BUILDINGS = 5;
    [HideInInspector]
    public int numBuildings = 0;

    bool prevGap = false;
    Vector3 fabScale;

    int cloudOffset = 300;

    [HideInInspector]
    public string fileSaveLocation;
    [HideInInspector]
    public string configFilePath;

    string playerName;

    //IEnumerator CoBuild;

    // Start is called before the first frame update
    void Start()
    {
        #region PlayerPrefs Loading

        if (PlayerPrefs.GetInt("Save?", 1) > 0) save = true;
        else save = false;

        if (PlayerPrefs.GetInt("SaveAsJson?", 1) > 0) json = true;
        else json = false;

        fileSaveLocation = PlayerPrefs.GetString("SaveDataLocation", "");

        configFilePath = PlayerPrefs.GetString("WorldConfig", "");

        playerName = PlayerPrefs.GetString("PlayerName", "");

        if (PlayerPrefs.GetInt("LoadLevel?", 0) > 0) loadLevel = true;
        else loadLevel = false;
        #endregion

        origin.Set(0, -1.57f, -(frameLength/2));
        fabScale.Set((float)laneWidth, 1f, (float)frameLength);

        GameObject first = new GameObject();
        first.transform.position = origin;
        lastFrame = first;
        lastFramePos = origin;
        //FrameTypes frameData;

        if (configFilePath != "")
        {
            frameData = ReadFrameData(configFilePath);
        }
        else
        {
            frameData = ReadFrameData(Application.dataPath + "/data.txt");
        }

        Debug.Log(frameData.frameTypes[1].weight);
        Debug.Log(frameData.frameTypes.Length);

        //If a pregenerated level is selected, create that instead of generating one
        if (loadLevel)
        {

        }
        else
        {
            CreateBuilding(frameData);
        }
        

        

        //Create empty frames to start off
        //Debug.Log("Frame length :" + frameLength);

        //rand = new System.Random();

        SpawnClouds();
    }

    // Update is called once per frame
    void Update()
    {
        //CalculateWeight();

    }

    /**
    *   @brief Function that reads in the list of frame types from a file
    *   
    *   @param path of file to be read in
    *   @return FrameTypes object with a list of all the types of frames tha can be used
    */
    public FrameTypes ReadFrameData(string jsonFile)
    {
        string json = File.ReadAllText(jsonFile);
        Debug.Log(json);
        FrameTypes frames = JsonUtility.FromJson<FrameTypes>(json);
        //List<Frame> frameData = new List<Frame>();

        return frames;
    }

    /**
    *   @brief Function to generate the level, creates 20 frames
    *  
    *   @param FrameTypes object read in from ReadFrameData, containing list of frames
    */
    public void CreateBuilding(FrameTypes listOfFrames)
    {
        //Uses the premade list of frames to create a level
        GameObject building = new GameObject();
        building.tag = "Building"; building.name = "Building";
        BoxCollider bc = building.AddComponent<BoxCollider>(); bc.isTrigger = true;
        bc.transform.position = lastFrame.transform.position;
        Frame frame;

        numBuildings++;

        int length = 20;
        for (int i = 0; i < length; i++)
        {
            //Copy ------------
            CalculateWeight();

            //Sort for the heightest weight
            frame = HighestWeightFrame();
            frame.lastUsed = Time.timeSinceLevelLoad;

            //Call CreateFrame to make each piece based on the Frame's data
            if (frame.repeat > 0)
            {
                //Check for repeats, then simply create the same piece again (Potentially with an empty space between?)
                for (int j = 0; j < frame.repeat; j++)
                {
                    CreateFrame(frame, building);

                    i++;
                    if (i == length) break;
                }

                
            }
            else
            {
                CreateFrame(frame, building);
            }

            //Piece pieces together to create a complete building
            //Use configuration settings(ie. Lane Width, Frame Length, etc.)
        }

        //Return building GameObject to have and reference later potentially?
        return;

        //Copy --------------
    }

    /**
    *   @brief Function to generate the level, creates n frames
    *  
    *   @param FrameTypes object read in from ReadFrameData, containing list of frames
    *   @param number of frames to generate
    */
    public void CreateBuilding(FrameTypes listOfFrames, int n)
    {
        //Uses the premade list of frames to create a level
        GameObject building = new GameObject();
        building.tag = "Building"; building.name = "Building";
        BoxCollider bc = building.AddComponent<BoxCollider>(); bc.isTrigger = true;
        bc.transform.position = lastFrame.transform.position;
        Frame frame;

        numBuildings++;
        
        for (int i = 0; i < n; i++)
        {
            //Run the weight calculations
            CalculateWeight();

            //Sort for the heightest weight
            frame = HighestWeightFrame();
            //frame.lastUsed = Time.time;

            //Call CreateFrame to make each piece based on the Frame's data
            if (frame.repeat > 0)
            {
                //Check for repeats, then simply create the same piece again (Potentially with an empty space between?)
                for (int j = 0; j < frame.repeat; j++)
                {
                    CreateFrame(frame, building);

                    i++;
                    if (i >= n) break;
                }

                
            }
            else
            {
                CreateFrame(frame, building);
            }

            //Piece pieces together to create a complete building
            //Use configuration settings(ie. Lane Width, Frame Length, etc.)
        }

        //Return building GameObject to have and reference later potentially?
        return;

    }

    /**
    *   @brief Creates the frame gameObject in unity
    *
    *   @param  Frame object from the list of frame types in FrameTypes object
    *   @param  the parent building this frame will be attatched to
    *   @return the frame gameObject to be placed in the world
    */
    public GameObject CreateFrame(Frame f, GameObject parent) //Actually build the frame using prefabs and other assets
    {
        f.lastUsed = Time.timeSinceLevelLoad;
        Vector3 sVec;
        sVec = lastFrame.transform.position;
        sVec.z += frameLength;

        GameObject framePiece = new GameObject();
        framePiece.tag = "Frame";
        //Create empty frame piece as a child under building
        framePiece.transform.SetParent(parent.transform, true);
        framePiece.transform.position = sVec;
        framePiece.name = f.name;

        BoxCollider boxCollider = framePiece.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(20f, 10f, .1f);

        //Determine the position of this frame


        spawnTrigger++; numOfFrames++;

        //If there is any gap (object array index 1) create end frame, add boards for bridging as needed

        if (f.obstacles[0] == 1 || f.obstacles[1] == 1 || f.obstacles[2] == 1)
        {
            //Dont create facade
            
            
            //

            //Add bridges as needed
            if(f.obstacles[0] != 1)
            {
                SpawnPlank(framePiece, 0);
            }
            else Instantiate(obstaclePrefabs[2], new Vector3(-obstacleXOffset, 0f, framePiece.transform.position.z), Quaternion.identity, framePiece.transform);
            if (f.obstacles[1] != 1)
            {
                SpawnPlank(framePiece, 1);
            }
            else Instantiate(obstaclePrefabs[2], new Vector3(0f, 0f, framePiece.transform.position.z), Quaternion.identity, framePiece.transform);
            if (f.obstacles[2] != 1)
            {
                SpawnPlank(framePiece, 2);
            }
            else Instantiate(obstaclePrefabs[2], new Vector3(obstacleXOffset, 0f, framePiece.transform.position.z), Quaternion.identity, framePiece.transform);

            startFrame = true; //Sets to true so next piece will have the special starting frame
            //Return
        }
        else
        {
            
            if (startFrame) //Checks if this frame starts the next chunk (after gap)
            {
                //Uses a special starting frame slice
                framePiece.name += " - starter";
                GameObject startFloor = Instantiate(buildingPrefabs[2], sVec, Quaternion.identity, framePiece.transform);
                startFloor.transform.localScale = fabScale;
                startFrame = false; //Resets it back to false after being used
            }
            else
            {
                GameObject floor = Instantiate(buildingPrefabs[0], sVec, Quaternion.identity, framePiece.transform);
                floor.transform.localScale = fabScale;
            }

            if(f.obstacles.Sum() != 0)
            {
                framePiece = SpawnObstacles(framePiece, f);
            }
   
        }
        

        //Check for what pieces are in the frame

        if (f.collectables.Sum() != 0)
        {
            framePiece = PlaceCollectibles(framePiece, f);
        }

        if(spawnTrigger >= 5 ) //Once 10 frames are created, create a trigger to spawn another set when the player passes through
        {
            Instantiate(buildingPrefabs[3], framePiece.transform.position, Quaternion.identity, framePiece.transform);
            framePiece.name += " /w Spawn Trigger";
            spawnTrigger = 0;
        }

        lastFrame = framePiece;
        //lastFramePos = lastFrame.transform.position;
        return framePiece;
    }

    //Creates more of the building over time, depending how far the user gets
    public IEnumerator BuildMore()
    {
        //Deterministic? Called when $now reaches _____

        //Call CreateBuilding? Attatch to current Building gameobject
        CreateBuilding(frameData, 20);

        //Creates more building while game runs in foreground

        
        yield return new WaitForSeconds(.01f);
        //StopCoroutine(CoBuild);
    }

    /**
    *   @brief Calculates weight for all frames in the frameType object
    */
    public void CalculateWeight()
    {

        //int num = 0;
        float freq, diff;       
        for(int num = 0; num < frameData.frameTypes.Length; num++)
        {
            now = Time.timeSinceLevelLoad * 1000; //In milliseconds
            diff = now - frameData.frameTypes[num].lastUsed;
            if (diff < frameData.frameTypes[num].frameDelay)
            {
                frameData.frameTypes[num].weight = -1;
                Debug.Log("Calculated weight for " + frameData.frameTypes[num].name + " at " + frameData.frameTypes[num].weight);
                num++;
                continue;
            }
            if ((frameData.frameTypes[num].frameCap > 0) && (diff > frameData.frameTypes[num].frameCap))
            {
                frameData.frameTypes[num].weight = -1;
                Debug.Log("Calculated weight for " + frameData.frameTypes[num].name + " at " + frameData.frameTypes[num].weight);
                num++;
                continue;
            }
            freq = frameData.frameTypes[num].frequency - (float)(diff * frameData.frameTypes[num].frequencyDelta); //Check
            frameData.frameTypes[num].weight = (frameData.frameTypes[num].priority * diff) + (frameData.frameTypes[num].priority * freq * 0.00001f);
            Debug.Log("Calculated weight for " + frameData.frameTypes[num].name + " at " + frameData.frameTypes[num].weight);
            //num++;
        }

        return;
    }

    public void SortFrames(FrameTypes data)
    {

    }

    /**
    *   @brief Finds the frame with the highest weight in the list of frames (FrameTypes object) 
    *   
    *   @return the frame with the heighest weight
    */
    public Frame HighestWeightFrame()
    {
        Frame highest;

        int s = 0; //Tracks the spot in the array of the highest weight frame
        int r;
        highest = frameData.frameTypes[0];

        //Searches the list of frames to find the highest weight
        //Then Returns it
        for (int i = 0; i < frameData.frameTypes.Length; i++)
        {
            if (frameData.frameTypes[i].weight < highest.weight) continue; //If the current frame'sweight is lower than the highest, skip

            if (frameData.frameTypes[i].weight == highest.weight) // If the current frame's weight is equal to the highest, check priority
            {
                if (frameData.frameTypes[i].priority < highest.priority) continue;
                if (frameData.frameTypes[i].priority > highest.priority)
                {
                    highest = frameData.frameTypes[i]; s = i;
                    continue;
                }
               
                if((int)(Random.value*100) > 35)
                {
                    highest = frameData.frameTypes[i]; s = i;
                    continue;
                }
                continue;
            }

            highest = frameData.frameTypes[i]; s = i;

        }

        Debug.Log(frameData.frameTypes[s].name + " is the highest weight at " + frameData.frameTypes[s].weight);
        if (frameData.frameTypes[s].name == "gap")
        {
            for (int k = 0; k < frameData.frameTypes.Length; k++)
            {
                if (frameData.frameTypes[k].name == "gap")
                {
                    frameData.frameTypes[k].lastUsed = Time.time * 1000;
                    frameData.frameTypes[k].weight = 0;
                }
            }

        }
        else
        {
            frameData.frameTypes[s].lastUsed = Time.timeSinceLevelLoad * 1000;
            frameData.frameTypes[s].weight = 0;
        }
        
        return highest;
    }

    /**
    *   @brief Places collectibles on existing frame
    *   
    *   @param  the gameObject of the frame, generated from CreateFrame
    *   @param  the frame that collectibles are being generated for
    *   @return frame gameObject with collectibles placed on it
    */
    public GameObject PlaceCollectibles(GameObject g, Frame f)
    {
        //Create if statements for all numbers (1-6) and spawn in collectibles based on the number at the index
        //Read from file to populate possible collectibles?


        //Find the center (z) of current frame
        float center = g.transform.position.z;

        //Make a selector for the prefab instantiated?

        if(f.collectables[0] != 0)
        {
            //switch case it if more than single coin object
            //for now, dont worry
            Instantiate(buildingPrefabs[4], new Vector3(-collectibleXOffset, collectibleHeightOffset, center), Quaternion.identity, g.transform);
        }
        if (f.collectables[1] != 0)
        {
            Instantiate(buildingPrefabs[4], new Vector3(0, collectibleHeightOffset, center), Quaternion.identity, g.transform);
        }
        if (f.collectables[2] != 0)
        {
            Instantiate(buildingPrefabs[4], new Vector3(collectibleXOffset, collectibleHeightOffset, center), Quaternion.identity, g.transform);
        }
        if (f.collectables[3] != 0)
        {
            Instantiate(buildingPrefabs[5], new Vector3(-collectibleXOffset, collectibleHeightOffset+3f, center), Quaternion.identity, g.transform);
        }
        if (f.collectables[4] != 0)
        {
            Instantiate(buildingPrefabs[5], new Vector3(0, collectibleHeightOffset+3f, center), Quaternion.identity, g.transform);
        }
        if (f.collectables[5] != 0)
        {
            Instantiate(buildingPrefabs[5], new Vector3(collectibleXOffset, collectibleHeightOffset+3f, center), Quaternion.identity, g.transform);
        }

        return g;
    }

    /**
    *   @brief Places obstacles on existing frame
    *   
    *   @param  the gameObject of the frame, generated from CreateFrame
    *   @param  the frame that obstacles are being generated for
    *   @return frame gameObject with obstacles placed on it
    */
    public GameObject SpawnObstacles(GameObject g, Frame f)
    {
        bool b = false;

        for (int i = 0; i < 3; i++)
        {
            switch (f.obstacles[i])
            {
                case 0:
                    break;
                case 2: //power lines -- check if whole frame wide
                    //if(i+1 < 3) //Dont want an out of bounds error
                    //{
                    //    if(f.obstacles[i+1] == 2) // Checks if the next spot is a powerline.
                    //    {
                    //        if(i+1 <3)
                    //        {
                    //            if(f.obstacles[i + 1] == 2) //Check the next
                    //            {
                    //                //spawn a 3 wide powerline over the whole frame
                    //                //Instantiate(whatever it is)
                    //                //also break out the whole for loop with b, because nothing else can spawn
                    //                b = true;
                    //                break;
                    //            }
                    //        }
                    //        //If it is just a 2 wide, spawn that

                    //        break;
                    //    }
                    //    else //If not, spawn a single then break
                    //    {
                    //        Instantiate(obstaclePrefabs[4], Spot(i, g), Quaternion.identity, g.transform);
                    //        break;
                    //    }
                        
                    //}
                    Instantiate(obstaclePrefabs[4], (Spot(i, g) + new Vector3(0f, 3.5f, 0f)), Quaternion.identity, g.transform);
                    break;
                case 3: //wall

                    Instantiate(obstaclePrefabs[1], Spot(i, g), Quaternion.identity, g.transform);
                    break;

                case 4: //Half Wall (Jump)

                    Instantiate(obstaclePrefabs[3], (Spot(i, g) + new Vector3(0f, 1f, 0f)), Quaternion.identity, g.transform);
                    break;

                case 5: //Slide

                    Instantiate(obstaclePrefabs[4], (Spot(i, g) + new Vector3(0f, 3.5f, 0f)), Quaternion.identity, g.transform);
                    break;

                case 6: //Hurdle (Slide or Jump)

                    Instantiate(obstaclePrefabs[5], (Spot(i, g) + new Vector3(0f, 2.5f, 0f)), Quaternion.identity, g.transform);
                    break;

                default:
                    Debug.Log("Object " + f.obstacles[i] + " not set in prefab list");
                    break;
            }

            if (b) break;
        }

        return g;
    }

    /**
    *   @brief Simple function to decide where to spawn objects
    * 
    *   @param lane number item is being placed in (0, 1, or 2)
    *   @param gameObject being placed on, used for transform locations for the children objects being placed
    *   @return location in world space of object being placed
    */
    Vector3 Spot(int i, GameObject g) //Simple function to decide where to spawn objects
    {
        Vector3 vec = Vector3.zero;
        float x = 0;

        if (i == 0) x = g.transform.position.x - obstacleXOffset;

        if (i == 2) x = g.transform.position.x + obstacleXOffset;

        vec.Set(x, g.transform.position.y, g.transform.position.z);

        return vec;
    }

    /**
    *   @brief Function to spawn plank(s) on gap frames
    * 
    *   @param object that the planks will be attatched to
    *   @param lane where plank is being spawned (0, 1, or 2)
    */
    void SpawnPlank(GameObject frame, int n)
    {
        //GameObject plank = new GameObject();
        //plank.transform.position = frame.transform.position;
        //plank.name = "Plank";
        switch (n)
        {
            case 0:
                GameObject plank = Instantiate(buildingPrefabs[1], (Spot(0, frame) + new Vector3(0f, 0.5f, 0f)), Quaternion.identity, frame.transform);
                plank.name = "Plank";
                break;
            case 1:
                plank = Instantiate(buildingPrefabs[1], (Spot(1, frame) + new Vector3(0f, 0.5f, 0f)), Quaternion.identity, frame.transform);
                plank.name = "Plank";
                break;
            case 2:
                plank = Instantiate(buildingPrefabs[1], (Spot(2, frame)+new Vector3(0f,0.5f,0f)), Quaternion.identity, frame.transform);
                plank.name = "Plank";
                break;
            default:
                Debug.Log("Wrong position selected");
                break;
        }
    }

    /**
    *   @brief Spawns clouds 
    */
    public void SpawnClouds()
    {
        Instantiate(buildingPrefabs[6], new Vector3(72f, 14f, (float)cloudOffset), Quaternion.identity);
    }

    /**
    *   @brief Unity Function - Checks when this object collides with another object
    * 
    *   @param collision info of object that collided (including object that collided)
    */
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "SpawnTrigger" && (numBuildings < MAX_BUILDINGS))
        {
            CreateBuilding(frameData, 20);
            //StartCoroutine(CoBuild);
            //BuildMore();
        }

        if(col.tag == "Frame" && save)
        {
            if (!json)
            {
                world.Add(new WorldSave() { name = col.gameObject.name, timePlayerTouch = Time.timeSinceLevelLoad * 1000f });
            }
            else
            {
                jsonData.worlds.Add(new WorldSave() { name = col.gameObject.name, timePlayerTouch = Time.timeSinceLevelLoad * 1000f });
            }
        }
    }

    /**
    *   @brief Writes the world data file of the game just played
    */
    void WriteFile()
    {
        string ext;              
        bool c = true;
        int i = 1;
        string fileName = "t", line;

        ext = (json) ? ".txt" : ".csv";

        string dateTime = System.DateTime.UtcNow.ToString("HH-mm-ss--dd-MMMM-yyyy");

        //while (c)
        //{
            fileName = "/WorldData "+playerName+ " " + dateTime + ext;
            //if (File.Exists(fileName)) i++;     //Check if the file exists, then increments i as needed
            //else c = false;
        //}
        StreamWriter sw = new StreamWriter(fileSaveLocation+fileName);
        if (!json)
        {
            //Write list of saved data to file
            string header = "Name of Frame, Time Reached(Milliseconds)";
            sw.WriteLine(header);
            foreach (var WorldSave in world)
            {
                line = WorldSave.name + "," + WorldSave.timePlayerTouch;
                sw.WriteLine(line);
            }
        }
        else
        {
            sw.Write(JsonUtility.ToJson(jsonData));
        }

        sw.Flush();
        sw.Close();

        fileName = "/lastRunW";
        StreamWriter sw2 = new StreamWriter(fileSaveLocation + fileName);
        if (!json)
        {
            //Write list of saved data to file
            string header = "Name of Frame, Time Reached(Milliseconds)";
            sw2.WriteLine(header);
            foreach (var WorldSave in world)
            {
                line = WorldSave.name + "," + WorldSave.timePlayerTouch;
                sw2.WriteLine(line);
            }
        }
        else
        {
            sw2.Write(JsonUtility.ToJson(jsonData));
        }

        sw2.Flush();
        sw2.Close();

    }

    /**
    *   @brief Unity Function - Executes when game is closed
    */
    void OnApplicationQuit()
    {
        if (save) WriteFile();
    }
}

