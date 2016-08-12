using UnityEngine;
using System.Collections;
using Games;
using System.IO;
using UnityEngine.UI;

namespace Ihaiu.Debugs
{
    public class GMMethod : MonoBehaviour
    {
    	
        void OnEnable()
        {
            InitTextOpenLog();
            InitTextWebUrlIsDevelop();
        }

        public void OnGMSendButtonClick(string str)
        {
        }




        public void SaveLog()
        {
            Queue allQueue = DebugLogManager.Instance.allQueue;
    		
            string str = @"<html>
    <head>
    <meta charset='utf-8' />
    </head>

    <body>
    ";
            while (allQueue.Count > 0)
            {
    			
                DebugLogVO vo = allQueue.Dequeue() as DebugLogVO;
    			
                string stackTrace = vo.stackTrace;
                string logString = vo.logString;
                //			logString = logString.Replace("<color=", "<font color=\"").Replace(">", "\">").Replace("</color\">", "</font>");
    			
                switch (vo.logType)
                {
                    case LogType.Log:
                        str += string.Format("<p><h3 style=\"color: {1}; \"><pre>[Log] {0}</pre></h3><br><pre>{2}</pre></p>", logString, "#008844", stackTrace);
                        break;
                    case LogType.Warning:
                        str += string.Format("<p><h2 style=\"color: {1}; margin-bottom: 12px;\"><pre>[Warning] {0}</pre></h2><br><pre>{2}</pre></p>", logString, "#ffa500", stackTrace);
                        break;
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        str += string.Format("<p><h2 style=\"color: {1}; margin-bottom: 12px;\"><pre>[Error] {0}</pre></h2><br><pre>{2}</pre></p>", logString, "#ff0000", stackTrace);
                        break;
                    default:
                        break;
                }
            }
            str += @"</body></html>";
    		
    		
            string filesPath = "/log.html";
            #if UNITY_EDITOR
            filesPath = Application.dataPath + "/../log.html";
            #elif UNITY_STANDALONE || UNITY_STANDALONE_OSX
            filesPath = Application.dataPath + "/log.html";
            #else
            filesPath = Application.persistentDataPath + "/log.html";
            #endif
            PathUtil.CheckPath(filesPath, true);
            if (File.Exists(filesPath))
                File.Delete(filesPath);
            FileStream fs = new FileStream(filesPath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(str);
            sw.Close();
            fs.Close();
    		
//            string url = "file:///" + filesPath;
            Debug.Log(filesPath);
//    		Application.OpenURL(url);
        }

        public void ClearCache()
        {
            Caching.CleanCache();
        }

        void InitTextOpenLog()
        {
            int isFlagShowPanel = PlayerPrefsUtil.GetIntSimple(PlayerPrefsKey.Setting_OpenLog);
            Debug.logger.logEnabled = isFlagShowPanel != 0;
            Transform tran = transform.FindChild("Button--OpenLog/Text");
            if (tran != null)
            {
                Text text = tran.GetComponent<Text>();
                text.text = "LogEnabled:" + (Debug.logger.logEnabled ? "开启" : "关闭");
            }
        }

        public void OpenLog()
        {
            PlayerPrefsUtil.UseUserId = false;
            int isFlagShowPanel = PlayerPrefsUtil.GetIntSimple(PlayerPrefsKey.Setting_OpenLog);
            PlayerPrefsUtil.UseUserId = true;
            Text text = transform.FindChild("Button--OpenLog/Text").GetComponent<Text>();
            if (isFlagShowPanel == 0)
            {
                text.text = "LogEnabled:开启";
                PlayerPrefsUtil.UseUserId = false;
                PlayerPrefsUtil.SetIntSimple(PlayerPrefsKey.Setting_OpenLog, 1);
                PlayerPrefsUtil.UseUserId = true;
                Debug.logger.logEnabled = true;
            }
            else
            {
                text.text = "LogEnabled:关闭";
                PlayerPrefsUtil.UseUserId = false;
                PlayerPrefsUtil.SetIntSimple(PlayerPrefsKey.Setting_OpenLog, 0);
                PlayerPrefsUtil.UseUserId = true;
                Debug.logger.logEnabled = false;
            }
        }


        void InitTextWebUrlIsDevelop()
        {
            Transform tran = transform.FindChild("Button--WebUrlIsDevelop/Text");
            if (tran != null)
            {
                Text text = tran.GetComponent<Text>();
                text.text = "WebUrl IsDevelop:" + (GameConst.WebUrlIsDevelop ? "开启" : "关闭");
            }
        }

        public void WebUrlIsDevelop()
        {

            GameConst.WebUrlIsDevelop = !GameConst.WebUrlIsDevelop;
            Text text = transform.FindChild("Button--WebUrlIsDevelop/Text").GetComponent<Text>();

            text.text = "WebUrl IsDevelop:" + (GameConst.WebUrlIsDevelop ? "开启" : "关闭");
        }


      

    }
}