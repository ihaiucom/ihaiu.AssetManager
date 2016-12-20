#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

namespace com.ihaiu
{
    public partial class AssetManagerSetting 
    {
        /** 编辑器模式下模拟加载--Config */
        public static bool EditorSimulateConfig       = true;
        /** 编辑器模式下模拟加载--AssetBundle */
        public static bool EditorSimulateAssetBundle  = true;


        /** 服务器--执行文件 */
        public static string EditorCheckServerExe = "Ihaiu/Editor/AssetBundleServer/UdpServer.exe";
        /** 服务器--执行文件 */
        public static string EditorAssetBundleServerExe = "Ihaiu/Editor/AssetBundleServer/AssetBundleServer.exe";
        /** 服务器--目录 */
        public static string EditorAssetBundleServerRoot_WWW                = Path.Combine (Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')), "www");
        /** 服务器--目录 */
        public static string EditorAssetBundleServerRoot_StreamingAssets    = Application.streamingAssetsPath;

        /** 目录--Resources */
        public static string EditorRootResources      = "Assets/Game/Resources";

        /** 目录--MResources */
        public static string EditorRootMResources      = "Assets/Game/MResources";

        /** 目录--Config */
        public static string EditorRootConfig           = "Assets/Game/Config";

        /** 目录--Lua */
        public static string EditorRootLua              = "Assets/Game/Lua";
        public static string EditorRootLuaBytes         = "Assets/Game/LuaBytes";

        /** 目录--StreamingAssets */
        public static string EditorRootStream         = "Assets/StreamingAssets";


        /** 目录--StreamingAssets/Platform/XXX */
        public static string EditorRootPlatform        = EditorRootStream + "/" + Platform.PlatformDirectory;

        public static string EditorRootVersion
        {
            get
            {
                return "version/" + Platform.PlatformDirectoryName + "/" + Games.GameConstConfig.Load().CenterName;
            }
        }



        /** 资源列表--Resources */
        public static string EditorFileCsvForResource = "Assets/Game/Resources/files.csv";
        /** 资源列表--StreamingAssets */
        public static string EditorFileCsvForStreaming = EditorRootPlatform + "/files.csv";
        /** 资源列表--UpdateList */
        public static string EditorUpdateAssetListPath     = EditorRootPlatform + "/UpdateAssetList.csv";
        /** 资源列表--LoadList */
        public static string EditorLoadAssetListPath     = EditorRootPlatform + "/LoadAssetList.csv";
        /** 资源列表--DontUnloadList */
        public static string EditorDontUnloadAssetListPath     = EditorRootPlatform + "/DontUnloadAssetList.csv";


        /** 获取绝对路径--StreamingAssets
         * path = "game_const.json"
         * return "Assets/StreamingAssets/game_const.json";
         */
        public static string EditorGetAbsoluteStreamPath(string path)
        {
            return EditorRootStream + "/" + path;
        }



        /** 获取AssetBundle的存放路径
         * assetBundleName="config-assetbundle"
         * return Assets/StreamingAssets/Platform/IOS/config-assetbundle
        */
        public static string EditorGetAbsolutePlatformPath(string assetBundleName)
        {
            return EditorRootPlatform + "/" + assetBundleName;
        }


        /** 获取Config文件路径 */
        public static string EditorGetConfigPath(string path)
        {

            path = path.Replace("Config/", EditorRootConfig + "/");
            string result = path + ".csv";
            if (File.Exists(result))
                return result;


            result = path + ".json";
            if (File.Exists(result))
                return result;

            result = path + ".txt";
            if (File.Exists(result))
                return result;

            return path;
        }


        /** 获取版本文件列表路径 */
        public static string EditorGetVersionFileListPath(string version)
        {
            return EditorRootVersion + "/"  + version  + ".csv";
        }

        public static string EditorToString()
        {
            string info = "";
            info += "\nAssetManagerSetting.EditorSimulateConfig : " + EditorSimulateConfig;
            info += "\nAssetManagerSetting.EditorSimulateAssetBundle : " + EditorSimulateAssetBundle;
            info += "\n";
            info += "\nAssetManagerSetting.EditorAssetBundleServerExe : " + EditorAssetBundleServerExe;
            info += "\nAssetManagerSetting.EditorAssetBundleServerRoot_WWW : " + EditorAssetBundleServerRoot_WWW;
            info += "\nAssetManagerSetting.EditorAssetBundleServerRoot_StreamingAssets : " + EditorAssetBundleServerRoot_StreamingAssets;
            info += "\n";
            info += "\nAssetManagerSetting.EditorRootMResources : " + EditorRootMResources;
            info += "\nAssetManagerSetting.EditorRootConfig : " + EditorRootConfig;
            info += "\nAssetManagerSetting.RootConfigBytes : " + RootConfigBytes;
            info += "\nAssetManagerSetting.EditorRootLua : " + EditorRootLua;
            info += "\nAssetManagerSetting.EditorRootLuaBytes : " + EditorRootLuaBytes;
            info += "\nAssetManagerSetting.EditorRootStream : " + EditorRootStream;
            info += "\nAssetManagerSetting.EditorRootPlatform : " + EditorRootPlatform;
            info += "\nAssetManagerSetting.EditorRootVersion : " + EditorRootVersion;
            info += "\nAssetManagerSetting.EditorFileCsvForResource : " + EditorFileCsvForResource;
            info += "\nAssetManagerSetting.EditorFileCsvForStreaming : " + EditorFileCsvForStreaming;
            info += "\nAssetManagerSetting.EditorUpdateAssetListPath : " + EditorUpdateAssetListPath;
            return info;
        }
    }
}

#endif
