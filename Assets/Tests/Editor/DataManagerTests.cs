using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DataManagerIntegrationTests
{
    [Test]
    public void SaveToAndLoadFromSessionStorage_SaveModeSetInEditor()
    {
        GameObject dataManagerGO = new GameObject();
        DataManager idm = dataManagerGO.AddComponent<DataManager>();
        idm.sessionStorage = new Dictionary<string, object>();
        idm.saveMode = DataManager.SaveMode.Session;

        Assert.IsFalse(idm.sessionStorage.ContainsKey("SaveToAndLoadFromSessionStorage_SaveModeSetInEditor"));

        TestData testData = new TestData();
        testData.Int = 9834;
        testData.Float = 893724.329874f;
        testData.String = "I am a test string";
        testData.Bool = true;

        idm.SaveData<TestData>("SaveToAndLoadFromSessionStorage_SaveModeSetInEditor", testData);
        TestData loadedData = idm.LoadData<TestData>("SaveToAndLoadFromSessionStorage_SaveModeSetInEditor");

        Assert.IsTrue(idm.sessionStorage.ContainsKey("SaveToAndLoadFromSessionStorage_SaveModeSetInEditor"));
        Assert.AreEqual(loadedData.String, "I am a test string");
        Assert.AreEqual(loadedData.Int, 9834);
        Assert.AreEqual(loadedData.Float, 893724.329874f);
        Assert.AreEqual(loadedData.Bool, true);
    }

    [Test]
    public void SaveToAndLoadFromPlayerPrefs_SaveModeSetInEditor()
    {
        PlayerPrefs.DeleteKey("SaveToAndLoadFromPlayerPrefs_SaveModeSetInEditor");
        Assert.IsFalse(PlayerPrefs.HasKey("SaveToAndLoadFromPlayerPrefs_SaveModeSetInEditor"));

        GameObject dataManagerGO = new GameObject();
        DataManager idm = dataManagerGO.AddComponent<DataManager>();
        idm.saveMode = DataManager.SaveMode.Prefs;

        TestData testData = new TestData();
        testData.Int = 63582;
        testData.Float = 0.2378612f;
        testData.String = "I come from a land down under";
        testData.Bool = false;

        idm.SaveData<TestData>("SaveToAndLoadFromPlayerPrefs_SaveModeSetInEditor", testData);
        TestData loadedData = idm.LoadData<TestData>("SaveToAndLoadFromPlayerPrefs_SaveModeSetInEditor");

        Assert.IsTrue(PlayerPrefs.HasKey("SaveToAndLoadFromPlayerPrefs_SaveModeSetInEditor"));
        Assert.AreEqual(loadedData.String, "I come from a land down under");
        Assert.AreEqual(loadedData.Int, 63582);
        Assert.AreEqual(loadedData.Float, 0.2378612f);
        Assert.AreEqual(loadedData.Bool, false);
    }
    [Test]
    public void SaveToAndLoadFromSessionStorage_SaveModeSetInCode()
    {
        GameObject dataManagerGO = new GameObject();
        DataManager idm = dataManagerGO.AddComponent<DataManager>();
        idm.sessionStorage = new Dictionary<string, object>();

        Assert.IsFalse(idm.sessionStorage.ContainsKey("SaveToAndLoadFromSessionStorage_SaveModeSetInCode"));

        TestData testData = new TestData();
        testData.Int = 9834;
        testData.Float = 893724.329874f;
        testData.String = "I am a test string";
        testData.Bool = true;

        idm.SaveData<TestData>("SaveToAndLoadFromSessionStorage_SaveModeSetInCode", testData, DataManager.SaveMode.Session);
        TestData loadedData = idm.LoadData<TestData>("SaveToAndLoadFromSessionStorage_SaveModeSetInCode", DataManager.SaveMode.Session);

        Assert.IsTrue(idm.sessionStorage.ContainsKey("SaveToAndLoadFromSessionStorage_SaveModeSetInCode"));
        Assert.AreEqual(loadedData.String, "I am a test string");
        Assert.AreEqual(loadedData.Int, 9834);
        Assert.AreEqual(loadedData.Float, 893724.329874f);
        Assert.AreEqual(loadedData.Bool, true);
    }

    [Test]
    public void SaveToAndLoadFromPlayerPrefs_SaveModeSetInCode()
    {
        PlayerPrefs.DeleteKey("SaveToAndLoadFromPlayerPrefs_SaveModeSetInCode");
        Assert.IsFalse(PlayerPrefs.HasKey("SaveToAndLoadFromPlayerPrefs_SaveModeSetInCode"));

        GameObject dataManagerGO = new GameObject();
        DataManager idm = dataManagerGO.AddComponent<DataManager>();


        TestData testData = new TestData();
        testData.Int = 63582;
        testData.Float = 0.2378612f;
        testData.String = "I come from a land down under";
        testData.Bool = false;

        idm.SaveData<TestData>("SaveToAndLoadFromPlayerPrefs_SaveModeSetInCode", testData, DataManager.SaveMode.Prefs);
        TestData loadedData = idm.LoadData<TestData>("SaveToAndLoadFromPlayerPrefs_SaveModeSetInCode", DataManager.SaveMode.Prefs);

        Assert.IsTrue(PlayerPrefs.HasKey("SaveToAndLoadFromPlayerPrefs_SaveModeSetInCode"));
        Assert.AreEqual(loadedData.String, "I come from a land down under");
        Assert.AreEqual(loadedData.Int, 63582);
        Assert.AreEqual(loadedData.Float, 0.2378612f);
        Assert.AreEqual(loadedData.Bool, false);
    }
}


[System.Serializable]
public class TestData
{
    public int Int;
    public float Float;
    public string String;
    public bool Bool;
}