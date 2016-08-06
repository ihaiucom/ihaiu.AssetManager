using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


namespace Ihaiu.Assets
{
    public class VersionManager : MonoBehaviour
    {
        public Version appVer;
        public Version curVer;
        public Version serverVer;

        void OnUpdateEnter()
        {
        }

        void OnUpdateEnd()
        {
        }

        void OnUpdateFailed()
        {
        }

        void OnUpdateProgress()
        {
        }

//        IEnumerator UpdateResource(string url)
//        {
//            // url = "http://112.126.75.68:8080/StreamingAssets/"
//            OnUpdateEnter();
//
//            //获取服务器端的file.csv
//
//            string filesUrl = AssetManagerSetting.GetServerUpdateAssetlistURL(url);
//            Debug.Log("get filesUrls: " + filesUrl);
//            WWW www = new WWW(filesUrl);
//            yield return www;
//
//            if (!string.IsNullOrEmpty(www.error))
//            {
//                Debug.LogError(string.Format("<color={0}>{1}</color>", "red", "[Error] 更新服务器资源" + filesUrl + "\n" + www.error));
//                www.Dispose();
//                www = null;
//                yield break;
//            }
//
//            string filesText = www.text;
//            string[] files = filesText.Split('\n');
//            www.Dispose();
//            www = null;
//            //Debug.Log("count: " + files.Length + " text: " + filesText);
//            //获取本地的file.csv
//            string gameConstFile = "";
//            List<string> updateFIleList = new List<string>();
//            for (int i = 0; i < files.Length; i++)
//            {
//                if (string.IsNullOrEmpty(files[i])) continue;
//                string[] keyValue = files[i].Split(';');
//                if (keyValue.Length < 2)
//                {
//                    continue;
//                }
//                string filePath = keyValue[0].Trim();
//                string SvrstrMd5 = keyValue[1].Trim();
//                if (filePath == "game_const.json")//game_const最后更新
//                {
//                    filePath =  AssetManagerSetting.GetPlatformPath(filePath);
//                    gameConstFile = filePath;
//                    continue;
//                }
//               
//                filePath =  AssetManagerSetting.GetPlatformPath(filePath);
//                string localfile = AssetManagerSetting.GetAbsolutePath(filePath).Trim();
//
//                bool localfileExist = File.Exists(localfile);
//
//                if (localfileExist == true)
//                {
//                    string localMd5 = PathUtil.md5file(localfile);
//                    if (SvrstrMd5 != localMd5)
//                    {
//                        Debug.Log("local file md5 not: " + localfile);
//                        updateFIleList.Add(filePath);
//                    }
//                }
//                else
//                {
//                    Debug.Log("local file not exit: " + localfile);
//                    updateFIleList.Add(filePath);
//                }
//            }
//            //game_const最后更新
//            updateFIleList.Add(gameConstFile);
//
//            //更新
//            int count = updateFIleList.Count;
//            for (int i = 0; i < count; i++)
//            {
//                string keyValue = updateFIleList[i];
//
//                OnUpdateProgress((float)(i + 1) / (float)count);
//
//                string svrFileUfl = url + keyValue;
//                www = new WWW(svrFileUfl);
//
//                yield return www;
//                if (!string.IsNullOrEmpty(www.error))
//                {
//                    OnUpdateFailed(svrFileUfl);
//
//                    www.Dispose();
//                    www = null;
//                    continue;
//                }
//                Debug.Log("get from url: " + svrFileUfl);
//                string localfile = AssetManagerSetting.GetAbsolutePath(keyValue) .Trim();
//                PathUtil.CheckPath(localfile, true);
//                File.WriteAllBytes(localfile, www.bytes);
//
//                www.Dispose();
//                www = null;
//            }
//
//            yield return new WaitForEndOfFrame();
//
//            string localfilesPath = PathUtil.DataPath + GameConst.Platform_AssetlistNameForStreaming;
//            File.WriteAllText(localfilesPath, filesText);
//
//            // 更新完成
//            OnUpdateEnd();
//        }
    }
}