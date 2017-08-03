using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Games;


namespace com.ihaiu
{
    public partial class VersionManager : MonoBehaviour
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


        public bool yieldbreak = false;
        public VersionCheckState state = VersionCheckState.Normal;
        private Version appVer = new Version();
        private Version curVer = new Version();
        private Version serverVer = new Version();

        public VersionInfo serverVersionInfo;

        private GameConstConfig appGameConstConfig;
        private GameConstConfig curGameConstConfig;


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


        private bool IsContinueHostUpdate = false;
        public void SetContinueHostUpdate()
        {
            IsContinueHostUpdate = true;
        }
        #endregion

       

        public IEnumerator CheckFirst()
        {
            yield return StartCoroutine(ReadGameConst_Streaming());

            #if UNITY_EDITOR
            appGameConstConfig.Set();

            if (appGameConstConfig.DevelopMode)
            {
                yield break;
            }

            if(!AssetManagerSetting.TestVersionMode)
            {
                appGameConstConfig.Set();
                yield break;
            }
            #endif

            appVer.Parse(appGameConstConfig.Version);
            VersionLocalInfo.Install.streamVersion.Parse(appGameConstConfig.Version);
            if (VersionLocalInfo.Install.IsResetAppRes)
            {
                appGameConstConfig.Set();
                curVer.Copy(appVer);
                yield return StartCoroutine(InitData());
            }
            else
            {
                yield return StartCoroutine(ReadGameConst_Persistent());

                curGameConstConfig.Set();
                curVer.Parse(curGameConstConfig.Version);
            }

            if (VersionLocalInfo.Install.IsNewApp)
            {
                VersionLocalInfo.Install.localVersion.Parse(appGameConstConfig.Version);
                VersionLocalInfo.Install.Save();
            }



//            yield return StartCoroutine(ReadGameConst_Persistent());
//
//            appVer.Parse(appGameConstConfig.Version);
//
//            bool needInitData = false;
//            if (curGameConstConfig == null)
//            {
//                appGameConstConfig.Set();
//                needInitData = true;
//            }
//            else
//            {
//                curGameConstConfig.Set();
//                curVer.Parse(curGameConstConfig.Version);
//                needInitData = VersionCheck.CheckNeedCopy(curVer, appVer);
//            }
//
//
//
//            if (needInitData)
//            {
//                yield return StartCoroutine(InitData());
//                curVer.Copy(appVer);
//            }
//
        }


        public IEnumerator CheckVersion()
        {
            #if UNITY_EDITOR
            if (appGameConstConfig.DevelopMode)
            {
                yield break;
            }

            if(!AssetManagerSetting.TestVersionMode)
            {
                yield break;
            }
            #endif



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

	                state = VersionCheck.CheckState(curVer, serverVer);

                    if (state != VersionCheckState.DownApp && serverVersionInfo.zipPanelStarShow)
                    {
                        VersionLocalInfo.Install.SetServerInfo(serverVersionInfo);
//                        yield return Coo.downloadZip.CheckExeCoroutine();
                    }

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
	                        break;
	                    default:
	                        break;
	                }
				}

                if (!yieldbreak && !serverVersionInfo.zipPanelStarShow)
                {
                    VersionLocalInfo.Install.SetServerInfo(serverVersionInfo).CheckZipExe();
                }
            }
            else
            {
                Debug.Log("zj OnFinal");
            }
        }

        IEnumerator InitData()
        {
            Caching.CleanCache();
            yield return InitAssetList();

            List<string> list = new List<string>();
            list.Add(AssetManagerSetting.FileName.GameConst);



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




            AssetManagerSetting.persistentAssetFileList.Save();

        }

        IEnumerator InitAssetList()
        {
            string url = AssetManagerSetting.StreamingFileURL.AssetListApp;
            WWW www = new WWW(url);
            yield return www;


            if (string.IsNullOrEmpty(www.error))
            {
                InitAssetList(www.text);
            }
            else
            {
                AssetManagerSetting.versionAssetFileList.Save();
            }
        }


        void InitAssetList(string txt)
        {

            if(!string.IsNullOrEmpty(txt))
            {
                AssetFileList appAssetFileList = AssetFileList.Deserialize(txt);
                AssetFile item;
                AssetFile verItem;
                for(int i = 0; i < appAssetFileList.list.Count; i ++)
                {
                    item = appAssetFileList.list[i];
                    verItem = AssetManagerSetting.versionAssetFileList.Get(item.path);
                    if (verItem == null || verItem.IsEnableCover(appVer))
                    {
                        AssetManagerSetting.versionAssetFileList.Add(item).SetVer(appVer);
                        AssetManagerSetting.persistentAssetFileList.Remove(AssetManagerSetting.GetPlatformPath(item.path));
                    }
                }
            }

            AssetManagerSetting.versionAssetFileList.Save();
        }


        /** 获取服务器版本号 */
        IEnumerator ReadServerVersionInfo()
        {

            int isUpdateTest = PlayerPrefsUtil.GetIntSimple(PlayerPrefsKey.Setting_Update);//测试更新
            string centerName = GameConst.CenterName;

            string channelName = centerName;
//            if (centerName == "Quick")
//            {
//                if (!string.IsNullOrEmpty(GameConst.QuickChannelName) && GameConst.QuickChannelName != "quick")
//                {
//                    channelName += "_" + GameConst.QuickChannelName;
//                }
//            }

            string url = AssetManagerSetting.ServerURL.GetServerVersionInfoURL(GameConst.WebUrl, channelName, isUpdateTest == 1) ;
            Debug.Log("VersionURL=" + url);
            WWW www = new WWW(url);
            yield return www;
            if(!string.IsNullOrEmpty(www.error))
            {
                OnError("获取服务器版本号出错");
                Debug.LogErrorFormat("获取服务器版本号出错 url={0}, www.error={1}", url, www.error);
                www.Dispose();
                www = null;

                if (centerName == "Quick")
                {
                    url = AssetManagerSetting.ServerURL.GetServerVersionInfoURL(GameConst.WebUrl, centerName, isUpdateTest == 1);

                    www = new WWW(url);
                    yield return www;
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        OnError("获取服务器版本号出错");
                        Debug.LogErrorFormat("获取服务器版本号出错 url={0}, www.error={1}", url, www.error);
                        www.Dispose();
                        www = null;

                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }

            Debug.Log(www.text);

            serverVersionInfo = JsonUtility.FromJson<VersionInfo>(www.text);
            serverVersionInfo.Set();
        }


        /** 读取streaming下只读配置  */
        IEnumerator ReadGameConst_Streaming()
        {   
            OnState("读取streaming game_const.json");
            string url = AssetManagerSetting.StreamingFileURL.GameConst;
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
            string url = AssetManagerSetting.PersistentFileURL.GameConst;
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
            string updateAssetListUrl = AssetManagerSetting.ServerURL.GetUpdateCsvURL(rootUrl);
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



            List<AssetFile> diffs = AssetFileList.Diff(AssetManagerSetting.versionAssetFileList, updateAssetList);


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

                AssetManagerSetting.versionAssetFileList.Add(item).SetVer(serverVer);

                AssetManagerSetting.persistentAssetFileList.Add(path, item.md5);
            }
            yield return new WaitForEndOfFrame();



            www = new WWW(AssetManagerSetting.PersistentFileURL.GameConst);
            yield return www;
            if(string.IsNullOrEmpty(www.error))
            {
                Debug.Log(AssetManagerSetting.PersistentFileURL.GameConst);
                Debug.Log(www.text);

                bool isSave = false;
                GameConstConfig gameConstConfig = JsonUtility.FromJson<GameConstConfig>(www.text);
                if (!gameConstConfig.updateEnableChangeCenterName)
                {
                    if (curGameConstConfig != null)
                    {
                        gameConstConfig.CenterName = curGameConstConfig.CenterName;
                    }
                    else if(appGameConstConfig != null)
                    {
                        gameConstConfig.CenterName = appGameConstConfig.CenterName;
                    }
                    isSave = true;
                }

                if (serverVersionInfo.isChangeGameConstVersion)
                {
                    gameConstConfig.Version = serverVersionInfo.version;
                    isSave = true;
                }

                if (isSave)
                {
                    gameConstConfig.Save(AssetManagerSetting.RootPathPersistent + AssetManagerSetting.FileName.GameConst);
                }
            }
            else
            {
                Debug.LogFormat("2 读取game_const.json失败 ReadGameConst_Persistent url={0} error={1}", AssetManagerSetting.PersistentFileURL.GameConst, www.error);
            }

            www.Dispose();
            www = null;





            AssetManagerSetting.versionAssetFileList.Save();
            AssetManagerSetting.persistentAssetFileList.Save();


            // 更新完成
            OnUpdateEnd();
        }
    }
}