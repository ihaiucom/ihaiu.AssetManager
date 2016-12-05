using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Games;


namespace com.ihaiu
{
    public class VersionManager : MonoBehaviour
    {
        public Action<string>       errorCallback;
        public Action<string>       stateCallback;

        public Action<string>       updateFailedCallback;
        public Action<float>        updateProgressCallback;

        public Action               updateEnterCallback;
        public Action               updateEndCallback;


		public Action               serverCloseCallback;
        public Action<string>       needDownAppCallback;
        public Action               needHostUpdateCallback;

        public Action               finalCallback;

        public bool yieldbreak = false;
        private Version appVer = new Version();
        private Version curVer = new Version();
        private Version serverVer = new Version();

        public VersionInfo serverVersionInfo;

        private GameConstConfig appGameConstConfig;
        private GameConstConfig curGameConstConfig;

        private AssetFileList   assetList;

        #region Event
        void OnError(string txt)
        {
            if (errorCallback != null)
                errorCallback(txt);
        }

        void OnState(string txt)
        {
            if (stateCallback != null)
                stateCallback(txt);
        }

        void OnUpdateEnter()
        {
            if (updateEnterCallback != null)
                updateEnterCallback();
        }

        void OnUpdateEnd()
        {
            if (updateEndCallback != null)
                updateEndCallback();
        }

        void OnUpdateFailed(string url)
        {
            if (updateFailedCallback != null)
                updateFailedCallback(url);
        }

        void OnUpdateProgress(float progress)
        {
            if (updateProgressCallback != null)
                updateProgressCallback(progress);
        }

		void OnServerClose()
		{
			if (serverCloseCallback != null)
				serverCloseCallback();
		}

        void OnNeedDownApp(string url)
        {
            if (needDownAppCallback != null)
                needDownAppCallback(url);
        }


        void OnNeedHostUpdate()
        {
            if (needHostUpdateCallback != null)
                needHostUpdateCallback();
        }

        void OnFinal()
        {
            if (finalCallback != null)
                finalCallback();
        }

        private bool IsContinueHostUpdate = false;
        public void SetContinueHostUpdate()
        {
            IsContinueHostUpdate = true;
        }
        #endregion


        public IEnumerator CheckVersion()
        {
            yield return StartCoroutine(ReadGameConst_Streaming());

            #if UNITY_EDITOR
            appGameConstConfig.Set();

            if (appGameConstConfig.DevelopMode)
            {
                OnFinal();
                yield break;
            }

            if(!AssetManagerSetting.TestVersionMode)
            {
                appGameConstConfig.Set();
                OnFinal();
                yield break;
            }
            #endif


            yield return StartCoroutine(ReadGameConst_Persistent());

            appVer.Parse(appGameConstConfig.Version);

            bool needInitData = false;
            if (curGameConstConfig == null)
            {
                appGameConstConfig.Set();
                needInitData = true;
            }
            else
            {
                curGameConstConfig.Set();
                curVer.Parse(curGameConstConfig.Version);
                needInitData = VersionCheck.CheckNeedCopy(curVer, appVer);
            }


            AssetManagerSetting.persistentAssetFileList = AssetFileList.Read(AssetManagerSetting.PersistentAssetFileListPath);

            if (needInitData)
            {
                yield return StartCoroutine(InitData());
                curVer.Copy(appVer);
            }



            yield return (ReadServerVersionInfo());
            Debug.Log("serverVersionInfo=" + serverVersionInfo);

            if (serverVersionInfo != null)
            {
				if (serverVersionInfo.isClose > 0)
				{
					OnServerClose();
					yieldbreak = true;
				}
				else
				{
	                serverVer.Parse(serverVersionInfo.version);

	                VersionCheckState state = VersionCheck.CheckState(curVer, serverVer);

	                switch(state)
	                {
	                    case VersionCheckState.DownApp:
	                        OnNeedDownApp(serverVersionInfo.downLoadUrl);
	                        yieldbreak = true;
	                        break;

	                    case VersionCheckState.HotUpdate:
	                        IsContinueHostUpdate = false;
	                        OnNeedHostUpdate();

	                        while (!IsContinueHostUpdate)
	                        {
	                            yield return new WaitForSeconds(0.25f);
	                        }

	                        yield return StartCoroutine(UpdateResource(serverVersionInfo.updateLoadUrl));
	                        yield return StartCoroutine(ReadGameConst_Persistent());
	                        curGameConstConfig.Set();
	                        OnFinal();
	                        break;
	                    default:
	                        OnFinal();
	                        break;
	                }
				}
            }
            else
            {
                Debug.Log("zj OnFinal");
                OnFinal();
            }
        }

        IEnumerator InitData()
        {
            Caching.CleanCache();
            yield return InitAssetList();

            List<string> list = new List<string>();
            list.Add(AssetManagerSetting.GameConstName);



            string infile = "";
            string outfile = "";
            int count = list.Count;
            for (int i = 0; i < count; i ++)
            {
                string file = list[i];
               

                file = string.Format(file, Platform.PlatformDirectory);
                infile = AssetManagerSetting.RootPathStreaming + file; 
                outfile = AssetManagerSetting.RootPathPersistent + file;

                AssetManagerSetting.persistentAssetFileList.Add(file, "");

                PathUtil.CheckPath(outfile);

                if (Application.platform == RuntimePlatform.Android)
                {
                    WWW www = new WWW(infile);
                    yield return www;

                    if (www.isDone)
                    {
                        File.WriteAllBytes(outfile, www.bytes);
                    }

                    www.Dispose();
                    www = null;
                    yield return 0;
                }
                else
                {
                    File.Copy(infile, outfile, true);
                }

                yield return new WaitForEndOfFrame();
            }



            AssetManagerSetting.persistentAssetFileList.Add(AssetManagerSetting.AssetListName, "");

            AssetManagerSetting.persistentAssetFileList.Save(AssetManagerSetting.PersistentAssetFileListPath);

        }

        IEnumerator InitAssetList()
        {
            string url = AssetManagerSetting.FilesCsvForStreaming;
            WWW www = new WWW(url);
            yield return www;

            assetList = AssetFileList.Read(AssetManagerSetting.AssetFileListPath);
            if(string.IsNullOrEmpty(www.error))
            {
                string path, md5;

                using(StringReader stringReader = new StringReader(www.text))
                {
                    while(stringReader.Peek() >= 0)
                    {
                        string line = stringReader.ReadLine();
                        if(!string.IsNullOrEmpty(line))
                        {
                            string[] seg = line.Split(';');
                            path            = seg[0];
                            md5             = seg[1];


                            assetList.Add(path, md5);

                            AssetManagerSetting.persistentAssetFileList.Remove(AssetManagerSetting.GetPlatformPath(path));
                        }
                    }
                }
            }

            assetList.Save(AssetManagerSetting.AssetFileListPath);
        }


        /** 获取服务器版本号 */
        IEnumerator ReadServerVersionInfo()
        {

            int isUpdateTest = PlayerPrefsUtil.GetIntSimple(PlayerPrefsKey.Setting_Update);//测试更新
            string centerName = GameConst.CenterName;
            if (isUpdateTest == 1)
            {
                centerName = "Test";
            }


            string url = AssetManagerSetting.GetServerVersionInfoURL(GameConst.WebUrl, centerName) ;
            WWW www = new WWW(url);
            yield return www;
            if(!string.IsNullOrEmpty(www.error))
            {
                OnError("获取服务器版本号出错");
                Debug.LogErrorFormat("获取服务器版本号出错 url={0}, www.error={1}", url, www.error);
                www.Dispose();
                www = null;

                yield break;
            }

            Debug.Log(www.text);

            serverVersionInfo = JsonUtility.FromJson<VersionInfo>(www.text);
            serverVersionInfo.Set();
        }


        /** 读取streaming下只读配置  */
        IEnumerator ReadGameConst_Streaming()
        {   
            OnState("读取streaming game_const.json");
            string url = AssetManagerSetting.GameConstUrl_Streaming;
            WWW www = new WWW(url);
            yield return www;
            if(string.IsNullOrEmpty(www.error))
            {
                Debug.Log(url);
                Debug.Log(www.text);

                appGameConstConfig = JsonUtility.FromJson<GameConstConfig>(www.text);
            }
            else
            {       
                OnError("读取Streaming下game_const.json失败");
                Debug.LogErrorFormat("读取game_const.json失败 ReadGameConst_Streaming url={0} error={1}", url, www.error);
            }

            www.Dispose();
            www = null;
        }


        IEnumerator ReadGameConst_Persistent()
        {   
            OnState("读取persistent game_const.json");
            string url = AssetManagerSetting.GameConstUrl_Persistent;
            WWW www = new WWW(url);
            yield return www;
            if(string.IsNullOrEmpty(www.error))
            {
                Debug.Log(url);
                Debug.Log(www.text);

                curGameConstConfig = JsonUtility.FromJson<GameConstConfig>(www.text);
            }
            else
            {       
                Debug.LogFormat("读取game_const.json失败 ReadGameConst_Persistent url={0} error={1}", url, www.error);
            }

            www.Dispose();
            www = null;
        }


        IEnumerator UpdateResource(string rootUrl)
        {
            // rootUrl = "http://www.ihaiu.com/StreamingAssets/"
            OnUpdateEnter();

            //获取服务器端的file.csv

            OnState("获取服务器端的file.csv");
            string updateAssetListUrl = AssetManagerSetting.GetServerFilesCsvURL(rootUrl);
            Debug.Log("UpdateAssetList URL: " + updateAssetListUrl);
            WWW www = new WWW(updateAssetListUrl);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                OnError("更新资源读取资源列表失败");
                Debug.LogErrorFormat("更新资源读取资源列表失败 updateAssetListUrl={0}, www.error={1}", updateAssetListUrl,  www.error);
                www.Dispose();
                www = null;
                yield break;
            }

            AssetFileList updateAssetList = AssetFileList.Deserialize(www.text);
            www.Dispose();
            www = null;
            //Debug.Log("count: " + files.Length + " text: " + filesText);


            OnState("读取" + AssetManagerSetting.AssetFileListPath);
            if(assetList == null) assetList = AssetFileList.Read(AssetManagerSetting.AssetFileListPath);

            List<AssetFile> diffs = AssetFileList.Diff(assetList, updateAssetList);

            AssetManagerSetting.persistentAssetFileList = AssetFileList.Read(AssetManagerSetting.PersistentAssetFileListPath);

            string path;
            //更新
            int count = diffs.Count;
            for (int i = 0; i < count; i++)
            {
                AssetFile item = diffs[i];
                path = AssetManagerSetting.GetPlatformPath(item.path);

                OnState("更新" + path);
                OnUpdateProgress((float)(i + 1) / (float)count);

                string url = rootUrl + path;
                www = new WWW(url);

                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    OnUpdateFailed(url);

                    www.Dispose();
                    www = null;
                    continue;
                }

                string localPath = AssetManagerSetting.RootPathPersistent + path;
                PathUtil.CheckPath(localPath, true);
                File.WriteAllBytes(localPath, www.bytes);

                www.Dispose();
                www = null;

                assetList.Add(item);

                AssetManagerSetting.persistentAssetFileList.Add(path, item.md5);
            }
            yield return new WaitForEndOfFrame();


            assetList.Save(AssetManagerSetting.AssetFileListPath);
            AssetManagerSetting.persistentAssetFileList.Save(AssetManagerSetting.PersistentAssetFileListPath);


            // 更新完成
            OnUpdateEnd();
        }
    }
}