using UnityEngine;
using System.Collections;

namespace Ihaiu.Assets
{
    public partial class AssetManagerSetting 
    {
        public static bool TestVersionMode = false;
        public static string BytesExt = ".txt";
        public static string AssetbundleExt = "-assetbundle";

        public static string MResourcesRoot      = "Assets/Game/MResources";

        public static string ConfigRoot         = "Assets/Game/Config";
        public static string ConfigBytesRoot    = "Assets/Game/ConfigBytes";


        public static string LuaRoot            = "Assets/Game/Lua";
        public static string LuaBytesRoot       = "Assets/Game/LuaBytes";

        public static string PathRoot = Application.streamingAssetsPath + "/";
        public static string URLRoot = "file:///" + Application.streamingAssetsPath + "/";

        public static string RootPathStreaming
        {
            get
            {
                return Application.streamingAssetsPath + "/";
            }
        }

        public static string RootPathPersistent
        {
            get
            {
                #if UNITY_EDITOR
                if(Application.isEditor)
                {
                    if(TestVersionMode)
                    {
                        return Application.dataPath + "/../res/";
                    }
                    else
                    {
                        return RootPathStreaming;
                    }
                }
                #endif

                #if UNITY_STANDALONE
                switch(Application.platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                        return Application.dataPath + "/../res/" ;
                    case RuntimePlatform.OSXPlayer:
                        return Application.dataPath + "/res/" ;
                }
                #endif

                return Application.persistentDataPath + "/";
            }
        }


        /** 是否严格 */
        public static bool IsStrict = true;

        /** 强制异步加载,等待一帧(Resource.AsyLoad) */
        public static bool ForcedResourceAsynLoadWaitFrame = true;


        /** 获取绝对URL
         * path = "Platform/IOS/config.assetbundle"
         * return "file:///xxxx/Platform/IOS/config.assetbundle";
         */
        public static string GetAbsoluteURL(string path)
        {
            return URLRoot + path;
        }

        /** 获取绝对路径
         * path = "Platform/IOS/config.assetbundle"
         * return "xxxx/Platform/IOS/config.assetbundle";
         */
        public static string GetAbsolutePath(string path)
        {
            return PathRoot + path;
        }



        /** 获取平台文件路径
         * path = "{0}/config.assetbundle"
         * return "Platform/IOS/config.assetbundle"
         */
        public static string GetPlatformPath(string path)
        {
            return string.Format(path, Platform.PlatformDirectory);
        }

        /** 获取平台文件URL
         * path="{0}/config.assetbundle"
         * return "file:///xxxx/Platform/IOS/config.assetbundle";
        */
        public static string GetPlatformURL(string path)
        {
            return GetAbsoluteURL(GetPlatformPath(path));
        }

        /** AssetBundleManifest文件路径 */
        public static string ManifestURL
        {
            get
            {
                return GetAbsoluteURL(Platform.ManifestPath);
            }
        }

        #region files.csv
        public static string FileCsvForResource = "Assets/Game/Resources/files.csv";

        /** 资源列表文件路径
         * return "files.csv" in "Resources" directory
         */
        public static string AssetlistForResource = "files";

        public static string FileCsvForStreaming
        {
            get
            {
                return GetAbsolutePath(GetPlatformPath("{0}/files.csv"));
            }
        }

        /** 资源列表文件路径
         * return "StreamingAssets/Platform/IOS/files.csv"
         * return "Res/Platform/IOS/files.csv"
         */
        public static string AssetlistForStreaming
        {
            get
            {
                return GetPlatformURL("{0}/files.csv");
            }
        }

        /** 服务器资源更新列表
         * root =  "http://112.126.75.68:8080/StreamingAssets/"
         * return  "http://112.126.75.68:8080/StreamingAssets/"
         */
        public static string GetServerUpdateAssetlistURL(string root)
        {
            return root + GetPlatformPath("{0}/files.csv");
        }
        #endregion files.csv




        /** 获取配置文件的AssetName
         * filename = "config/skill";
         * return "Assets/Game/ConfigBytes/skill.txt"
         */
        public static string GetConfigAssetName(string filename)
        {
            return filename.ToLower().Replace("config/", ConfigBytesRoot) + BytesExt;
        }


        /** 是否是Config文件
         * filename = "config/skill"; return true;
         * filename = "panel/heropanel" return false;
        */
        public static bool IsConfigFile(string filename)
        {
            return filename.ToLower().IndexOf("config/") == 0;
        }


        /** 获取资源Type */
        public static System.Type GetObjType(string objType)
        {
            switch(objType)
            {
                case "Sprite":
                    return typeof(Sprite);

                case "GameObject":
                    return typeof(GameObject);

                default:
                    return typeof(System.Object);
            }
        }


    }
}
