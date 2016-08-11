using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Ihaiu.Debugs
{
    public class DebugPanel : MonoBehaviour
    {
    	public Queue allQueue;
    	public Queue logQueue;
    	public Queue warningQueue;
    	public Queue errorQueue;
    	public GameObject gmPanel;
    	
    	public void Awake()
    	{
    		gameObject.SetActive(false);
            GMVisiable();
        }
    	
    	
    	
    	
    	public DebugLogType debugLogType;
    	private DebugLogVO vo;
    	void Update()
    	{
    		if(debugLogType == DebugLogType.None)
    		{
    			return;
    		}
    		
    		if(text.text.Split('\n').Length > 100)
    		{
    			return;
    		}
    		
    		if(logQueue == null)
    		{
                allQueue        = DebugLogManager.Instance.allQueue;
                logQueue        = DebugLogManager.Instance.logQueue;
                warningQueue    = DebugLogManager.Instance.warningQueue;
                errorQueue      = DebugLogManager.Instance.errorQueue;
    		}
    		
    		
    		if (debugLogType == DebugLogType.All && allQueue.Count > 0)
    		{
    			vo = allQueue.Dequeue() as DebugLogVO;
    		}
    		else if (debugLogType == DebugLogType.Log && logQueue.Count > 0)
    		{
    			vo = logQueue.Dequeue() as DebugLogVO;
    		}
    		else if (debugLogType == DebugLogType.Warning && warningQueue.Count > 0)
    		{
    			vo = warningQueue.Dequeue() as DebugLogVO;
    		}
    		else if (debugLogType == DebugLogType.Error && errorQueue.Count > 0)
    		{
    			vo = errorQueue.Dequeue() as DebugLogVO;
    		}
    		else
    		{
    			vo = null;
    		}
    		
    		if (vo == null)
    		{
    			return;
    		}
    		
    		string stackTrace = vo.stackTrace;
    		//		if(stackTrace.Length > 500)
    		//		{
    		//			string[] lines = stackTrace.Split('\n');
    		//			stackTrace = "";
    		//			for(int i = 0; i < (lines.Length < 20 ? lines.Length : 20); i ++)
    		//			{
    		//				stackTrace += lines[i] + "\n";
    		//			}
    		//		}
    		
    		text.text += "\n";
    		switch (vo.logType)
    		{
    		case LogType.Log:
    			text.text += string.Format("<color='{1}'>{0}</color>\n{2}", vo.logString , "#008844", stackTrace);
    			break;
    		case LogType.Warning:
    			text.text += string.Format("<color='{1}'>{0}</color>\n{2}", vo.logString, "#ffa500", stackTrace);
    			break;
    		case LogType.Assert:
    		case LogType.Error:
    		case LogType.Exception:
    			text.text += string.Format("<color='{1}'>{0}</color>\n{2}", vo.logString, "#ff0000", stackTrace);
    			break;
    		default:
    			break;
    		}
    		//		text.text +=  "Length=" + text.text.Length.ToString() + " linenum="+ (text.text.Split('\n').Length);
    		//		try{
    		text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight > (text.transform.parent as RectTransform).sizeDelta.y ? text.preferredHeight :  (text.transform.parent as RectTransform).sizeDelta.y);
    		//		}
    		//		catch()
            //		{warningQueue
    		//
    		//		}
    	}
    	
    	
    	public void ShowAll()
    	{
    		debugLogType = DebugLogType.All;
    	}
    	
    	public void ShowLog()
    	{
    		debugLogType = DebugLogType.Log;
    	}
    	
    	public void ShowWarning()
    	{
    		debugLogType = DebugLogType.Warning;
    	}
    	
    	public void ShowError()
    	{
    		debugLogType = DebugLogType.Error;
    	}
    	
    	public Text text;
    	public void ShowInfo()
    	{
    		debugLogType = DebugLogType.None;
    		text.text += "\n";
    		text.text += AppInfo.GetAppInfo();
    		text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight > (text.transform.parent as RectTransform).sizeDelta.y ? text.preferredHeight :  (text.transform.parent as RectTransform).sizeDelta.y);
    	}
    	
    	public void AddInfo(string str)
    	{
    		text.text += "\n";
    		text.text += str;
    		text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight > (text.transform.parent as RectTransform).sizeDelta.y ? text.preferredHeight :  (text.transform.parent as RectTransform).sizeDelta.y);
    		
    	}
    	
    	public void Clear()
    	{
    		text.text = "";
    	}

        public void ClearAll()
        {
            allQueue.Clear();
            logQueue.Clear();
            warningQueue.Clear();
            Clear();
        }
    	
    	public void GMVisiable()
    	{
    		if(gmPanel != null) gmPanel.SetActive(!gmPanel.activeSelf);
    	}
    	
    	public void Hide()
    	{
    		gameObject.SetActive(false);
    	}
    	
    	public void Show()
    	{
    		gameObject.SetActive(true);
    	}
    	
    	public void OnGMSendButtonClick()
    	{
    		Text gmText = gameObject.transform.Find("GMBar/InputField/Text").GetComponent<Text>();
    		
            Debug.Log( "请在这里设置你的命令 DebugPanel.OnGMSendButtonClick() " + gmText.text );
    		
    		gmText.text = "send over!";
    		
    		Hide();
    	}
    	

    	
    }


}
