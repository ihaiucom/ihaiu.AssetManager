using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Ihaiu.Assets
{
    public class AssetMenuItems 
    {
       



        [MenuItem("AssetManager/RemoveUnusedAssetBundleNames")]
        public static void RemoveUnusedAssetBundleNames()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        [MenuItem("AssetManager/ClearAssetBundleNames")]
        public static void ClearAssetBundleNames()
        {
            AssetBundleEditor.ClearAssetBundleNames();
        }

        [MenuItem("AssetManager/Set AssetBundle Name")]
        public static void SetAssetBundleNames()
        {
            AssetBundleEditor.SetNames();
        }

        [MenuItem("AssetManager/Build AssetBundle")]
        public static void BuildAssetBundles()
        {
            AssetBundleEditor.BuildAssetBundles();
        }

        [MenuItem("AssetManager/Build Config AssetBundle")]
        public static void BuildConfig()
        {
            AB.Config();
        }

        [MenuItem("AssetManager/Build Lua AssetBundle")]
        public static void BuildLua()
        {
            AB.Lua();
        }


        [MenuItem("AssetManager/Generator files.csv(Resources)")]
        public static void GeneratorFileCsvFromResources()
        {
            FilesCsvForResources.Generator();
        }


        [MenuItem("AssetManager/Generator files.csv(StreamingAssets)")]
        public static void GeneratorFilesCsvForStreamingAssets()
        {
            FilesCsvForStreamingAssets.Generator();
        }


        [MenuItem("AssetManager/Clear Manifest Help File")]
        public static void ClearManifestHelpFile()
        {
            AssetBundleEditor.ClearManifestHelpFile();
        }



        [MenuItem("AssetManager/Generator AssetBundle Info")]
        public static void GeneratorAssetBundleInfo()
        {
            AssetBundleEditor.GeneratorAssetBundleInfo();
        }
    }
}