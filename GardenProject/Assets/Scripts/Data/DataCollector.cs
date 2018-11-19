//--------------------------------------------------------------------------------
/*
 * This Script is used for collecing Data generated/User Input
 * 
 * Used in Start Scene, attached to Empty GameObject "DataCollector"
 * Require 1 InputField variable : User ID
 * which can be found under Canvas GameObject in Hierarchy.
 */
//--------------------------------------------------------------------------------

using System.IO; // For StreamWriter, File, Directory
using UnityEngine;  // Default Unity Script (MonoBehaviour, Application, Header, Tooltip, HideInInspector, DontDestroyOnLoad, Time)
using UnityEngine.UI; // For InputField
using UnityEngine.SceneManagement; // For SceneManager

public class DataCollector : MonoBehaviour
{
    // For other scripts to access DataCollector
    public static DataCollector Instance;

    [Tooltip("Time interval to collect Headset Position & Rotation. Default: 1.0f")]
    public float dataRecordInterval = 1f;
    
    [Header("ID Input Field. Under Canvas")]
    public InputField UserIdInputField;

    // For converting User Input into respective data
    [HideInInspector]
    private string dataID = "";

    // For holding SteamVR Camera Component
    [HideInInspector]
    public GameObject user;

    // For Recording Head Movement Data
    [HideInInspector]
    public bool startRecording = false;
    private float time = 0f;
    [HideInInspector]
    public string currentFolderPath;

    // Enum for different Data type to record
    public enum DataToRecord
    {
        HeadMovement
    }

    // Runs before Start()
    private void Awake()
    {
        // Allow DataCollector Instance to be alive after change scene
        DontDestroyOnLoad(this);
    }

    // Runs at the start of first frame
    private void Start ()
    {
        // Set this instance of DataCollector to allow other scripts to access its variables and data
        Instance = this;

        // Allow input of User ID
        UserIdInputField.Select();
    }

    // Runs at every frame
    private void Update ()
    {
        // Start Recording when bool is set to true
        if (startRecording)
        {
            time += Time.deltaTime;

            // Record Head Movement Data every data Record Interval(Default: 1.0f)
            if (time > dataRecordInterval)
            {
                // Reset timer
                time = 0;

                // Write generated data into csv file
                PushData(GenerateData());
            }
        }
	}

    // Returns generated Head Movement Data string
    private string GenerateData()
    {
        string data = "";

        // Get Time Information into data string
        data += System.DateTime.Now.ToString("HH");
        data += ":";
        data += System.DateTime.Now.ToString("mm");
        data += ":";
        data += System.DateTime.Now.ToString("ss");
        data += ":";
        data += System.DateTime.Now.ToString("FFF");

        // Seperator
        data += ",";

        // Get Headset Position in vector 3 format
        string posstr = user.GetComponent<SteamVR_Camera>().head.transform.position.ToString("F3");

        // Change , to . to prevent Position data to be seperated
        data += ChangeLetters(posstr, ',', '.');

        // Seperator
        data += ",";

        // Get Headset Rotation in vector 3 format
        string rotstr = user.GetComponent<SteamVR_Camera>().head.transform.rotation.ToString("F3");

        // Change , to . to prevent Position data to be seperated
        data += ChangeLetters(rotstr, ',', '.');

        return data;
    }

    // Edit the current file by adding the new text
    private void PushData(string text)
    {
        // Open csv file at the path return from GetPath()
        StreamWriter sw = File.AppendText(GetCSVPath(DataToRecord.HeadMovement, currentFolderPath));

        // Write onto the file
        sw.WriteLine(text);

        // Close the file
        sw.Close();
    }

    // Returns the fodler path being used to store the csv files
    private string GetFolderPath()
    {
        string Folder = "/Data/";

        // If the folder path already exists, create a new folder with a duplicate number
        string filePath = Application.dataPath + Folder + dataID;
        int duplicateCounts = 0;
        while (true)
        {
            if (Directory.Exists(filePath))
            {
                ++duplicateCounts;
                filePath = Application.dataPath + Folder + dataID + "(" + duplicateCounts.ToString() + ")";
            }
            else
                break;
        }
        return filePath;
    }

    // Duplicate or Create new folder
    private void CreateFolder()
    {
        currentFolderPath = GetFolderPath();
        // Create new folder and csv files
        Directory.CreateDirectory(currentFolderPath);
        CreateCSV(DataToRecord.HeadMovement, currentFolderPath);
    }

    // Returns the file path being used to store the data
    public string GetCSVPath(DataToRecord dataToRecord, string folderPath)
    {
        string File = "";

        switch (dataToRecord)
        {
            case DataToRecord.HeadMovement:
                File += "/HeadMovement";
                break;
        }

        return folderPath + File + ".csv";
    }

    // Create new CSV
    private void CreateCSV(DataToRecord dataToRecord, string folderPath)
    {
        // Create new CSV and Write in Data Headers on first line
        StreamWriter output = File.CreateText(GetCSVPath(dataToRecord, folderPath));
        switch (dataToRecord)
        {
            case DataToRecord.HeadMovement:
                output.WriteLine("Time, Position, Rotation");
                break;
        }
        output.Close();
    }

    // Change "letter" in "str" to "toBeLetter"
    private string ChangeLetters(string str, char letter, char toBeLetter)
    {
        char[] ret = str.ToCharArray();
        for(int i = 0; i < ret.Length; ++i)
        {
            if(ret[i] == letter)
            {
                ret[i] = toBeLetter;
            }
        }
        return new string(ret);
    }

    //--------------------------------------------------------------------------------
    //                                  PUBLIC FUNCTION
    //--------------------------------------------------------------------------------

    // Link to User ID InputField OnEndEdit()
    // This Registers User ID and use it to create csv file.
    // Then Set bool to true so can start Recording Head Movement Data.
    // And Change Scene to Main Scene (Watch video)
    public void Proceed()
    {
        // If no text, dont let them proceed
        if (UserIdInputField.text == null)
            return;

        // Register User ID
        dataID = UserIdInputField.text;

        // Create Folder and CSV for user based on input
        CreateFolder();

        // Start recording Head Movement
        startRecording = true;

        // Change to MainScene
        SceneManager.LoadScene("MainScene");
    }
}