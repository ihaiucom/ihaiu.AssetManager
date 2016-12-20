using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Games;

namespace com.ihaiu
{
    public class VersionInfo 
    {
        public string version = "0.0.0";

        public string downLoadUrl   = "http://127.0.0.1:8080/app.apk";
        public string updateLoadUrl = "http://127.0.0.1:8080/StreamingAssets/";

        public string noteDownApp       = "需要下载新版本";
        public string noteHostUpdate    = "需要更新才可继续游戏，更新包大小为<color=red>2KB</color>，建议使用<color=red>Wi-Fi</color>网络下载";

		public int isClose = 0;



        #if UNITY_EDITOR
        public static VersionInfo Load(string path)
        {
            var f = new FileInfo(path);
            if (f.Exists)
            {
                var sr = f.OpenText();
                var str = sr.ReadToEnd();
                sr.Close();

                VersionInfo config = JsonUtility.FromJson<VersionInfo>(str);
                return config;
            }
            else
            {
                return new VersionInfo();
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
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("[VersionInfoJsonGenerator]" + filesPath);
        }
        #endif

        public override string ToString()
        {
            return string.Format("[VersionInfo] version={0}, downLoadUrl={1}, updateLoadUrl={2}", version, downLoadUrl, updateLoadUrl);
        }

        public void Set()
        {
        }
    }
}