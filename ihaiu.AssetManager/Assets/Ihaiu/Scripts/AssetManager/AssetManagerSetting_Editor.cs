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
        public static string EditorCheckServerExe       = "Game/Scripts/CC/Editor/AssetBundleServer/UdpServer.exe";
        /** 服务器--执行文件 */
        public static string EditorAssetBundleServerExe = "Game/Scripts/CC/Editor/AssetBundleServer/AssetBundleServer.exe";
        /** 服务器--目录 */
        public static string EditorAssetBundleServerRoot_WWW                = Path.Combine (Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')), "www");
        /** 服务器--目录 */
        public static string EditorAssetBundleServerRoot_StreamingAssets    = Application.streamingAssetsPath;
        /** 服务器--目录 */
        public static string EditorAssetBundleServerRoot_Workspace          = EditorRoot.WorkspaceStream;


        public class EditorRoot
        {
            /** 目录--Resources */
            public static string Resources      = "Assets/Game/Resources";

            /** 目录--MResources */
            public static string MResources      = "Assets/Game/MResources";

            /** 目录--Config */
            public static string Config           = "Assets/Game/Config";

            /** 目录--Lua */
            public static string Lua              = "Assets/Game/Lua";
            public static string LuaBytes         = "Assets/Game/LuaBytes";

            /** 目录--StreamingAssets */
            public static string Stream             = "Assets/StreamingAssets/";

            /** 目录--StreamingAssets/Platform/XXX */
            public static string StreamPlatform     = Stream + Platform.PlatformDirectory + "/";



            /** 目录--Workspace */
            public static string Workspace                  = "Workspace/";

            /** 目录--Workspace/IOS/ */
            public static string WorkspacePlaforom          = Workspace + Platform.PlatformDirectoryName + "/";

            /** 目录--Workspace/IOS/StreamingAssets/ */
            public static string WorkspaceStream            = WorkspacePlaforom + "StreamingAssets/";

            /** 目录--Workspace/IOS/StreamingAssets/Platform/XXX */
            public static string WorkspaceStreamPlatform    = WorkspaceStream + Platform.PlatformDirectory + "/";

            /** 目录--分包资源--Workspace/IOS/res/ */
            public static string WorkspaceResZip            = WorkspacePlaforom + "res/";

            /** 目录--Workspace/IOS/NoCenter/ */
            public static string WorkspaceVersion
            {
                get
                {
                    return WorkspacePlaforom + "Version/"+  Games.GameConstConfig.Load().CenterName + "/";
                }
            }




            /** 目录--Workspace/Tmp/ */
            public static string WorkspaceTmp                   = Workspace + "Tmp/";

            /** 目录--Workspace/Tmp/AssetCollect/ */
            public static string WorkspaceAssetCollect          = WorkspaceTmp + "AssetCollect/";
        }

        public class EditorWorkspaceFilePath
        {
            /** 资源列表--game_const.json */
            public static string GameConst              = EditorRoot.WorkspaceStream + FileName.GameConst;

            /** 资源列表--AssetList_File.csv */
            public static string AssetListFile          = EditorRoot.WorkspaceStreamPlatform + FileName.AssetList_File;
            /** 资源列表--AssetList_Update.csv */
            public static string AssetListUpdate        = EditorRoot.WorkspaceStreamPlatform + FileName.AssetList_Update;
            /** 资源列表--AssetList_LoadMap.csv */
            public static string AssetListLoadMap       = EditorRoot.WorkspaceStreamPlatform + FileName.AssetList_LoadMap;
            /** 资源列表--AssetList_DontUnload.csv */
            public static string AssetListDontUnload    = EditorRoot.WorkspaceStreamPlatform + FileName.AssetList_DontUnload;

            /** 分包资源 res.zip */
            public static string ResZip                 = EditorRoot.WorkspacePlaforom + FileName.ResZip;
            /** 分包资源 AssetList_Zip.json */
            public static string AssetListZip           = EditorRoot.WorkspacePlaforom + FileName.AssetList_Zip;
            /** 分包资源 AssetList_App.csv */
            public static string AssetListApp           = EditorRoot.WorkspacePlaforom + FileName.AssetList_App;

            /** 资源列表--Workspace/IOS/AssetCollect_Guide.json */
            public static string AssetCollectGuide      = EditorRoot.WorkspacePlaforom + "AssetCollect_Guide.json";
        }



        public class EditorStreamFilePath
        {
            /** 资源列表--game_const.json */
            public static string GameConst              = EditorRoot.Stream + FileName.GameConst;
            /** 资源列表--AssetList_LoadMap.csv */
            public static string AssetListLoadMap       = EditorRoot.StreamPlatform + FileName.AssetList_LoadMap;
            /** 资源列表--AssetList_DontUnload.csv */
            public static string AssetListDontUnload    = EditorRoot.StreamPlatform + FileName.AssetList_DontUnload;

        }




        /** 获取绝对路径--StreamingAssets
         * path = "game_const.json"
         * return "Assets/StreamingAssets/game_const.json";
         */
        public static string EditorGetAbsoluteStreamPath(string path)
        {
            return EditorRoot.Stream + path;
        }



        /** 获取AssetBundle的存放路径
         * assetBundleName="config-assetbundle"
         * return Assets/StreamingAssets/Platform/IOS/config-assetbundle
        */
        public static string EditorGetAbsolutePlatformPath(string assetBundleName)
        {
            return EditorRoot.StreamPlatform + assetBundleName;
        }


        /** 获取AssetBundle的存放路径
         * assetBundleName="config-assetbundle"
         * return Workspace/IOS/StreamingAssets/Platform/IOS/config-assetbundle
        */
        public static string EditorGetAbsoluteWorkspaceStreamPlatformPath(string assetBundleName)
        {
            return EditorRoot.WorkspaceStreamPlatform + "/" + assetBundleName;
        }

       
        /** 获取Config文件路径 */
        public static string EditorGetConfigPath(string path)
        {

            path = path.Replace("Config/", EditorRoot.Config + "/");
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
            return EditorRoot.WorkspaceVersion + "/"  + version  + ".csv";
        }

    }
}

#endif
