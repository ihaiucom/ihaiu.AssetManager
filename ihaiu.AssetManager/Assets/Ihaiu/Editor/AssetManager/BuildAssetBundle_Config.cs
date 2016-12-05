using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace com.ihaiu
{
    public partial class AB 
    {

        public static void Config()
        {
            string configRoot = AssetManagerSetting.EditorRootConfig;
            string bytesRoot = AssetManagerSetting.RootConfigBytes;

            if (!Directory.Exists(configRoot))
            {
                Debug.Log("目录不存在" + configRoot);
                return;
            }

            List<string> configList = new List<string>();
            Recursive(configRoot, configList);




            if (Directory.Exists(bytesRoot)) PathUtil.DeleteDirectory(bytesRoot);
            Directory.CreateDirectory(bytesRoot);

            for(int i = 0; i < configList.Count; i ++)
            {
                string sourcePath = configList[i];
                string destPath = PathUtil.ChangeExtension(sourcePath.Replace(configRoot, bytesRoot), AssetManagerSetting.BytesExt);

                PathUtil.CheckPath(destPath, true);
                File.Copy(sourcePath, destPath, true);
            }



            AssetDatabase.Refresh();

            List<string> bytesList = new List<string>();
            Recursive(bytesRoot, bytesList);


            string assetBundleName =  AssetManagerSetting.ConfigAssetBundleName;

            AssetBundleBuild[] builds = new AssetBundleBuild[1];
            builds[0].assetBundleName =  assetBundleName;
            builds[0].assetNames = bytesList.ToArray();
            Debug.Log("bytesList.Count=" + bytesList.Count);

            string outPath = bytesRoot;
            PathUtil.CheckPath(outPath, false);
            BuildPipeline.BuildAssetBundles(outPath, builds, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();

            string inAssetBundlePath = bytesRoot + "/" + assetBundleName;
            string outBytesPath = AssetManagerSetting.EditorGetAbsolutePlatformPath(assetBundleName);
            byte[] bytes = File.ReadAllBytes(inAssetBundlePath);

            bytes = EncryptBytes(bytes, SKey);

            PathUtil.CheckPath(outBytesPath, true);
            File.WriteAllBytes(outBytesPath, bytes);

            AssetDatabase.Refresh();

            if (Directory.Exists(bytesRoot)) PathUtil.DeleteDirectory(bytesRoot);
            AssetDatabase.Refresh();

        }




    }



}