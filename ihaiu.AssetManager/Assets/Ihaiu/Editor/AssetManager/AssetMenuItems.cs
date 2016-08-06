using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Ihaiu.Assets
{
    public class AssetMenuItems 
    {
        const string kSimulationAssetBundleMode = "资源/Simulation AssetBundle Mode";

        [MenuItem(kSimulationAssetBundleMode)]
        public static void ToggleSimulationAssetBundleMode ()
        {
            AssetManagerSetting.SimulateAssetBundleInEditor = !AssetManagerSetting.SimulateAssetBundleInEditor;
        }

        [MenuItem(kSimulationAssetBundleMode, true)]
        public static bool ToggleSimulationAssetBundleModeValidate ()
        {
            Menu.SetChecked(kSimulationAssetBundleMode, AssetManagerSetting.SimulateAssetBundleInEditor);
            return true;
        }



        const string kSimulationConfigMode = "资源/Simulation Config Mode";

        [MenuItem(kSimulationConfigMode)]
        public static void ToggleSimulationConfigMode ()
        {
            AssetManagerSetting.SimulateConfigInEditor = !AssetManagerSetting.SimulateConfigInEditor;
        }

        [MenuItem(kSimulationConfigMode, true)]
        public static bool ToggleSimulationConfigModeValidate ()
        {
            Menu.SetChecked(kSimulationConfigMode, AssetManagerSetting.SimulateConfigInEditor);
            return true;
        }



        [MenuItem("资源/RemoveUnusedAssetBundleNames")]
        public static void RemoveUnusedAssetBundleNames()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        [MenuItem("资源/ClearAssetBundleNames")]
        public static void ClearAssetBundleNames()
        {
            AssetBundleEditor.ClearAssetBundleNames();
        }

        [MenuItem("资源/Set AssetBundle Name")]
        public static void SetAssetBundleNames()
        {
            AssetBundleEditor.SetNames();
        }

        [MenuItem("资源/Build AssetBundle")]
        public static void BuildAssetBundles()
        {
            AssetBundleEditor.BuildAssetBundles();
        }

        [MenuItem("资源/Build Config AssetBundle")]
        public static void BuildConfig()
        {
            AB.Config();
        }

        [MenuItem("资源/Build Lua AssetBundle")]
        public static void BuildLua()
        {
            AB.Lua();
        }


        [MenuItem("资源/Generator files.csv(Resources)")]
        public static void GeneratorFileCsvFromResources()
        {
            FilesCsvForResources.Generator();
        }


        [MenuItem("资源/Generator files.csv(StreamingAssets)")]
        public static void GeneratorFilesCsvForStreamingAssets()
        {
            FilesCsvForStreamingAssets.Generator();
        }


        [MenuItem("资源/Clear Manifest Help File")]
        public static void ClearManifestHelpFile()
        {
            AssetBundleEditor.ClearManifestHelpFile();
        }
    }
}