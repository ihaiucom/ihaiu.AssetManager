#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace Ihaiu.Assets
{
    public partial class AssetManagerSetting 
    {


        public static string MResourcesRoot      = "Assets/Game/MResources";

        public static string ConfigRoot         = "Assets/Game/Config";
        public static string ConfigBytesRoot    = "Assets/Game/ConfigBytes";


        public static string LuaRoot            = "Assets/Game/Lua";
        public static string LuaBytesRoot       = "Assets/Game/LuaBytes";



        /** 获取绝对路径
         * path = "Platform/IOS/config.assetbundle"
         * return "xxxx/Platform/IOS/config.assetbundle";
         */
        public static string GetAbsolutePath(string path)
        {
            return RootPathStreaming + path;
        }

        public static string FileCsvForResource = "Assets/Game/Resources/files.csv";

        public static string FileCsvForStreaming
        {
            get
            {
                return GetAbsolutePath(GetPlatformPath("{0}/files.csv"));
            }
        }


   
        static int m_SimulateAssetBundleInEditor = -1;
        const string kSimulateAssetBundles = "SimulateAssetBundles";

        /** 编辑器模式模拟加载AssetBundle。用AssetDatabase */
        public static bool SimulateAssetBundleInEditor 
        {
            get
            {
                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor)
                {
                    m_SimulateAssetBundleInEditor = newValue;
                    EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
        }


        static int m_SimulateConfigInEditor = -1;
        const string kSimulateConfig = "SimulateConfig";

        /** 编辑器模式模拟加载Config。用AssetDatabase */
        public static bool SimulateConfigInEditor 
        {
            get
            {
                if (m_SimulateConfigInEditor == -1)
                    m_SimulateConfigInEditor = EditorPrefs.GetBool(kSimulateConfig, true) ? 1 : 0;

                return m_SimulateConfigInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateConfigInEditor)
                {
                    m_SimulateConfigInEditor = newValue;
                    EditorPrefs.SetBool(kSimulateConfig, value);
                }
            }
        }

        /** 获取配置文件相对项目路径
         * path = "Config/stage_map"
         * return "Assets/Games/Config/stage_map.csv";
         */
        public static string GetConfigPath(string path)
        {
            string result = string.Format("Assets/Games/{0}.csv", path);
            if (File.Exists(result))
            {
                return result;
            }
            return string.Format("Assets/Games/{0}.json", path);
        }



        /** 打包AssetBundle的根目录
         *  return Assets/StreamingAssets/Platform/IOS/
        */
        public static string BuildPlatformRoot
        {
            get
            {
                return Application.streamingAssetsPath + "/" + Platform.GetPlatformDirectory(EditorUserBuildSettings.activeBuildTarget);
            }
        }

        /** 获取打包AssetBundle的存放路径
         * assetBundleName="config-assetbundle"
         * return Assets/StreamingAssets/Platform/IOS/config-assetbundle
        */
        public static string GetBuildPlatformPath(string assetBundleName)
        {
            return BuildPlatformRoot + "/" + assetBundleName;
        }







    }
}

#endif
