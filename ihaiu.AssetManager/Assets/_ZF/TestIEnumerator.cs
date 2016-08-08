using UnityEngine;
using System.Collections;

public class TestIEnumerator : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Init());
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    IEnumerator Init()
    {
        Debug.Log("Init 0");
        yield return LoadA();

        Debug.Log("Init 1");

        yield return LoadB();
        Debug.Log("Init 2");
    }

    IEnumerator Init2()
    {
        Debug.Log("Init2 0");
        yield return StartCoroutine(LoadA());

        Debug.Log("Init2 1");

        yield return StartCoroutine(LoadB());
        Debug.Log("Init2 2");
    }

    IEnumerator LoadA()
    {
        for(int i = 0; i < 3; i ++)
        {
            Debug.Log("LoadA " + i);
            yield break;
        }

        Debug.Log("LoadA End");
    }

    IEnumerator LoadB()
    {

        for(int i = 0; i < 3; i ++)
        {
            Debug.Log("LoadB " + i);
            yield return new WaitForSeconds(1);
        }

        Debug.Log("LoadB End");
    }



}
