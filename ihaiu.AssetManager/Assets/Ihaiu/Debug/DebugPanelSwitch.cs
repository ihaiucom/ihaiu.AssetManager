using UnityEngine;
using System.Collections;

namespace Ihaiu.Debugs
{
    public class DebugPanelSwitch : MonoBehaviour {

    	public GameObject panel;
    	void Start () 
    	{
            if(panel == null) panel = GameObject.Find("DebugPanel");
    		if(panel != null) panel.SetActive(true);
    		DontDestroyOnLoad(transform.parent.gameObject);
    	}

    	float touchTime = 0;
    	float touchVisiableTime = 0;
    	void Update () 
    	{
    		if(Application.isMobilePlatform)
    		{
                    if (Input.touchCount >= 4 && Time.time >= touchTime)
                    {
                        touchVisiableTime += Time.deltaTime;
                        if (touchVisiableTime >= 1)
                        {
                            touchTime = Time.time + 1F;
                            touchVisiableTime = 0;
                            panel.SetActive(!panel.activeSelf);
                            if (panel.activeSelf)
                            {
                                panel.GetComponent<RectTransform>().SetAsLastSibling();
                            }
                        }
                    }
                    else
                    {
                        touchVisiableTime = 0;
                    }
    		}
    		else
    		{
                    if (Input.GetKeyDown(KeyCode.F1))
                    {
                        panel.SetActive(!panel.activeSelf);
                        if (panel.activeSelf)
                        {
                            panel.GetComponent<RectTransform>().SetAsLastSibling();
                        }
                    }
     		}
    	}

    	public void Show()
    	{
    		panel.SetActive(true);
    		panel.GetComponent<RectTransform>().SetAsLastSibling();
    	}

    	public void Hide()
    	{
    		panel.SetActive(false);
    	}
    }
}