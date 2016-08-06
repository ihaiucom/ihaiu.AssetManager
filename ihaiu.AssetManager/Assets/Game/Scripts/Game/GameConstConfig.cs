using UnityEngine;
using System.Collections;
using System.IO;

namespace Games
{
	public class GameConstConfig 
	{
		public bool CleanupDataPath = false;
		public bool DevelopMode     = true;					    //开发模式
	
		public string AppName   = "ihaiu";           			//应用程序名称
		public string AppPrefix = "ihaiu_";                     //应用程序前缀

		public string 	WebUrl = "http://www.ihaiu.com/";  	    //更新地址
		public string 	Version = "0.0.0";						//游戏版本号

		public void Set()
		{

			GameConst.CleanupDataPath = CleanupDataPath;
			GameConst.DevelopMode = DevelopMode;

			
            			
			GameConst.AppName = AppName;
			GameConst.AppPrefix = AppPrefix;
			
			GameConst.WebUrl = WebUrl;


            GameConst.Version = Version;
		}


        #if UNITY_EDITOR
        public static GameConstConfig Load()
        {
            var f = new FileInfo(Application.streamingAssetsPath + "/" + GameConst.GameConstFileName);
            var sr = f.OpenText();
            var str = sr.ReadToEnd();
            sr.Close();

            GameConstConfig config = JsonUtility.FromJson<GameConstConfig>(str);
            return config;
        }


        public void Save()
        {
            string str = JsonUtility.ToJson(this, true);
            string filesPath = Application.streamingAssetsPath + "/" + GameConst.GameConstFileName;

            PathUtil.CheckPath(filesPath, true);
            if (File.Exists(filesPath)) File.Delete(filesPath);

            FileStream fs = new FileStream(filesPath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(str);
            sw.Close(); fs.Close();
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("[GameConstJsonGenerator]" + filesPath);
        }
        #endif


	}
}