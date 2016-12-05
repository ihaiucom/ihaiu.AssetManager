using UnityEngine;
using System.Collections;
using System.IO;

namespace com.ihaiu
{
    public partial class AssetManagerSetting 
    {
        /** 是否是测试版本模式 */
        public static bool TestVersionMode = true;
        /** 加载超时(秒) */
        public static int LoadTimeOut           = 10;

        public static string BytesExt           = ".txt";
        public static string AssetbundleExt     = "-assetbundle";

        public static string ConfigAssetBundleName  = "config" + AssetbundleExt;
        public static string LuaAssetBundleName     = "luacode" + AssetbundleExt;

        public static string PersistentAssetListName    = "PersistentAssetList.csv";
        public static string FilesName                  = "files.csv";
        public static string AssetBundleListName        = "AssetBundleList.csv";
        public static string AssetListName              = "AssetList.csv";
        public static string UpdateAssetListName        = "UpdateAssetList.csv";
        public static string LoadAssetListName          = "LoadAssetList.csv";
        public static string DontUnloadAssetListName          = "DontUnloadAssetList.csv";
        public static string GameConstName              = "game_const.json";
        public static string VersionInfoName            = "version";

        public static string RootConfigBytes            = "Assets/Game/ConfigBytes";


        public static string AssetFileListPath              = RootPathPersistent + AssetListName;
        public static string PersistentAssetFileListPath    = RootPathPersistent + PersistentAssetListName;


        public static AssetFileList persistentAssetFileList = new AssetFileList();
        public static AssetFileList dontUnloadAssetFileList = new AssetFileList();

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


        public static string GameConstPath = RootPathPersistent + GameConstName;



        /** 强制异步加载,等待一帧(Resource.AsyLoad) */
        public static bool ForcedResourceAsynLoadWaitFrame = true;
        /** 是否缓存Resouces加载的对象 */
        public static bool IsCacheResourceAsset = false;
        /** 是否缓存AssetBundle加载的对象 */
        public static bool IsCacheAssetBundleAsset = false;

        /** 检测缓存时间间隔（秒） */
        public static float CheckCacheAssetRate = 10;
        /** 缓存时间（秒） */
        public static float CacheAssetTime = 60;
        /** 是否使用缓存时间 */
        public static bool UseCacheAssetTime = true;
        public static bool CheckCacheActive = true;











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
            #if UNITY_IPHONE
            return GetAbsoluteURL(Platform.PlatformDirectory + "/" + assetBundleName).Replace(" ", "%20");
            #else
            return GetAbsoluteURL(Platform.PlatformDirectory + "/" + assetBundleName);
            #endif
        }










        /** 获取绝对Path
         * path = "Platform/IOS/config.assetbundle"
         * return "xxxx/Platform/IOS/config.assetbundle";
         */
        public static string GetAbsolutePath(string path)
        {
            if (persistentAssetFileList.Has(path))
            {
                return RootPathPersistent + path;
            }
            else
            {
                return RootPathStreaming + path;
            }
        }


        /** 获取平台文件Path
         * path="{0}/config.assetbundle"
         * return "xxxx/Platform/IOS/config.assetbundle";
        */
        public static string GetAbsolutePlatformPath(string path)
        {
            return GetAbsolutePath(GetPlatformPath(path));
        }


        public static string GetAbsoluteAssetBundlePath(string assetBundleName)
        {
            return GetAbsolutePath(Platform.PlatformDirectory + "/" + assetBundleName);
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

        /** 资源列表文件路径
         * return "StreamingAssets/Platform/IOS/LoadAssetList.csv"
         * return "Res/Platform/IOS/LoadAssetList.csv"
         */
        public static string LoadAssetListURL
        {
            get
            {
                return GetAbsolutePlatformURL("{0}/" + LoadAssetListName);
            }
        }

        /** 资源列表文件路径
         * return "StreamingAssets/Platform/IOS/DontUnloadAssetList.csv"
         * return "Res/Platform/IOS/DontUnloadAssetList.csv"
         */
        public static string DontUnloadAssetListURL
        {
            get
            {
                return GetAbsolutePlatformURL("{0}/" + DontUnloadAssetListName);
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
        public static string GetServerVersionInfoURL(string root, string centerName)
        {
            return root + "/versioninfo/" + Platform.PlatformDirectoryName.ToLower() + "/" + centerName + "/" + AssetManagerSetting.VersionInfoName;
        }


        public static string ConfigAssetBundleURL
        {
            get
            {
                return GetAbsolutePlatformURL("{0}/" + ConfigAssetBundleName);
            }
        }



        public static string LuaAssetBundleURL
        {
            get
            {
                return GetAbsolutePlatformURL("{0}/" + LuaAssetBundleName);
            }
        }

        /** 获取配置文件的AssetName
         * filename = "config/skill";
         * return "Assets/Game/ConfigBytes/skill.txt"
         */
        public static string GetConfigAssetName(string filename)
        {
            return filename.Replace("Config", RootConfigBytes) + BytesExt;
        }


        /** 是否是Config文件
         * filename = "config/skill"; return true;
         * filename = "panel/heropanel" return false;
        */
        public static bool IsConfigFile(string filename)
        {
            return filename.ToLower().IndexOf("config/") == 0;
        }








        public const string ObjType_Texture      = "Texture";
        public const string ObjType_Sprite         = "Sprite";
        public const string ObjType_GameObject     = "GameObject";

        private static System.Type tmpSpriteType = typeof(Sprite);
        private static System.Type tmpGameObjectType = typeof(GameObject);
        private static System.Type tmpTextureType = typeof(Texture);
        private static System.Type tmpObjectType = typeof(System.Object);

        /** 获取资源Type */
        public static System.Type GetObjType(string objType)
        {
            switch(objType)
            {
                case ObjType_Sprite:
                    return tmpSpriteType;

                case ObjType_GameObject:
                    return tmpGameObjectType;

                case ObjType_Texture:
                    return tmpTextureType;

                default:
                    return tmpObjectType;
            }
        }


        public static string GetInfo()
        {
            string info = "";
            info += "\nAssetManagerSetting.ForcedResourceAsynLoadWaitFrame : " + ForcedResourceAsynLoadWaitFrame;
            info += "\nAssetManagerSetting.IsCacheResourceAsset : " + IsCacheResourceAsset;
            info += "\n";
            info += "\nAssetManagerSetting.TestVersionMode : " + TestVersionMode;
            info += "\nAssetManagerSetting.LoadTimeOut : " + LoadTimeOut;
            info += "\nAssetManagerSetting.AssetbundleExt : " + AssetbundleExt;
            info += "\nAssetManagerSetting.ConfigAssetBundleName : " + ConfigAssetBundleName;
            info += "\nAssetManagerSetting.LuaAssetBundleName : " + LuaAssetBundleName;

            info += "\n";
            info += "\nAssetManagerSetting.FilesName : " + FilesName;
            info += "\nAssetManagerSetting.PersistentAssetListName : " + PersistentAssetListName;
            info += "\nAssetManagerSetting.AssetListName : " + AssetListName;
            info += "\nAssetManagerSetting.UpdateAssetListName : " + UpdateAssetListName;
            info += "\nAssetManagerSetting.LoadAssetListName : " + LoadAssetListName;
            info += "\nAssetManagerSetting.DontUnloadAssetListName : " + DontUnloadAssetListName;
            info += "\nAssetManagerSetting.GameConstName : " + GameConstName;
            info += "\nAssetManagerSetting.VersionInfoName : " + VersionInfoName;


            info += "\n";
            info += "\nAssetManagerSetting.RootPathStreaming : " + RootPathStreaming;
            info += "\nAssetManagerSetting.RootPathPersistent : " + RootPathPersistent;
            info += "\nAssetManagerSetting.RootUrlStreaming : " + RootUrlStreaming;
            info += "\nAssetManagerSetting.RootUrlPersistent : " + RootUrlPersistent;
            info += "\n";
            info += "\nAssetManagerSetting.AssetFileListPath : " + AssetFileListPath;
            info += "\nAssetManagerSetting.PersistentAssetFileListPath : " + PersistentAssetFileListPath;
            info += "\n";
            info += "\nAssetManagerSetting.GameConstUrl_Streaming : " + GameConstUrl_Streaming;
            info += "\nAssetManagerSetting.GameConstUrl_Persistent : " + GameConstUrl_Persistent;
            info += "\nAssetManagerSetting.GameConstPath : " + GameConstPath;

            info += "\n";
            info += "\nAssetManagerSetting.FilesCsvForStreaming : " + FilesCsvForStreaming;
            info += "\nAssetManagerSetting.ManifestURL : " + ManifestURL;
            info += "\nAssetManagerSetting.ConfigAssetBundleURL : " + ConfigAssetBundleURL;

            #if UNITY_EDITOR
            info += "\n";
            info += EditorToString();
            #endif


            return info;
        }


    }
}
