using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Games;

namespace Ihaiu.Assets
{
    [Serializable]
    public class NoticeInfo
    {
        public string qqGroupKey = "0POfj73j3PTMfn2w6XAeAHvVVa6fwvXK";
        public string qqNumber = "344021892";
        public string content = "<size=50>尊敬的主公:</size>\n加入核心玩家群344021892<color=#0000ffff><i>（点击加入）</i></color>, 加群即可领取礼包：100元宝+500银币。";
    }

    public class VersionInfo 
    {
        public string version = "0.0.0";

        public string downLoadUrl = "https://beta.bugly.qq.com/t4cg";
        public string updateLoadUrl = "http://112.126.75.68:8080/StreamingAssets/";

        public string noteAll = "服务器维护<color=red>预计13:00开服</color>";
        public string noteLite = "需要更新才可继续游戏，更新包大小为<color=red>2KB</color>，建议使用<color=red>Wi-Fi</color>网络下载";

		public int isClose = 0;
		public string noteClose = "关服公告";

        public NoticeInfo noticeBoard = new NoticeInfo();
        public string UploadVideo = "http://101.200.195.25:8000/";
        public int isShareOpen = 0;


        #if UNITY_EDITOR
        public static VersionInfo Load(string serverRoot)
        {
            string path = serverRoot + "/" + AssetManagerSetting.VersionInfoName;

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