/*
DataManager API

All this api has to do is store and retrieve data. It can do so in two methods and can be expanded upon to utilize more methods.
The api is agnostic to payload type, as long as it is a **Serializable** class. Within the class you can pass any serializable data. 
VERY IMPORTANT! data has to be wrapped with a Serializable class!
*/
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance; // Make an accessible instance of this script
    public enum SaveMode // save mode
    {
        Session,
        Prefs,
        #region AddAnotherMethod-1
        // File, // 1. [Adding more methods is easy. first add a mode to the SaveMode enum]
        #endregion
    }
    public SaveMode saveMode;
    public Dictionary<string, object> sessionStorage;
    private void Awake()
    {
        if (Instance != null && Instance != this) // Unity ""Singleton"" pattern
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        sessionStorage = new Dictionary<string, object>(); // Instantiate our session storage
    }
    // public method for saving data. SaveMode can be set from the editor.
    // this method takes in data wrapped in a Serializable class
    public void SaveData<T>(string key, T payload) where T : class
    {
        switch (saveMode)
        {       // Switch to determine save method
            case SaveMode.Session:
                SaveDataToSession(key, payload);
                break;
            case SaveMode.Prefs:
                SaveDataToPlayerPrefs(key, payload);
                break;
                #region AddAnotherMethod-2
                // case SaveMode.File: // 2. [Then add the switch case in the Save method]
                //     SaveDataToFile(key, payload);
                //     break;
                #endregion
        }
    }
    public string SaveData<T>(string key, T payload, SaveMode mode) where T : class // Overloaded method to set SaveMode from code if we so please
    {
        if (mode != saveMode)
        {
            saveMode = mode;
        }
        SaveData<T>(key, payload);
        return null;
    }
    // public method for loading data. SaveMode can be set from code or from the editor.
    // this method returns data wrapped in a class (any class)

    // TODO: Validate class type matches saved class type when loading to avoid corrupting the data
    public T LoadData<T>(string key, SaveMode mode = default) where T : class
    {
        if (mode != default)
        {
            saveMode = mode;
        }
        T t;
        switch (saveMode)
        {
            case SaveMode.Session:
                t = LoadDataFromSession(key) as T;
                return t;
            case SaveMode.Prefs:
                t = LoadDataFromPlayerPrefs<T>(key);
                return t;
                #region AddAnotherMethod-3
                // case SaveMode.File: // 3. [Don't forget the Load method]
                //     LoadDataFromFile(key) as T;
                //     break;
                #endregion
        }
        // Handle errors
        return default(T);
    }
    private void SaveDataToSession(string key, object payload)
    {
        // Overwrite switch?
        if (sessionStorage.ContainsKey(key)) // Check if the data exsists in the sessions storage and if so delete it with extreme prejudice
        {
            sessionStorage.Remove(key);
        }
        sessionStorage.Add(key, payload); // Add our data to the session storage
    }
    private object LoadDataFromSession(string key)
    {
        object obj = new object();
        if (sessionStorage.TryGetValue(key, out object o)) // Try to get our data 
        {
            return o; // We got it! yay! return it
        }
        // Handle non existent key
        return null;
    }
    private void SaveDataToPlayerPrefs<T>(string key, T payload) where T : class
    {
        try
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
            string json = JsonUtility.ToJson(payload); // Serialize our data into a json string
            // consider using a better Json parser
            PlayerPrefs.SetString(key, json); // Save the string
            PlayerPrefs.Save(); // REALLY save the string
        }
        catch
        {
            // Handle save errors
            print("Couldn't save data to player prefs");
        }
    }
    private T LoadDataFromPlayerPrefs<T>(string key) where T : class
    {
        try
        {
            string jsonString = PlayerPrefs.GetString(key); // Get our data from playerprefs
            T jsonData = JsonUtility.FromJson<T>(jsonString); // Deserialize it into the requested class type
            return jsonData;
        }
        catch
        {
            // Handle load errors
            print("Couldn't load data to player prefs");
            return null;
        }
    }
    #region AddAnotherMethod-4
    // private void SaveDataToFile(string key, object payload) // 4. [And finally add your methods for saving and loading. That's it]
    // {

    // }
    // private void LoadDataFromFile(string key)
    // {

    // }
    #endregion
}
