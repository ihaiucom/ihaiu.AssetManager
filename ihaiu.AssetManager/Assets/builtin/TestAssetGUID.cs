using UnityEngine;
using System.Collections;
using System.IO;

public class TestAssetGUID : MonoBehaviour {

    public Material[] materials;
    public Texture[] textures;
    public Sprite[] sprites;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	void Update () {
	
	}


    /** 首字母小写 */
    public static string FirstLower(string str)
    {
        return char.ToLower(str[0]) + str.Substring(1); 
    }

    [ContextMenu("EchoSprite")]
    public void EchoSprite()
    {
        StringWriter sw = new StringWriter();
        sw.WriteLine("string SPRITE_GUID_NEW = \"{fileID: 21300000, guid: __GUID__, type: 3}\";");
        sw.WriteLine("");

        string[] oldGuids = new string[]{ 
            "{fileID: 10907, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10901, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10915, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10911, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10913, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10917, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10905, guid: 0000000000000000f000000000000000, type: 0}",
        };

        int i = 0;
        foreach(Sprite sprite in sprites)
        {
            if (sprite != null)
            {
                string fieldName = FirstLower(sprite.name.Replace(" ", "_").Replace("-", "_"));
                string fieldName_oldUid = fieldName + "_oldUid" ;
                sw.WriteLine("Sprite {0};", sprite.name);
                sw.WriteLine("string {0}\t= \"{1}\";", fieldName_oldUid, oldGuids[i]);
            }
            i++;
        }

        Debug.Log(sw.ToString());
    }
}
