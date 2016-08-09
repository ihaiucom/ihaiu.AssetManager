using UnityEngine;
using System.Collections;
using System.IO;

public class TestPathFileName : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log(Path.GetFileName("config/stage.csv"));
        Debug.Log(Path.GetFileName("config/stage"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
