using UnityEngine;
using System.Collections;
using System.IO;

namespace com.ihaiu
{
    public partial class AssetManagerSetting 
	{
        public class AssetBundleFileName
        {

            public static string Config  = "config" + AssetbundleExt;
            public static string Lua     = "luacode" + AssetbundleExt;
        }

        public class FileName
        {
            public static string GameConst                    = "game_const.json";


            /** 存储最终文件md5,用此对面更新文件是否需要更新 */
            public static string AssetList_Version           = "AssetList_Version.csv";
            /** 存储加载用Persistent文件的列表 */
            public static string AssetList_PersistentLoad    = "AssetList_PersistentLoad.csv";

            /** 存储Stream目录下的文件列表 path,md5 */
            public static string AssetList_File              = "AssetList_File.csv";
            /** App包含的资源 path,md5 */
            public static string AssetList_App               = "AssetList_App.csv";
            /** Zip包含的资源 path,md5 */
            public static string AssetList_Zip               = "AssetList_Zip.csv";
            /** 热更新的文件列表 path,md5 */
            public static string AssetList_Update            = "AssetList_Update.csv";
            /** 资源加载映射 loadType, path,  objType, assetBundleName, assetName, ext */
            public static string AssetList_LoadMap           = "AssetList_LoadMap.csv";
            /** 永久缓存 path */
            public static string AssetList_DontUnload         = "AssetList_DontUnload.csv";

            /** 本地版本信息 */
            public static string VersionLocalInfo         = "VersionLocalInfo.json";
            /** 资源包文件名 */
            public static string ResZip         = "res.zip";



        }

        public class FilePath
        {
        }


        public class SyncLoadFile
        {
            public static AssetBundle LoadAssetBundle(string assetBundleName)
            {
                string path = GetPlatformPath(assetBundleName);

                if (persistentAssetFileList.Has(path))
                {
                    path = RootPathPersistent + path;
                }
                else
                {
                    #if UNITY_ANDROID && !UNITY_EDITOR
                    path = Application.dataPath + "!assets/" + path;
                    #else
                    path = RootPathStreaming + path;
                    #endif
                }


                return AssetBundle.LoadFromFile(path);
            }


            public static byte[] LoadBytes(string fileName)
            {

                string path = GetPlatformPath(fileName);

                if (persistentAssetFileList.Has(path))
                {
                    path = RootPathPersistent + path;
                    return File.ReadAllBytes(path);
                }
                else
                {
                    #if UNITY_ANDROID && !UNITY_EDITOR
                    return AssetLoadSdk.LoadBytes(path);
                    #else
                    path = RootPathStreaming + path;
                    return File.ReadAllBytes(path);
                    #endif
                }
            }


            public static string LoadText(string fileName)
            {

                string path = GetPlatformPath(fileName);

                if (persistentAssetFileList.Has(path))
                {
                    path = RootPathPersistent + path;
                    return File.ReadAllText(path);
                }
                else
                {
                    #if UNITY_ANDROID && !UNITY_EDITOR
                    return AssetLoadSdk.LoadText(path);
                    #else
                    path = RootPathStreaming + path;
                    return File.ReadAllText(path);
                    #endif
                }
            }



            public static string LoadText_Streaming(string fileName)
            {

                string path = GetPlatformPath(fileName);
              
                #if UNITY_ANDROID && !UNITY_EDITOR
                return AssetLoadSdk.LoadText(path);
                #else
                path = RootPathStreaming + path;
                return File.ReadAllText(path);
                #endif
            }

            public static AssetBundle Manifest()
            {
                return LoadAssetBundle("{0}/" + Platform.PlatformDirectoryName);
            }

            public static string AssetListLoaMap()
            {
                return LoadText("{0}/" + FileName.AssetList_LoadMap);
            }


            public static string AssetListDontUnload()
            {
                return LoadText("{0}/" + FileName.AssetList_DontUnload);
            }

            public static string GameConst_Streaming()
            {
                return LoadText_Streaming(FileName.GameConst);
            }

            public static string AssetListApp_Streaming()
            {
                return LoadText_Streaming("{0}/" + FileName.AssetList_App);
            }

            public static byte[] AssetBundleConfig()
            {
                return LoadBytes("{0}/" + AssetBundleFileName.Config);
            }

        }

        public class PersistentFilePath
        {
            public static string GameConst                  = RootPathPersistent + FileName.GameConst;
            public static string VersionLocalInfo           = RootPathPersistent + FileName.VersionLocalInfo;
            public static string ResZip                     = RootPathPersistent + FileName.ResZip;
            public static string UnzipFolder                = RootPathPersistent;

            public static string AssetListVersion           = RootPathPersistent + FileName.AssetList_Version;
            public static string AssetListPersistentLoad    = RootPathPersistent + FileName.AssetList_PersistentLoad;
            public static string AssetListZip               = RootPathPersistent + FileName.AssetList_Zip;


        }


        public class StreamingFilePath
        {
            /** 资源列表文件路径
             * return "StreamingAssets/Platform/IOS/AssetList_File.csv"
             */
            public static string AssetListFile
            {
                get
                {
                    return RootPathStreaming + GetPlatformPath("{0}/" + FileName.AssetList_File);
                }
            }

        }

        public class FileURL
        {

            /** AssetBundleManifest文件路径 */
            public static string ManifestURL
            {
                get
                {
                    return GetAbsolutePlatformURL("{0}/" + Platform.PlatformDirectoryName);
                }
            }

            public static string AssetBundleConfig
            {
                get
                {
                    return GetAbsolutePlatformURL("{0}/" + AssetBundleFileName.Config);
                }
            }


            public static string AssetBundleLua
            {
                get
                {
                    return GetAbsolutePlatformURL("{0}/" + AssetBundleFileName.Lua);
                }
            }


            /** 资源列表文件路径--加载映射
             * return "StreamingAssets/Platform/IOS/AssetList_LoadMap.csv"
             * return "Res/Platform/IOS/AssetList_LoadMap.csv"
             */
            public static string AssetListLoaMap
            {
                get
                {
                    return GetAbsolutePlatformURL("{0}/" + FileName.AssetList_LoadMap);
                }
            }

            /** 资源列表文件路径--永久缓存
             * return "StreamingAssets/Platform/IOS/AssetList_DontUnload.csv"
             * return "Res/Platform/IOS/AssetList_DontUnload.csv"
             */
            public static string AssetListDontUnload
            {
                get
                {
                    return GetAbsolutePlatformURL("{0}/" + FileName.AssetList_DontUnload);
                }
            }
        }


        public class PersistentFileURL
        {
            public static string GameConst    = RootUrlPersistent     + FileName.GameConst;
        }

        public class StreamingFileURL
        {
            public static string GameConst     = RootUrlStreaming      + FileName.GameConst;

            /** 资源列表文件路径--文件列表
             * return "StreamingAssets/Platform/IOS/AssetList_File.csv"
             */
            public static string AssetListFile
            {
                get
                {
                    return GetAbsolutePlatformURL("{0}/" + FileName.AssetList_File);
                }
            }


            /** 资源列表文件路径--文件列表
             * return "StreamingAssets/Platform/IOS/AssetList_App.csv"
             */
            public static string AssetListApp
            {
                get
                {
                    return GetAbsolutePlatformURL("{0}/" + FileName.AssetList_App);
                }
            }

        }

        public class ServerURL
        {

            public static string VersionInfoUrlFormat       = "?p=kcj_res.git/.git;a=blob_plain;hb=refs/heads/{0};f={0}/verinfo/{2}ver_{1}.txt";

            /** 服务器资源更新列表URL
             * root =  "http://112.126.75.68:8080/StreamingAssets/"
             * return  "http://112.126.75.68:8080/StreamingAssets/Platform/XXX/UpdateAssetList.csv"
             */
            public static string GetUpdateCsvURL(string root)
            {
                return root + GetPlatformPath("{0}/" + FileName.AssetList_Update);
            }

            /** 获取服务器版本信息URL */
            public static string GetServerVersionInfoURL(string root, string centerName, bool isTest = false)
            {
                //            return root + "/kcj_conf/" + Platform.PlatformDirectoryName.ToLower() + "/" + centerName + "/" + AssetManagerSetting.VersionInfoName;

                if (Games.GameConst.WebUrlIsDevelop)
                {
                    return root + "/ver_" + centerName.ToLower() + ".txt";
                }
                else
                {
					return root + string.Format(VersionInfoUrlFormat, Platform.PlatformDirectoryName.ToLower(), centerName.ToLower(), isTest ? "test_" : "", DateTimeUtils.CurrentTimestamp, Application.version);
                }
            }
        }

    }
}
