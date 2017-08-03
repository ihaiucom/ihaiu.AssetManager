using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;


namespace com.ihaiu
{
    public class AssetMenuItems 
    {

        [MenuItem ("资源管理/版本设置面板", false, 900)]
        public static void VersionReleaseWindowMenu()
        {
            VersionReleaseWindow.Open();
        }


        [MenuItem ("资源管理/版本推送面板", false, 900)]
        public static void VersionPushWindowMenu()
        {
            VersionPushWindow.Open();
        }

        [MenuItem ("资源管理/版本信息编辑面板", false, 900)]
        public static void VersionInfoWindowMenu()
        {
            VersionInfoWindow.Open();
        }



        [MenuItem("资源管理/RemoveUnusedAssetBundleNames")]
        public static void RemoveUnusedAssetBundleNames()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        [MenuItem("资源管理/ClearAssetBundleNames")]
        public static void ClearAssetBundleNames()
        {
            AssetBundleEditor.ClearAssetBundleNames();
        }

        [MenuItem("资源管理/Set AssetBundle Name")]
        public static void SetAssetBundleNames()
        {
            AssetBundleEditor.SetNames();
        }

        [MenuItem("资源管理/Build AssetBundle")]
        public static void BuildAssetBundles()
        {
            AssetBundleEditor.BuildAssetBundles();
        }

        [MenuItem("资源管理/Build Config AssetBundle")]
        public static void BuildConfig()
        {
            AB.Config();
        }

        [MenuItem("资源管理/Build Lua AssetBundle")]
        public static void BuildLua()
        {
            AB.Lua();
        }


        [MenuItem("资源管理/Generator AssetList_File.csv(StreamingAssets)")]
        public static void GeneratorFilesCsvForStreamingAssets()
        {
            AssetListCsvFile.Generator();
        }

        [MenuItem("资源管理/Generator LoadAssetList.csv")]
        public static void GeneratorLoadAssetListCsv()
        {
            
            AssetListCsvLoadMap.Generator(Games.GameConstConfig.Load().DevelopMode);
        }

        [MenuItem("资源管理/Generator DontUnloadAssetList.csv")]
        public static void GeneratorDontUnloadAssetListCsv()
        {
            AssetListCsvDontUnload.Generator(Games.GameConstConfig.Load().DevelopMode);
        }

        [MenuItem("资源管理/Clear Manifest Help File")]
        public static void ClearManifestHelpFile()
        {
            AssetBundleEditor.ClearManifestHelpFile();
        }





        [MenuItem("资源管理/CleanCache")]
        public static void CleanCache()
        {
            Caching.CleanCache();
        }




        [MenuItem("资源管理/查看AssetBundle")]
        public static void SeeAssetBundle()
        {
            SeeAssetBundleInfoWindow.Open();
        }

        [MenuItem("资源管理/生成ResZip")]
        public static void GeneratorResZip()
        {
            ResZipEditor.Install.Generator();
        }


        [MenuItem("资源管理/删除Zip的StreamingAssets资源")]
        public static void DeleteResZipForStreamingAssets()
        {
            ResZipEditor.Install.DeleteApp();
        }


    }
}