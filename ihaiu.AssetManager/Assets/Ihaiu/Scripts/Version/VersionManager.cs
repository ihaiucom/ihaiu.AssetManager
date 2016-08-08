using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Games;


namespace Ihaiu.Assets
{
    public class VersionManager : MonoBehaviour
    {
        private Version appVer = new Version();
        private Version curVer = new Version();
        private Version serverVer = new Version();

        private VersionInfo serverVersionInfo;

        private GameConstConfig appGameConstConfig;
        private GameConstConfig curGameConstConfig;

        void OnUpdateEnter()
        {
        }

        void OnUpdateEnd()
        {
        }

        void OnUpdateFailed(string url)
        {
        }

        void OnUpdateProgress(float progress)
        {
        }

        void OnNeedDownApp(string url)
        {
        }

        void OnFinal()
        {
        }


        IEnumerator CheckVersion()
        {
            yield return StartCoroutine(ReadGameConst_Streaming());

            #if UNITY_EDITOR
            if (appGameConstConfig.DevelopMode)
            {
                appGameConstConfig.Set();
                OnFinal();
                AssetManagerSetting.persistentAssetFileList.Save(AssetManagerSetting.PersistentAssetFileListPath);
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

            if (needInitData)
            {
                yield return StartCoroutine(InitData());
                curVer.Copy(appVer);
            }


            yield return StartCoroutine(ReadServerVersionInfo());

            if (serverVersionInfo != null)
            {
                serverVer.Parse(serverVersionInfo.version);

                VersionCheckState state = VersionCheck.CheckState(curVer, serverVer);

                switch(state)
                {
                    case VersionCheckState.DownApp:
                        OnNeedDownApp(serverVersionInfo.downLoadUrl);
                        break;

                    case VersionCheckState.HotUpdate:
                        yield return StartCoroutine(UpdateResource(serverVersionInfo.updateLoadUrl));
                        yield return StartCoroutine(ReadGameConst_Persistent());
                        curGameConstConfig.Set();
                        OnFinal();
                        break;
                    default:
                        AssetManagerSetting.persistentAssetFileList = AssetFileList.Read(AssetManagerSetting.PersistentAssetFileListPath);
                        OnFinal();
                        break;
                }
            }
            else
            {
                OnFinal();
            }
        }

        IEnumerator InitData()
        {
            List<string> list = new List<string>();
            list.Add(AssetManagerSetting.GameConstName);
            list.Add(AssetManagerSetting.AssetListName);



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


            AssetManagerSetting.persistentAssetFileList.Save(AssetManagerSetting.PersistentAssetFileListPath);

        }


        /** 获取服务器版本号 */
        IEnumerator ReadServerVersionInfo()
        {
            string url = AssetManagerSetting.GetServerVersionInfoURL(GameConst.WebUrl) ;
            WWW www = new WWW(url);
            yield return www;
            if(!string.IsNullOrEmpty(www.error))
            {
                Debug.LogErrorFormat("获取服务器版本号出错 url={0}, www.error={1}", url, www.error);
                www.Dispose();
                www = null;

                yield break;
            }


            serverVersionInfo = JsonUtility.FromJson<VersionInfo>(www.text);

        }


        /** 读取streaming下只读配置  */
        IEnumerator ReadGameConst_Streaming()
        {   
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
                Debug.LogErrorFormat("读取game_const.json失败 ReadGameConst_Streaming url={0} error={1}  text={2}", url, www.error);
            }

            www.Dispose();
            www = null;
        }


        IEnumerator ReadGameConst_Persistent()
        {   
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
                Debug.LogFormat("读取game_const.json失败 ReadGameConst_Persistent url={0} error={1}  text={2}", url, www.error);
            }

            www.Dispose();
            www = null;
        }



        IEnumerator UpdateResource(string rootUrl)
        {
            // rootUrl = "http://www.ihaiu.com/StreamingAssets/"
            OnUpdateEnter();

            //获取服务器端的file.csv

            string updateAssetListUrl = AssetManagerSetting.GetServerUpdateAssetlistURL(rootUrl);
            Debug.Log("UpdateAssetList URL: " + updateAssetListUrl);
            WWW www = new WWW(updateAssetListUrl);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogErrorFormat("更新资源读取资源列表失败 updateAssetListUrl={0}, www.error={1}", updateAssetListUrl,  www.error);
                www.Dispose();
                www = null;
                yield break;
            }

            AssetFileList updateAssetList = AssetFileList.Deserialize(www.text);
            www.Dispose();
            www = null;
            //Debug.Log("count: " + files.Length + " text: " + filesText);


            AssetFileList assetFileList = AssetFileList.Read(AssetManagerSetting.AssetFileListPath);

            List<AssetFile> diffs = AssetFileList.Diff(assetFileList, updateAssetList);

           

            //更新
            int count = diffs.Count;
            for (int i = 0; i < count; i++)
            {
                AssetFile item = diffs[i];

                OnUpdateProgress((float)(i + 1) / (float)count);

                string url = rootUrl + item.path;
                www = new WWW(url);

                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    OnUpdateFailed(url);

                    www.Dispose();
                    www = null;
                    continue;
                }

                string localPath = AssetManagerSetting.RootPathPersistent + item.path;
                PathUtil.CheckPath(localPath, true);
                File.WriteAllBytes(localPath, www.bytes);

                www.Dispose();
                www = null;

                assetFileList.Add(item);

                AssetManagerSetting.persistentAssetFileList.Add(item);
            }
            yield return new WaitForEndOfFrame();


            assetFileList.Save(AssetManagerSetting.AssetFileListPath);
            AssetManagerSetting.persistentAssetFileList.Save(AssetManagerSetting.PersistentAssetFileListPath);


            // 更新完成
            OnUpdateEnd();
        }
    }
}