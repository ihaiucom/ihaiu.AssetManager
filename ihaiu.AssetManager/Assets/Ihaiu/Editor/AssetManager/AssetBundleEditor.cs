using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Ihaiu.Assets
{
    public class AssetBundleEditor 
    {
        static string resourceRoot = AssetManagerSetting.MResourcesRoot;
        static string[] resourcesPaths = new string[]{
            resourceRoot,
    //        "Assets/Ihaiu/AssetManagerExampleFiles/Component"

        };


        static string assetbundleExt { get { return AssetManagerSetting.AssetbundleExt; } }

        static string[] filterDirList = new string[]{};
        static List<string> filterExts = new List<string>{".cs", ".js"};
        static List<string> imageExts = new List<string>{".png", ".jpg", ".jpeg", ".bmp", "gif", ".tga", ".tiff", ".psd"};
        static bool isSpriteTag = true;

        static List<string> exts = new List<string>(new string[]{ ".prefab", ".png", ".jpg", ".jpeg", ".bmp", "gif", ".tga", ".tiff", ".psd", ".mat", ".mp3", ".wav" });


        public static void ClearAssetBundleNames()
        {
            string[] names = AssetDatabase.GetAllAssetBundleNames();

            int count = names.Length;
            for(int i = 0; i < count; i ++)
            {
                if (names[i].IndexOf(assetbundleExt) != -1)
                {
                    string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(names[i]);
                    for (int j = 0; j < assets.Length; j++)
                    {
                        AssetImporter importer = AssetImporter.GetAtPath(assets[j]);
                        importer.assetBundleName = null;
                    }
                }
            }
        }


        public static void SetNames()
        {
            List<string> list = new List<string>();
            PathUtil.RecursiveFile(resourcesPaths, list, exts);




            // 生成所有节点
            Dictionary<string, AssetNode> nodeDict = AssetNodeUtil.GenerateAllNode(list, filterDirList, filterExts, imageExts, isSpriteTag, assetbundleExt);

            // 生成每个节点依赖的节点
            AssetNodeUtil.GenerateNodeDependencies(nodeDict);


            // 寻找入度为0的节点
            List<AssetNode> roots = AssetNodeUtil.FindRoots(nodeDict);

            // 移除父节点的依赖和自己依赖相同的节点
            AssetNodeUtil.RemoveParentShare(roots);


            // 强制设置某些节点为Root节点，删掉被依赖
            AssetNodeUtil.ForcedSetRoots(nodeDict, list);


            // 寻找入度为0的节点
            roots = AssetNodeUtil.FindRoots(nodeDict);

            // 入度为1的节点自动打包到上一级节点
            AssetNodeUtil.MergeParentCountOnce(roots);

            // 生成需要设置AssetBundleName的节点
            Dictionary<string, AssetNode> assetDict = AssetNodeUtil.GenerateAssetBundleNodes(roots);


            // 设置AssetBundleNames
            AssetNodeUtil.SetAssetBundleNames(assetDict, resourceRoot, assetbundleExt);


            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        public static void BuildAssetBundles()
        {
            string outputPath = AssetManagerSetting.BuildPlatformRoot;
            PathUtil.CheckPath(outputPath, false);
            Debug.Log("outputPath=" + outputPath);

            BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh();
        }

        public static void ClearManifestHelpFile()
        {
            string outputPath = AssetManagerSetting.BuildPlatformRoot;

            List<string> fileList = new List<string>();
            PathUtil.RecursiveFile(outputPath, fileList, new List<string>(new string[]{".manifest"}));

            for(int i = 0; i < fileList.Count; i ++)
            {
                PathUtil.DeleteFile(fileList[i]);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}