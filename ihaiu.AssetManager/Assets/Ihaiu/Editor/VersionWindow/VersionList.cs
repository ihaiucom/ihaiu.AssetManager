using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;


namespace com.ihaiu
{
    
    public class VersionList : ScriptableSingleton<VersionList>
    {
        public static List<Version>     list = new  List<Version>();
        public static List<string>      versionStrList = new List<string>();
        public static string[]          versionStrArr = new string[0];


        /** 版本App版本列表 */
        public static List<Version>     appVersionList = new List<Version>();
        public static List<string>      appVersionStrList = new List<string>();
        public static string[]          appVersionStrArr = new string[0];


        /** 最终App版本对应的补丁版本号 */
        public static Dictionary<string, Version> lastAppRevisedDict = new Dictionary<string, Version>();


        /** 最终版本号 */
        public static Version lastVersion       = new Version();
        /** 最终App版本号 */
        public static Version lastAppVersion    = new Version();


        [SerializeField]
        bool     isRead = false;

        static Version tmp = new Version();
        public static void Read(bool force = false)
        {
            if (!force && instance.isRead)
                return;

            lastAppVersion = new Version();
            lastVersion = new Version();

            list.Clear();
            appVersionList.Clear();
            lastAppRevisedDict.Clear();

            string root = AssetManagerSetting.EditorRootVersion;

            PathUtil.CheckPath(root, false);

            string[] names = Directory.GetFiles(root);
            foreach (string filename in names) 
            {
                string ext = Path.GetExtension(filename).ToLower();
                if (ext != ".csv")
                    continue;


                string fn = Path.GetFileName(filename).Replace(".csv", string.Empty);
                Version version = Version.ParseVersion(fn);
                list.Add(version);

                if (lastVersion.Comparer(version) <= 0)
                {
                    lastVersion.Copy(version);
                }

                if (version.verType == VersionType.App)
                {
                    if (lastAppVersion.Comparer(version) <= 0)
                    {
                        lastAppVersion.Copy(version);
                    }

                    appVersionList.Add(version);
                }

                tmp.Copy(version);
                tmp.revised = 0;
                tmp.verType = VersionType.App;

                Version lastAppRevised;
                if (!lastAppRevisedDict.TryGetValue(tmp.ToStringNoDate(), out lastAppRevised))
                {
                    lastAppRevised = new Version();
                    lastAppRevisedDict.Add(tmp.ToStringNoDate(), lastAppRevised);
                }

                if (lastAppRevised.Comparer(version) <= 0)
                {
                    lastAppRevised.Copy(version);
                }
            }

            appVersionStrList.Clear();
            appVersionList.Sort(Version.SortComparer);
            appVersionStrArr = new string[appVersionList.Count];
            for(int i = 0; i < appVersionList.Count; i ++)
            {
                appVersionStrArr[i] = appVersionList[i].ToDateString();
                appVersionStrList.Add(appVersionList[i].ToString());
            }


            versionStrList.Clear();
            list.Sort(Version.SortComparer);
            versionStrArr = new string[list.Count];
            for(int i = 0; i < list.Count; i ++)
            {
                versionStrArr[i] = list[i].ToString();
                versionStrList.Add(versionStrArr[i]);
            }

        }

        public static Version GetLastAppRevised(Version app)
        {
            tmp.Copy(app);
            tmp.revised = 0;

            Version lastAppRevised;
            if (!lastAppRevisedDict.TryGetValue(tmp.ToStringNoDate(), out lastAppRevised))
            {
                lastAppRevised = new Version();
                lastAppRevised.Copy(app);
                lastAppRevisedDict.Add(lastAppRevised.ToStringNoDate(), lastAppRevised);
            }
            return lastAppRevised;
        }

    	
    }

}