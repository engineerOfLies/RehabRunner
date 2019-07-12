using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class BuildingCreator : MonoBehaviour
{

    public float laneWidth, frameLength;

    const float SCALE_SCALER = 10;

    public int now;

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
        public int lastUsed;
    }

    [System.Serializable]
    public struct FrameTypes
    {
        public Frame[] frameTypes;
    }

    [HideInInspector]
    static FrameTypes frameData;
    public int frameScale = 0; //Saves an int to scale the facade based on how many frames
    public int numOfFrames = 0;
    public GameObject leadingFrame; //Saves the first frame in a building

    [Header("Tweak to perfection")]
    [SerializeField]
    private float collectibleHeightOffset, collectibleXOffset;

    // Start is called before the first frame update
    void Start()
    {

        //FrameTypes frameData;

        frameData = ReadFrameData("Assets/data.txt");

        Debug.Log(frameData.frameTypes[1].weight);
    }

    // Update is called once per frame
    void Update()
    {
        //CalculateWeight(frameData);


    }

    public FrameTypes ReadFrameData(string jsonFile)
    {
        string json = File.ReadAllText(jsonFile);
        Debug.Log(json);
        FrameTypes frames = JsonConvert.DeserializeObject<FrameTypes>(json);
        //List<Frame> frameData = new List<Frame>();

        return frames;
    }

    //Function to generate the level
    public void CreateBuilding(FrameTypes listOfFrames)
    {
        //Uses the premade list of frames to create a level
        GameObject building;
        Frame frame;

        int length = 50;
        for (int i = 0; i < length; i++)
        {
            CalculateWeight(listOfFrames);

            //Sort for the heightest weight
            frame = HighestWeightFrame(listOfFrames);
            frame.lastUsed = 0;

            //Call CreateFrame to make each piece based on the Frame's data
            if (frame.repeat > 0)
            {
                //Check for repeats, then simply create the same piece again (Potentially after an empty space?)
                for (int j = 0; j < frame.repeat; j++)
                {
                    CreateFrame(frame);
                }

            }
            else
            {
                CreateFrame(frame);
            }


            //Piece pieces together to create a complete building
            //Use configuration settings(ie. Lane Width, Frame Length, etc.)
        }

        //Return building GameObject to have and reference later potentially
        return;
    }

    //Function to create a set number of frames (n)
    public void CreateBuilding(FrameTypes listOfFrames, int n)
    {
        //Uses the premade list of frames to create a level
        GameObject building;

        Frame frame;

        for (int i = 0; i < n; i++)
        {
            //Run the weight calculations
            CalculateWeight(listOfFrames);

            //Sort for the heightest weight
            frame = HighestWeightFrame(listOfFrames);
            frame.lastUsed = 0;

            //Call CreateFrame to make each piece based on the Frame's data
            if (frame.repeat > 0)
            {
                //Check for repeats, then simply create the same piece again (Potentially after an empty space?)
                for (int j = 0; j < frame.repeat; j++)
                {
                    CreateFrame(frame);
                }

            }
            else
            {
                CreateFrame(frame);
            }



            //Piece pieces together to create a complete building
            //Use configuration settings(ie. Lane Width, Frame Length, etc.)
        }

        //Return building GameObject to have and reference later potentially
        return;
    }

    public GameObject CreateFrame(Frame f) //Actually build the frame using prefabs and other assets
    {
        GameObject framePiece = new GameObject();

        //If there is any gap (object array index 1) dont create frame, just add boards for bridging

        if (f.obstacles[0] == 1 || f.obstacles[1] == 1 || f.obstacles[2] == 1)
        {
            //Dont create facade
            frameScale = 0;

            //Add bridges as needed
            if(f.obstacles[0] != 1)
            {
                //Spawn bridge
            }
            if(f.obstacles[1] != 1)
            {

            }
            if(f.obstacles[2] != 1)
            {

            }
            //Return
        }
        else
        {
            if (frameScale == 0) //If this is the first frame after a gap, remember its location?
            {
                leadingFrame = framePiece;
                //create new facade
            }
            
            frameScale++;
        }


        //Check for what pieces are in the frame
        

        framePiece = PlaceCollectibles(framePiece, f);

        return framePiece;
    }

    //Creates more of the building over time, depending how far the user gets
    public IEnumerator BuildMore()
    {
        //Deterministic? Called when $now reaches _____

        //Call CreateBuilding? Attatch to current Building gameobject

        //Creates more building while game runs in foreground
        yield return new WaitForSeconds(.01f);

    }

    //Calculates weight for all frames in the frameType object
    public void CalculateWeight(FrameTypes data)
    {
        int diff, num;
        float freq;
        num = 0;
        foreach (Frame f in data.frameTypes)
        {
            diff = now - data.frameTypes[num].lastUsed;
            if (diff < data.frameTypes[num].frameDelay)
            {
                data.frameTypes[num].weight = -1;
                Debug.Log("Calculated weight for " + data.frameTypes[num].name + " at " + data.frameTypes[num].weight);
                num++;
                return;
            }
            if ((data.frameTypes[num].frameCap > 0) && (diff > data.frameTypes[num].frameCap))
            {
                data.frameTypes[num].weight = -1;
                Debug.Log("Calculated weight for " + data.frameTypes[num].name + " at " + data.frameTypes[num].weight);
                num++;
                return;
            }
            freq = data.frameTypes[num].frequency - (float)(diff * data.frameTypes[num].frequencyDelta);
            data.frameTypes[num].weight = data.frameTypes[num].priority * diff + data.frameTypes[num].priority * freq * 0.00001f;
            Debug.Log("Calculated weight for " + data.frameTypes[num].name + " at " + data.frameTypes[num].weight);
            num++;
        }

        return;
    }

    public void SortFrames(FrameTypes data)
    {

    }

    public Frame HighestWeightFrame(FrameTypes fr)
    {
        Frame highest;


        highest = fr.frameTypes[0];

        //Searches the list of frames to find the highest weight
        //Then Returns it
        foreach (Frame frm in fr.frameTypes)
        {
            if (frm.weight < highest.weight) continue; //If the current frame'sweight is lower than the highest, skip

            if (frm.weight == highest.weight) // If the current frame's weight is equal to the highest, check priority
            {
                if (frm.priority <= highest.priority) continue;

                highest = frm;
                continue;
            }

            highest = frm;

        }

        return highest;
    }

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
            //Instantiate(coin, new Vector3(-collectibleXOffest, collectibleHeightOffset, center), Quaternion.identity, g);
        }
        if (f.collectables[1] != 0)
        {
            //Instantiate(coin, new Vector3(0, collectibleHeightOffset, center), Quaternion.identity, g);
        }
        if (f.collectables[2] != 0)
        {
            //Instantiate(coin, new Vector3(collectibleXOffest, collectibleHeightOffset, center), Quaternion.identity, g);
        }
        if (f.collectables[3] != 0)
        {
            //Instantiate(coin, new Vector3(-collectibleXOffest, collectibleHeightOffset*2, center), Quaternion.identity, g);
        }
        if (f.collectables[4] != 0)
        {
            //Instantiate(coin, new Vector3(0, collectibleHeightOffset*2, center), Quaternion.identity, g);
        }
        if (f.collectables[5] != 0)
        {
            //Instantiate(coin, new Vector3(collectibleXOffest, collectibleHeightOffset*2, center), Quaternion.identity, g);
        }

        return g;
    }
}

