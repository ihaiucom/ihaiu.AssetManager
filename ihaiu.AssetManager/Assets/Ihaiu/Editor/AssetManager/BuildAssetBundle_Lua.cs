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


        public static void Lua()
        {
            string luaRoot = AssetManagerSetting.EditorRootLua;
            string bytesRoot = AssetManagerSetting.EditorRootLuaBytes;

            if (!Directory.Exists(luaRoot))
            {
                Debug.Log("目录不存在" + luaRoot);
                return;
            }

            List<string> luaList = new List<string>();
            RecursiveLua(luaRoot, luaList);



            if (Directory.Exists(bytesRoot)) PathUtil.DeleteDirectory(bytesRoot);
            Directory.CreateDirectory(bytesRoot);

            for(int i = 0; i < luaList.Count; i ++)
            {
                string ext = Path.GetExtension(luaList[i]);
                if(ext.Equals(".lua"))
                {
                    string sourcePath = luaList[i];
                    string destPath = PathUtil.ChangeExtension(sourcePath.Replace(luaRoot, bytesRoot), AssetManagerSetting.BytesExt);

                    PathUtil.CheckPath(destPath, true);
                    File.Copy(sourcePath, destPath, true);
                }
            }


            AssetDatabase.Refresh();

            List<string> luaBytesList = new List<string>();
            RecursiveLuaBytes(bytesRoot, luaBytesList);

            string assetBundleName =  AssetManagerSetting.LuaAssetBundleName;

            AssetBundleBuild[] builds = new AssetBundleBuild[1];
            builds[0].assetBundleName =  assetBundleName;
            builds[0].assetNames = luaBytesList.ToArray();
            Debug.Log("luaBytesList.Count=" + luaBytesList.Count);

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

        }




    }

	

}