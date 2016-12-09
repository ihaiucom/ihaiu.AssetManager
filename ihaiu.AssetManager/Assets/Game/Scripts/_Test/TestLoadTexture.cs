using UnityEngine;
using System.Collections;
using Games;

public class TestLoadTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Load());
	}
	
    public IEnumerator Load()
    {
        while (true)
        {
            for(int i = 0; i < 100; i ++)
            {
                
            Game.assetManager.Load("images/image_test", OnLoad);
            }
            yield return new WaitForSeconds(2);
        }
    }

    public Sprite presprite;
    public Sprite sprite;
    public int prehash;
    public int hash;
    public int preid;
    public int id;
    public bool isOnce;

    void OnLoad(string filename, object obj)
    {
        sprite = (Sprite)obj;

        if (presprite != null)
        {
            isOnce = presprite == sprite;
            prehash = presprite.GetHashCode();
            hash = sprite.GetHashCode();


            preid = presprite.GetInstanceID();
            id = sprite.GetInstanceID();
        }
        presprite = sprite;
    }
}
