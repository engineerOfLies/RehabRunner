using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;


public class Run : MonoBehaviour {

    private const float QUICK_FALL = 18f;
    
    public GameObject player;
    public Rigidbody pBody;
    float speedMod, stumbleTimer, slideTimer, jumpTimer, pointsPerBar;
    Vector3 zVec, xVec, targVec, vel3, velStep, velZero;
    double score = 0;
    int coins = 0;

    [Range(3,20)]
    public float speed;
    public float thrust;
    [Range(0,5)]
    public float strafeSpeed;

    private Animator anim;
    private GameObject bab;
    
    string left = "a" , right = "d";

    [SerializeField]
    private float strafeDistance = 50f, stumbleTimerSet, slideTimerSet, jumpTimerSet;

    bool jumping = false, climbing = false, sliding = false;
    bool leftLane = false, rightLane = false;

    private GameObject[] buildings;

    Renderer rend;

    [System.Serializable]
    public struct SaveData
    {
        public float timePerformed; //in milliseconds
        public string action; //the action (or collision) performed
    }
    
    [HideInInspector]
    public List<SaveData> saves;

    [System.Serializable]
    public struct SaveDataJson
    {
        public List<SaveData> savess;
    }
    [HideInInspector]
    public SaveDataJson saveDataJson;

    [SerializeField]
    bool save = true; //Set to true to save data
    [SerializeField]
    bool json = true;

    public Text coinText, copDist;
    public Slider distanceTracker;
    float dist = 0f;
    public float distanceFromRobber;

    [HideInInspector]
    public string fileSaveLocation;

    string playerName;

	// Use this for initialization
	void Start ()
    {
        #region PlayerPref Loading

        if (PlayerPrefs.GetInt("Save?", 1) > 0) save = true;
        else save = false;

        if (PlayerPrefs.GetInt("SaveAsJson?", 1) > 0) json = true;
        else json = false;

        if (PlayerPrefs.GetInt("EndlessMode?", 0) > 0) ; //Do something with endless mode
        else;

        fileSaveLocation = PlayerPrefs.GetString("SaveDataLocation", "");

        playerName = PlayerPrefs.GetString("PlayerName", "");
        #endregion

        bab = this.gameObject.transform.GetChild(0).gameObject;

        xVec.Set(strafeDistance, 0, 0);
        zVec = Vector3.zero;

        velStep.Set(0, -.1f, 0);
        velZero.Set(0, 0, 0);

        rend = bab.GetComponent<Renderer>();
        anim = bab.GetComponent<Animator>();

        //coinText.text = "Coins: "+coins;
        distanceTracker.maxValue = distanceFromRobber;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!climbing)
        {
            //vel3 = pBody.velocity;
            //vel3.y -= QUICK_FALL * Time.deltaTime;
            //pBody.velocity = vel3;
        }

        //speedMod = (Time.deltaTime / 100000) + 1; //Too fast, change before reimplementing

        //Constantly moves the player forward, slowly increasing speed over time
        zVec.z = (speed + speedMod)*Time.deltaTime;

        
        
        targVec.y = player.transform.position.y; //Keeps the y value the players

        if (climbing)
        {
            //zVec.z = 0f;
            //player.transform.position -= zVec;
            //targVec.y += 1.8f; // >:(
        }

        coinText.text = "Coins: " + coins;
        distanceTracker.value = dist;
        copDist.text = System.Math.Round((distanceFromRobber - dist), 2) + " m";

        //Debug.Log(stumbleTimer);

        if (stumbleTimer>0)
        {
            //zVec.z = zVec.z * .5f;
            stumbleTimer -= Time.deltaTime;
        }
        else
        {
            //rend.material.color = Color.gray;
            dist+= Time.deltaTime;
        }

        if (slideTimer > 0)
        {
            slideTimer -= Time.deltaTime;
        }
        else sliding = false;

        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
        else jumping = false;

        buildings = GameObject.FindGameObjectsWithTag("Building");

        foreach (GameObject building in buildings)
        {
            building.transform.position -= zVec;
        }



        //Adds horizontal commands to the target position if inputed
        //CHANGE TO SET POSITIONS--LESS ROOM FOR ERROR
        if (Input.GetKeyDown(KeyCode.RightArrow) && !rightLane)
        {
            MoveRight();
            //targVec += xVec;
            //if (leftLane) leftLane = false; //If the player is in the left lane, set left to false and dont change right
            //else rightLane = true; //Otherwise, the player must be entering the right lane
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !leftLane)
        {
            MoveLeft();
            //targVec -= xVec;
            //if (rightLane) rightLane = false; // vice versa
            //else leftLane = true; 
        }

        //Moves the player position to the target position (potentially change)
        player.transform.position = Vector3.MoveTowards(player.transform.position, targVec, strafeSpeed);

        //Allows the player to jump if jump is not on cooldown
        if (Input.GetAxis("Jump")!= 0 && !jumping)
        {
            Jump();
            //play jump animation
            
            //Debug.Log("Yump");

            //jumping = true;
            //jumpTimer = jumpTimerSet; //tie to animation time
        }

        if (Input.GetAxis("Slide")!= 0 && !sliding && !jumping)
        {
            Slide();
            ////play slide animation
            //sliding = true;
            //slideTimer = slideTimerSet;
        }


        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
	}

    public void SaveAction(string s) //Saves current time with action inputted 
    {
        if (save)
        {
            if (!json)
            {
                saves.Add(new SaveData() { timePerformed = Time.timeSinceLevelLoad * 1000f, action = s });
            }
            else
            {
                saveDataJson.savess.Add(new SaveData() { timePerformed = Time.timeSinceLevelLoad * 1000f, action = s });
            }
        }
    }

    public void MoveRight()
    {
        if(!rightLane)
        {
            SaveAction("Move Right");

            targVec += xVec;
            if (leftLane) leftLane = false; //If the player is in the left lane, set left to false and dont change right
            else rightLane = true; //Otherwise, the player must be entering the right lane            
        }
    }

    public void MoveLeft()
    {
        if(!leftLane)
        {
            SaveAction("Move Left");

            targVec -= xVec;
            if (rightLane) rightLane = false; // vice versa
            else leftLane = true;           
        }
    }

    public void Jump()
    {
        if (!jumping)
        {
            Debug.Log("Yump");

            jumping = true;
            jumpTimer = jumpTimerSet; //tie to animation time

            anim.Play("Jump", 0, 0f);

            SaveAction("Jump");
        }
    }

    public void Slide()
    {
        if (!sliding && !jumping)
        {
            //play slide animation
            sliding = true;
            slideTimer = slideTimerSet;

            anim.Play("Slide", 0, 0f);

            SaveAction("Slide"); //Slide or duck, whatever
        }
    }

    public void StopRunning()
    {


    }

    //Player stumbles over an obstacle and slows down for a moment
    public void Stumble()
    {
        stumbleTimer = stumbleTimerSet;
        rend.material.color = Color.red;
        //call stumble animation also
        //slow speed after?
        anim.Play("Stumble", 0, 0f);
        SaveAction("Stumble"); //Player hit something
    }

    void PlayStumbleAnim()
    {
        //Default stumble
    }
    void PlayStumbleAnim(int n)
    {
        //Can give this function a number based on which anim to play
    }

    void OnCollisionExit(Collision collision)
    {

        if(collision.gameObject.tag == "Facade")
        {
            climbing = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Facade") //not in use
        {
            //you fell
            //do something with climbing back up, idk
            //CHANGE TO SET POSITIONS REEEE
            climbing = true;

        }
        else if (collision.gameObject.tag == "Wall")
        {
            PlayStumbleAnim();
            Stumble();
            //the player hit a wall
            //stop until they move back over
        }
        else;
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Slide")
        {
            if (!sliding)
            {
                PlayStumbleAnim(); // Maybe make a different animation for this?
                Stumble();
            }
            else; //good job
            //the player hit an obstacle they had to slide under
            //fall down under it and get up on the other side, slower
        }
        else if (collision.gameObject.tag == "HalfWall")
        {

            if (!jumping)
            {
                PlayStumbleAnim(); 
                Stumble();
            }
            else; //good job
            //stumble, be it a hurdle or half wall
            //slow down a bit as you trip over obstacle
        }
        else if (collision.gameObject.tag == "Hurdle")
        {
            if (!sliding && !jumping)
            {
                PlayStumbleAnim(); //the player stumbles if they aren't sliding or jumping\
                Stumble();
            }
            else; //good job
        }
        else if (collision.gameObject.tag == "Gap")
        {
            if (!jumping)
            {
                PlayStumbleAnim(); //fall into gap and stumble a bit, dont slow
                Stumble();
                Debug.Log("You fell into the gap");
            }
            else;

        }
        else if (collision.gameObject.tag == "GoldBar")
        {
            //get points, add score, good jorb? GO FASTER??
            score += pointsPerBar;
            SaveAction("Bar Collected");
            coins++;
            
            //def pickup noise, like uh... a 'brring' or somethin, yeah
            //oh and kill the bar

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "GoldBarHigh" && jumping)
        {
            //get points, add score, good jorb? GO FASTER??
            score += pointsPerBar;
            SaveAction("High Bar Collected");
            coins++;
            //def pickup noise, like uh... a 'brring' or somethin, yeah
            //oh and kill the bar
            Destroy(collision.gameObject);
        }
        
    }

    void OnApplicationQuit()
    {
        if(save)WriteFile();
    }

    void WriteFile()
    {
        bool c = true;
        int i = 1;
        string fileName = "t", line, ext;

        ext = (json) ? ".txt" : ".csv";

        string dateTime = System.DateTime.UtcNow.ToString("HH-mm-ss--dd-MMMM-yyyy");

        fileName = "/ActionData "+playerName+ " " + dateTime + ext;
        
        StreamWriter sw = new StreamWriter(fileSaveLocation+fileName, false);
        if (!json)
        {
            //Write list of saved data to file
            string header = "Time(Milliseconds),Action";
            sw.WriteLine(header);
            foreach (var SaveData in saves)
            {
                line = SaveData.timePerformed + "," + SaveData.action;
                sw.WriteLine(line);
            }
        }
        else
        {
            sw.Write(JsonUtility.ToJson(saveDataJson));
        }

        sw.Flush();
        sw.Close();

        fileName = "/lastRunA";
        StreamWriter sw2 = new StreamWriter(fileSaveLocation + fileName, false);
        if (!json)
        {
            //Write list of saved data to file
            string header = "Time(Milliseconds),Action";
            sw2.WriteLine(header);
            foreach (var SaveData in saves)
            {
                line = SaveData.timePerformed + "," + SaveData.action;
                sw2.WriteLine(line);
            }
        }
        else
        {
            sw2.Write(JsonUtility.ToJson(saveDataJson));
        }

        sw2.Flush();
        sw2.Close();
    }
}
