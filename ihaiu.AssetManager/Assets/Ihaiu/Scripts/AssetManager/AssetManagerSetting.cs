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
        public static int LoadTimeOut           = 20;
        /** WWW加载网络资源最多缓存多少个 */
        public static int WWWCacheMaxNum        = 50;
        /** 是否是同步加载方式 */
        public static bool SyncLoadType         = true;

        public static string BytesExt           = ".txt";
        public static string AssetbundleExt     = "-assetbundle";
        public static string RootConfigBytes    = "Assets/Game/ConfigBytes";

        /** 永久缓存资源列表 */
        public static AssetFileList dontUnloadAssetFileList = new AssetFileList();


        /** 加载文件指向目录 RootPathPersistent */
        private static AssetFileList _persistentAssetFileList;
        public static AssetFileList persistentAssetFileList
        {
            get
            {
                if (_persistentAssetFileList == null)
                {
                    _persistentAssetFileList = AssetFileList.Read(PersistentFilePath.AssetListPersistentLoad);
                }

                return _persistentAssetFileList;
            }
        }

        /** 用来检测资源版本，是否需要更新 */
        private static AssetFileList _versionAssetFileList;
        public static AssetFileList versionAssetFileList
        {
            get
            {
                if (_versionAssetFileList == null)
                {
                    _versionAssetFileList = AssetFileList.Read(PersistentFilePath.AssetListVersion);
                }

                return _versionAssetFileList;
            }
        }



        /** Zip里的资源 */
        private static AssetFileList _zipAssetFileList;
        public static AssetFileList zipAssetFileList
        {
            get
            {
                if (_zipAssetFileList == null)
                {
                    _zipAssetFileList = AssetFileList.Read(PersistentFilePath.AssetListZip);
                }

                return _zipAssetFileList;
            }
        }


        /** 资源加载情况收集 */
        public static AssetCollect  collect            = new AssetCollect();
        /** 是否开启资源加载情况收集 */
        public static bool          IsCollect          = true;

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
            #if UNITY_EDITOR
            if(!TestVersionMode)
            {
                return RootUrlStreaming + path;
            }
            #endif

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

        public static string GetAbsoluteAssetBundlePath(string assetBundleName)
        {


            string path = Platform.PlatformDirectory + "/" + assetBundleName;

            #if UNITY_ANDROID && !UNITY_EDITOR
            if (persistentAssetFileList.Has(path))
            {
                return RootPathPersistent + path;
            }
            else
            {
                return Application.dataPath + "!assets/" + path;
            }
            #else
            if (persistentAssetFileList.Has(path))
            {
                return RootPathPersistent + path;
            }
            else
            {
                return RootPathStreaming + path;
            }
            #endif
        }

        public static string GetSdkPlatformPath(string path)
        {
            return GetSdkPath(GetPlatformPath(path));
        }

        public static string GetSdkPath(string path)
        {
            if (persistentAssetFileList.Has(path))
            {
                return RootPathPersistent + path;
            }
            else
            {
                #if UNITY_ANDROID && !UNITY_EDITOR
                return path;
                #else
                return RootPathStreaming + path;
                #endif
            }
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











        /** 获取平台文件路径
         * path = "{0}/config.assetbundle"
         * return "Platform/IOS/config.assetbundle"
         */
        public static string GetPlatformPath(string path)
        {
            return string.Format(path, Platform.PlatformDirectory);
        }

        /** 获取平台（参数）相对路径
         * path = "Platform/IOS/config.assetbundle"
         * return "{0}/config.assetbundle"
         */
        public static string ToPlatformPath(string path)
        {
            return path.Replace(Platform.PlatformDirectory, "{0}");
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








        public const string ObjType_Texture         = "Texture";
        public const string ObjType_Sprite          = "Sprite";
        public const string ObjType_GameObject      = "GameObject";

        public static System.Type tmpSpriteType = typeof(Sprite);
        public static System.Type tmpGameObjectType = typeof(GameObject);
        public static System.Type tmpTextureType = typeof(Texture);
        public static System.Type tmpObjectType = typeof(System.Object);

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

    }
}
