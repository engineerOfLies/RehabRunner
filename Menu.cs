using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using SFB;
//using UnityEditor;


public class Menu : MonoBehaviour
{
    public Button playButton;
    public Button optionButton;
    RectTransform homePanel;
    bool move = false;
    Vector3 t1, t2; //Target 1 and 2, for moving the panels
    bool b1 = true, b2 = true;
    public Button quitButton;

    #region Options
    public RectTransform optionPanel;

    public Toggle saveDataToggle; //Save World and Play Data

    public Toggle endlessModeToggle; //Toggle to enable endless run mode 

    public Toggle saveAsJson; 

    public Button dataSaveLocationButton;

    public Button fileSelectorButton;
   
    public Slider audioVolumeSlider; //Slider to adjust audio volume

    public Button confirmOptionsButton;

    public Button loadProfileButton;

    public Button createProfileButton;

    public InputField createProfileInput;
    #endregion

    //struct
    [System.Serializable]
    public struct Profile
    {
        public string name;
        public string saveDirectory; // pathS
        public string configData; //path
        public bool saveWorldPlayData;
        public bool saveAsJson;
        public float volume;
    }
    Text profileName;
    Profile defaultProfile;

    public Profile currentProfile;

    //IEnumerator moveOptions;

    // Start is called before the first frame update
    void Start()
    {
        
        homePanel = optionButton.transform.parent.GetComponent<RectTransform>();

        playButton.onClick.AddListener(LoadGame);

        optionButton.onClick.AddListener(MoveToOptions);

        quitButton.onClick.AddListener(QuitGame);

        #region Options
        saveDataToggle.onValueChanged.AddListener(SaveWorldPlayData);

        endlessModeToggle.onValueChanged.AddListener(EndlessModeToggle);

        saveAsJson.onValueChanged.AddListener(SaveAsJson);

        dataSaveLocationButton.onClick.AddListener(SaveLocation);

        fileSelectorButton.onClick.AddListener(FileSelector);
        //fileSelectorButton.transform.GetChild(1).gameObject.GetComponent<Text>().text = Application.dataPath;

        audioVolumeSlider.onValueChanged.AddListener(ChangeVolume);

        confirmOptionsButton.onClick.AddListener(ConfirmOptions);

        loadProfileButton.onClick.AddListener(LoadProfile);

        createProfileInput.onEndEdit.AddListener(CreateProfile);
        profileName = createProfileInput.transform.parent.GetComponent<Text>();
        
        #endregion

        t1 = homePanel.position;
        t2 = optionPanel.position;

        #region Default Profile Setup
        defaultProfile.name = "Default Profile";
        defaultProfile.saveDirectory = Application.dataPath;
        defaultProfile.configData = Application.dataPath+"/data.txt";
        defaultProfile.saveWorldPlayData = true;
        defaultProfile.saveAsJson = false;
        defaultProfile.volume = .70f;
        #endregion

        currentProfile = defaultProfile;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(move)
        {
            float x = 0;
            x += Time.deltaTime;
            homePanel.position = Vector3.MoveTowards(homePanel.position, t1, (Mathf.Abs(homePanel.position.x - t1.x)/8));
            optionPanel.position = Vector3.MoveTowards(optionPanel.position, t2, (Mathf.Abs(optionPanel.position.x - t2.x) / 8));

            if (x > 2)
            {
                move = false;
                x = 0f;
            }
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("NewStuff", LoadSceneMode.Single);
    }

    public void MoveToOptions()
    {
        //  -(x - 1) ^ 2 + 1
        if(b1)
        {
            t1 += new Vector3(-1200f, 0f, 0f);
            b1 = false;
        }else
        {
            t1 += new Vector3(1200f, 0f, 0f);
            b1 = true;
        }

        if(b2)
        {
            t2 += new Vector3(-1200f, 0f, 0f);
            b2 = false;
        }
        else
        {
            t2 += new Vector3(1200f, 0f, 0f);
            b2 = true;
        }              
        move = true;
    }
    
    //IEnumerator MoveOptions()
    //{
        
        
    //    yield return null; 
    //}

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SaveWorldPlayData(bool b)
    {
        int i;
        if (b) i = 1;
        else i = 0;
        PlayerPrefs.SetInt("Save?", i);
        currentProfile.saveWorldPlayData = b;
    }

    public void EndlessModeToggle(bool e)
    {
        int j;
        j = (e) ? 1 : 0; //If (e), set j to 1, else set j to 0
        PlayerPrefs.SetInt("EndlessMode?", j);
        

    }

    public void SaveAsJson(bool n)
    {
        int k;
        k = (n) ? 1 : 0;
        PlayerPrefs.SetInt("SaveAsJson?", k);
        currentProfile.saveAsJson = n;
    }

    public void SaveLocation()
    {
        string[] pathS = new string[1];
        pathS = StandaloneFileBrowser.OpenFolderPanel("Choose folder to save data", Application.dataPath, false);
        if(pathS.Length>0)PlayerPrefs.SetString("SaveDataLocation", pathS[0]);

        currentProfile.saveDirectory = pathS[0];
    }

    public void FileSelector()
    {
        string[] path = new string[1];
        path = StandaloneFileBrowser.OpenFilePanel("Choose file", Application.dataPath, "txt", false);
        if (path.Length > 0) PlayerPrefs.SetString("WorldConfig", path[0]);

        currentProfile.configData = path[0];
    }

    public void ChangeVolume(float f)
    {
        Debug.Log(f);
        currentProfile.volume = f;
    }

    public void LoadProfile()
    {
        string[] jsonPath = new string[1];
        //string jStr;
        jsonPath = StandaloneFileBrowser.OpenFilePanel("Select profile to load", Application.dataPath, ".json", false);
        if(jsonPath.Length>0)
        {
            currentProfile = JsonUtility.FromJson<Profile>(jsonPath[0]);
        }

        profileName.text = "Profile: " + currentProfile.name;
        //If the function cant find anything
        //return defaultProfile;
    }

    public void CreateProfile(string s)
    {
        currentProfile.name = s;
        profileName.text = "Profile: "+currentProfile.name;
    }

    public void SaveProfile()
    {
        string json;
        json = JsonUtility.ToJson(currentProfile);
        StreamWriter sw = new StreamWriter(Application.dataPath+"/"+currentProfile.name+".json");
        sw.Write(json);
        sw.Flush();  sw.Close(); 
    }

    public void ConfirmOptions()
    {
        //save json
        SaveProfile();
        MoveToOptions();
    }
}

