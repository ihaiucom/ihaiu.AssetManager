using UnityEngine;
using System.Collections;
using System.IO;

namespace Ihaiu.Assets
{
    public partial class AssetManagerSetting 
    {
        public static bool TestVersionMode = true;

        public static string BytesExt           = ".txt";
        public static string AssetbundleExt     = "-assetbundle";

        public static string ConfigAssetBundleName  = "config" + AssetbundleExt;
        public static string LuaAssetBundleName     = "luacode" + AssetbundleExt;

        public static string PersistentAssetListName    = "PersistentAssetList.csv";
        public static string FilesName                  = "files.csv";
        public static string AssetBundleListName        = "AssetBundleList.csv";
        public static string AssetListName              = "AssetList.csv";
        public static string UpdateAssetListName        = "UpdateAssetList.csv";
        public static string GameConstName              = "game_const.json";
        public static string VersionInfoName            = string.Format("version_{0}.json", Platform.PlatformDirectoryName.ToLower());



        public static string AssetFileListPath              = RootPathPersistent + AssetListName;
        public static string PersistentAssetFileListPath    = RootPathPersistent + PersistentAssetListName;


        public static AssetFileList persistentAssetFileList = new AssetFileList();

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
                if(TestVersionMode)
                {
                    return Application.dataPath + "/../res/";
                }
                else
                {
                    return RootPathStreaming;
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

        public static string RootUrlStreaming
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android) 
                {
                    return RootPathStreaming;
                }
                else
                {
                    return "file:///" + RootPathStreaming;
                }
            }
        }


        public static string RootUrlPersistent
        {
            get
            {
                return "file:///" + RootPathPersistent;
            }
        }

        public static string GameConstUrl_Streaming     = RootUrlStreaming      + GameConstName;
        public static string GameConstUrl_Persistent    = RootUrlPersistent     + GameConstName;



        /** 强制异步加载,等待一帧(Resource.AsyLoad) */
        public static bool ForcedResourceAsynLoadWaitFrame = true;


        /** 获取绝对URL
         * path = "Platform/IOS/config.assetbundle"
         * return "file:///xxxx/Platform/IOS/config.assetbundle";
         */
        public static string GetAbsoluteURL(string path)
        {
            if (persistentAssetFileList.Has(path))
            {
                return RootUrlPersistent + path;
            }
            else
            {
                return RootUrlStreaming + path;
            }
        }


        /** 获取平台文件URL
         * path="{0}/config.assetbundle"
         * return "file:///xxxx/Platform/IOS/config.assetbundle";
        */
        public static string GetAbsolutePlatformURL(string path)
        {
            return GetAbsoluteURL(GetPlatformPath(path));
        }


        public static string GetAbsoluteAssetBundleURL(string assetBundleName)
        {
            return GetAbsoluteURL(Platform.PlatformDirectory + "/" + assetBundleName);
        }


        /** 获取平台文件路径
         * path = "{0}/config.assetbundle"
         * return "Platform/IOS/config.assetbundle"
         */
        public static string GetPlatformPath(string path)
        {
            return string.Format(path, Platform.PlatformDirectory);
        }

        /** AssetBundleManifest文件路径 */
        public static string ManifestURL
        {
            get
            {
                return GetAbsolutePlatformURL("{0}/" + Platform.PlatformDirectoryName);
            }
        }

        #region files.csv

        /** 资源列表文件路径
         * return "files.csv" in "Resources" directory
         */
        public static string FilesCsvForResource = "files";

        /** 资源列表文件路径
         * return "StreamingAssets/Platform/IOS/files.csv"
         * return "Res/Platform/IOS/files.csv"
         */
        public static string FilesCsvForStreaming
        {
            get
            {
                return GetAbsolutePlatformURL("{0}/" + FilesName);
            }
        }

        /** 服务器资源更新列表URL
         * root =  "http://112.126.75.68:8080/StreamingAssets/"
         * return  "http://112.126.75.68:8080/StreamingAssets/Platform/XXX/UpdateAssetList.csv"
         */
        public static string GetServerFilesCsvURL(string root)
        {
            return root + GetPlatformPath("{0}/" + UpdateAssetListName);
        }
        #endregion files.csv

        /** 获取服务器版本信息URL */
        public static string GetServerVersionInfoURL(string root)
        {
            return root + VersionInfoName;
        }




        public static string ConfigAssetBundleURL
        {
            get
            {
                return GetAbsolutePlatformURL("{0}/" + ConfigAssetBundleName);
            }
        }



        /** 获取配置文件的AssetName
         * filename = "config/skill";
         * return "Assets/Game/ConfigBytes/skill.txt"
         */
        public static string GetConfigAssetName(string filename)
        {
            return Path.GetFileName(filename);
        }


        /** 是否是Config文件
         * filename = "config/skill"; return true;
         * filename = "panel/heropanel" return false;
        */
        public static bool IsConfigFile(string filename)
        {
            return filename.ToLower().IndexOf("config/") == 0;
        }








        public const string ObjType_Sprite         = "Sprite";
        public const string ObjType_GameObject     = "GameObject";

        /** 获取资源Type */
        public static System.Type GetObjType(string objType)
        {
            switch(objType)
            {
                case ObjType_Sprite:
                    return typeof(Sprite);

                case ObjType_GameObject:
                    return typeof(GameObject);

                default:
                    return typeof(System.Object);
            }
        }


    }
}
