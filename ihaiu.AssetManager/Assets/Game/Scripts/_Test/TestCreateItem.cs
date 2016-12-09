using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestCreateItem : MonoBehaviour
{
    public Font font;
    public GameObject[] prefabs;
    public Sprite[] sprites;

	void Start () 
    {
        
        foreach(GameObject prefab in prefabs)
        {
            if (prefab == null)
                continue;
            
            GameObject go = GameObject.Instantiate(prefab);
            go.transform.SetParent(transform, false);
        }

        foreach(Sprite sprite in sprites)
        {
            if (sprite == null)
                continue;
            
            GameObject go = new GameObject(sprite.name);
            go.transform.SetParent(transform, false);
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;

            GameObject txt = new GameObject("Text");
            RectTransform rt = txt.AddComponent<RectTransform>();
            txt.transform.SetParent(go.transform);
            rt.anchoredPosition = new Vector2(0, 0);

            Text text = txt.AddComponent<Text>();
            text.text = sprite.name;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.font = font;

            if (sprite.name == "pic")
            {
            }
        }
	}
	
	void Update () 
    {
	
	}
}
