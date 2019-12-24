/* 
Our data container. Can be easily modified to fit game needs 
*/
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // VERY IMPORTANT! data has to be wrapped with a Serializable class
public class Data
{
    // POC
    public bool Bool;
    public int Int;
    public float Float;
    public string String;
    public Vector3 pos;

    // Game data
    public int matches;
    public float seconds;
    public List<int> cardValues;
    public List<string> states;
    public List<Vector3> positions;
}
