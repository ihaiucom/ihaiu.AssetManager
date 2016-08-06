using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestFor : MonoBehaviour {

    public int count = 100000;
    public float costtime = 0;
    public float costtime2 = 0;
    public List<string> logs = new List<string>(); 
    // Use this for initialization
    public float begin;
    public float begin2 ;
    public float end ;
	void Start () {
        begin = Time.unscaledTime;
        StartCoroutine(OnRun());
	}

    IEnumerator OnRun()
    {
        yield return new WaitForEndOfFrame();
        begin2 = Time.unscaledTime;
        for(int i = 0; i < count; i ++)
        {
            Vector3 v = Vector3.zero + Vector3.one;
            logs.Add(i + " "+ v);
        }

        end = Time.unscaledTime;
        costtime = end - begin;
        costtime2 = end - begin2;
        Debug.Log(costtime);
        Debug.Log(costtime2);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
