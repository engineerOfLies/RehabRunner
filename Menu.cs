using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;


public class Menu : MonoBehaviour
{
    
    public Toggle saveDataToggle; //Save World and Play Data

    public Toggle endlessModeToggle; //Toggle to enable endless run mode 

    public Button dataSaveLocationButton;

    public Button fileSelectorButton;

    public Slider audioVolumeSlider; //Slider to adjust audio volume

    // Start is called before the first frame update
    void Start()
    {
        saveDataToggle.onValueChanged.AddListener(SaveWorldPlayData);

        endlessModeToggle.onValueChanged.AddListener(EndlessModeToggle);

        dataSaveLocationButton.onClick.AddListener(SaveLocation);

        fileSelectorButton.onClick.AddListener(FileSelector);
        //fileSelectorButton.transform.GetChild(1).gameObject.GetComponent<Text>().text = Application.dataPath;

        audioVolumeSlider.onValueChanged.AddListener(ChangeVolume);
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void SaveWorldPlayData(bool b)
    {
        int i;
        if (b) i = 1;
        else i = 0;
        PlayerPrefs.SetInt("Save?", i);
    }

    public void EndlessModeToggle(bool e)
    {
        int j;
        j = (e) ? 1 : 0; //If (e), set j to 1, else set j to 0
        PlayerPrefs.SetInt("EndlessMode?", j);

    }

    public void SaveLocation()
    {
        string pathS;
        pathS = EditorUtility.OpenFolderPanel("Choose folder to save data", Application.dataPath, "");
        PlayerPrefs.SetString("SaveDataLocation", pathS);
    }

    public void FileSelector()
    {
        string path;
        path = EditorUtility.OpenFilePanel("Choose file", Application.dataPath, "txt");
        PlayerPrefs.SetString("WorldConfig", path);
    }

    public void ChangeVolume(float f)
    {
        Debug.Log(f);
    }
}

