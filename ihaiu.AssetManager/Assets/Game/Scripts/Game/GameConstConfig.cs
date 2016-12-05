using UnityEngine;
using System.Collections;
using System.IO;
using com.ihaiu;


namespace Games
{
	public class GameConstConfig 
	{
		public bool DevelopMode     = true;					    //开发模式
        public bool TestVersionMode = false;                    //测试版本更新模式
	
		public string AppName   = "ihaiu";           			//应用程序名称
		public string AppPrefix = "ihaiu_";                     //应用程序前缀

        public string 	WebUrl_Release  = "http://www.ihaiu.com/";  	    //更新地址--发布版本
        public string   WebUrl_Develop  = "http://localhost/";              //更新地址--调试版本
        public string 	Version         = "0.0.0";						        //游戏版本号
        public string   CenterName = "Official";                //运营商

        public bool IsCacheResourceAsset                = false;    //是否缓存Resouces加载的对象
        public bool ForcedResourceAsynLoadWaitFrame     = true;     //强制异步加载,等待一帧(Resource.AsyLoad)

		public void Set()
		{
			GameConst.DevelopMode = DevelopMode;

			
            			
			GameConst.AppName = AppName;
			GameConst.AppPrefix = AppPrefix;
			
            GameConst.WebUrl_Release = WebUrl_Release;
            GameConst.WebUrl_Develop = WebUrl_Develop;


            GameConst.Version       = Version;
            GameConst.CenterName    = CenterName;

            AssetManagerSetting.IsCacheResourceAsset                = IsCacheResourceAsset;
            AssetManagerSetting.ForcedResourceAsynLoadWaitFrame     = ForcedResourceAsynLoadWaitFrame;

            #if UNITY_EDITOR
            AssetManagerSetting.TestVersionMode               = TestVersionMode;
            AssetManagerSetting.EditorSimulateConfig          = GameConst.DevelopMode;
            AssetManagerSetting.EditorSimulateAssetBundle     = GameConst.DevelopMode;
            #endif

		}


        #if UNITY_EDITOR
        public static GameConstConfig last;
        public static GameConstConfig Load()
        {
            string filesPath = Application.streamingAssetsPath + "/" + AssetManagerSetting.GameConstName;
            return last = Load(filesPath);
        }


        public void Save()
        {
            last = this;
            string filesPath = Application.streamingAssetsPath + "/" + AssetManagerSetting.GameConstName;
            Save(filesPath);
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("[GameConstJsonGenerator]" + filesPath);
        }
        #endif

        public static GameConstConfig Load(string path)
        {
            var f = new FileInfo(path);
            if (f.Exists)
            {
                var sr = f.OpenText();
                var str = sr.ReadToEnd();
                sr.Close();

                GameConstConfig config = JsonUtility.FromJson<GameConstConfig>(str);
                return config;
            }
            else
            {
                return new GameConstConfig();
            }
        }


        public void Save(string filesPath)
        {
            string str = JsonUtility.ToJson(this, true);

            PathUtil.CheckPath(filesPath, true);
            if (File.Exists(filesPath)) File.Delete(filesPath);

            FileStream fs = new FileStream(filesPath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(str);
            sw.Close(); fs.Close();
        }


	}
}