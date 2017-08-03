using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace com.ihaiu
{

    public class ResZipEditor 
    {
        

        private static ResZipEditor _install;
        public static ResZipEditor Install
        {
            get
            {
                if (_install == null)
                {
                    _install = new ResZipEditor();
                }
                return _install;
            }
        }


        public void Generator()
        {
            GenerateAssetListZipAndApp();
            CopyToResZip();
            GeneratorZip();
        }

        public void GenerateAssetListZipAndApp()
        {
            string listPath = AssetManagerSetting.EditorWorkspaceFilePath.AssetListFile;
            if (!File.Exists(listPath))
            {
                Debug.LogError("不存在:" + listPath);
                EditorWindow.mouseOverWindow.ShowNotification(new GUIContent("不存在:" + listPath));
                return;
            }

            if (!File.Exists(AssetManagerSetting.EditorWorkspaceFilePath.AssetCollectGuide))
            {
                Debug.LogError("不存在:" + AssetManagerSetting.EditorWorkspaceFilePath.AssetCollectGuide);
                EditorWindow.mouseOverWindow.ShowNotification(new GUIContent("不存在:" + AssetManagerSetting.EditorWorkspaceFilePath.AssetCollectGuide));
                return;
            }

            AssetFileList   assetList = AssetFileList.Read(listPath);
            AssetCollect    collect = AssetCollect.LoadGuide();
            collect.InternalListToDict();
            collect.OnLoadInternal(AssetManagerSetting.FileName.GameConst);
            collect.OnLoadInternal(AssetManagerSetting.AssetBundleFileName.Config);
            collect.OnLoadInternal(AssetManagerSetting.AssetBundleFileName.Lua);
            collect.OnLoadInternal(Platform.PlatformDirectoryName);


            string roleHeads = @"Image/Hero/hero_20001
            Image/Hero/hero_20002
            Image/Hero/hero_20003
            Image/Hero/hero_20102
            Image/Hero/hero_20105
            Image/Hero/hero_20107
            Image/Hero/hero_20201
            Image/Hero/hero_20202
            Image/Hero/hero_20305
            Image/Hero/hero_20310
            Image/Soldier/Soldier_30101
            Image/Soldier/Soldier_30102
            Image/Soldier/Soldier_30103
            Image/Soldier/Soldier_30136
            Image/Soldier/Soldier_30161
            Image/Soldier/Soldier_30165";

            string[] roleHeadArr = roleHeads.Split('\n');
            for(int i = 0; i < roleHeadArr.Length; i ++)
            {
                string assetBundleName = roleHeadArr[i].Trim().ToLower() + AssetManagerSetting.AssetbundleExt;
                if(!collect.internalDict.ContainsKey(assetBundleName))
                    collect.OnLoadInternal(assetBundleName);
            }


            Debug.Log("guid asset count=" + collect.internalDict.Count);


            AssetFileList appAssetList = new AssetFileList();
            AssetFileList zipAssetList = new AssetFileList();


            string path;
            bool isApp = false;

            for(int i = 0; i < assetList.list.Count; i ++)
            {
                path = assetList.list[i].path.Replace("{0}/", "");

                isApp = collect.internalDict.ContainsKey(path);

                if (!isApp && path.StartsWith("assets/art/"))
                    isApp = true;


                if (!isApp && path.StartsWith("assets/game/arts_effect"))
                    isApp = true;


				if (!isApp && path.StartsWith("assets/builtin/"))
					isApp = true;

                if (!isApp && path.StartsWith("image/hero/hero_list_"))
                    isApp = true;

				if (!isApp && path.StartsWith("image/common/"))
					isApp = true;

				if (!isApp && path.StartsWith("image/emoji/"))
					isApp = true;

				if (!isApp && path.StartsWith("image/item/"))
					isApp = true;

				if (!isApp && path.StartsWith("image/lang/"))
					isApp = true;

                if (!isApp && path.StartsWith("module/"))
                    isApp = true;

				if (!isApp && path.StartsWith("materials/"))
					isApp = true;

                if (!isApp && path.StartsWith("shaders/"))
                    isApp = true;

                if (!isApp && path.EndsWith(AssetManagerSetting.FileName.AssetList_LoadMap) ||
                    path.EndsWith(AssetManagerSetting.FileName.AssetList_DontUnload)
                )
                {
                    isApp = true;
                }


                if (isApp)
                {
                    appAssetList.Add(assetList.list[i]);
                }
                else
                {
                    zipAssetList.Add(assetList.list[i]);
                }

            }

            AssetFile item = assetList.Get("{0}/" + AssetManagerSetting.FileName.AssetList_DontUnload);
            if (item != null)
            {
                appAssetList.Add(item);
            }
            item = assetList.Get("{0}/" + AssetManagerSetting.FileName.AssetList_LoadMap);
            if (item != null)
            {
                appAssetList.Add(item);
            }

            appAssetList.Save(AssetManagerSetting.EditorWorkspaceFilePath.AssetListApp);
            zipAssetList.Save(AssetManagerSetting.EditorWorkspaceFilePath.AssetListZip);
        }


        public void CopyToResZip()
        {
            string listPath = AssetManagerSetting.EditorWorkspaceFilePath.AssetListZip;
            if (!File.Exists(listPath))
            {
                GenerateAssetListZipAndApp();
            }

            if (!File.Exists(listPath))
            {
                Debug.LogError("不存在:" + listPath);
                EditorWindow.mouseOverWindow.ShowNotification(new GUIContent("不存在:" + listPath));
                return;
            }


            if (Directory.Exists(AssetManagerSetting.EditorRoot.WorkspaceResZip))
            {
                Directory.Delete(AssetManagerSetting.EditorRoot.WorkspaceResZip, true);
            }

            Debug.Log("listPath=" + listPath);
            AssetFileList assetList = AssetFileList.Read(listPath);

            string src;
            string dst;
            string dir;
            for(int i = 0 ; i < assetList.list.Count; i ++)
            {
                src = AssetManagerSetting.EditorRoot.WorkspaceStream + AssetManagerSetting.GetPlatformPath(assetList.list[i].path);
                dst = AssetManagerSetting.EditorRoot.WorkspaceResZip + AssetManagerSetting.GetPlatformPath(assetList.list[i].path);


                if (!File.Exists(src))
                {
                    continue;
                }

                dir = Path.GetDirectoryName(dst);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.Copy(src, dst, true);
            }


            dst = AssetManagerSetting.EditorRoot.WorkspaceResZip  + AssetManagerSetting.GetPlatformPath("{0}/" + AssetManagerSetting.FileName.AssetList_Zip);
            File.Copy(listPath, dst, true);
        }

        public void GeneratorZip()
        {
            ZipHelper.ZipDirectory(AssetManagerSetting.EditorRoot.WorkspaceResZip, AssetManagerSetting.EditorWorkspaceFilePath.ResZip);
        }



        public void CopyToStreaming_UnZip()
        {
            string listPath = AssetManagerSetting.EditorWorkspaceFilePath.AssetListApp;
            if (!File.Exists(listPath))
            {
                GenerateAssetListZipAndApp();
            }

            if (!File.Exists(listPath))
            {
                Debug.LogError("不存在:" + listPath);
                EditorWindow.mouseOverWindow.ShowNotification(new GUIContent("不存在:" + listPath));
                return;
            }


            AssetFileList assetList = AssetFileList.Read(listPath);

            string src;
            string dst;
            string dir;
            for(int i = 0 ; i < assetList.list.Count; i ++)
            {
                src = AssetManagerSetting.EditorRoot.WorkspaceStream + AssetManagerSetting.GetPlatformPath(assetList.list[i].path);
                dst = AssetManagerSetting.EditorRoot.Stream + AssetManagerSetting.GetPlatformPath(assetList.list[i].path);
                if (!File.Exists(src))
                {
                    continue;
                }

                dir = Path.GetDirectoryName(dst);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.Copy(src, dst, true);
            }

            dst = AssetManagerSetting.EditorRoot.Stream + AssetManagerSetting.GetPlatformPath("{0}/" + AssetManagerSetting.FileName.AssetList_App);
            File.Copy(listPath, dst, true);

            AssetDatabase.Refresh();
        }


        public void CopyToStreaming_All()
        {
            string listPath = AssetManagerSetting.EditorWorkspaceFilePath.AssetListFile;
            if (!File.Exists(listPath))
            {
                GenerateAssetListZipAndApp();
            }

            if (!File.Exists(listPath))
            {
                Debug.LogError("不存在:" + listPath);
                EditorWindow.mouseOverWindow.ShowNotification(new GUIContent("不存在:" + listPath));
                return;
            }


            AssetFileList assetList = AssetFileList.Read(listPath);

            string src;
            string dst;
            string dir;
            for(int i = 0 ; i < assetList.list.Count; i ++)
            {
                src = AssetManagerSetting.EditorRoot.WorkspaceStream + AssetManagerSetting.GetPlatformPath(assetList.list[i].path);
                dst = AssetManagerSetting.EditorRoot.Stream + AssetManagerSetting.GetPlatformPath(assetList.list[i].path);
                if (!File.Exists(src))
                {
                    continue;
                }

                dir = Path.GetDirectoryName(dst);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.Copy(src, dst, true);
            }

            dst = AssetManagerSetting.EditorRoot.Stream + AssetManagerSetting.GetPlatformPath("{0}/" + AssetManagerSetting.FileName.AssetList_App);
            File.Copy(listPath, dst, true);

            AssetDatabase.Refresh();
        }



        public void DeleteApp()
        {
            string listPath = AssetManagerSetting.EditorWorkspaceFilePath.AssetListZip;
            if (!File.Exists(listPath))
            {
                GenerateAssetListZipAndApp();
            }

            if (!File.Exists(listPath))
            {
                Debug.LogError("不存在:" + listPath);
                EditorWindow.mouseOverWindow.ShowNotification(new GUIContent("不存在:" + listPath));
                return;
            }


            AssetFileList assetList = AssetFileList.Read(listPath);

            DeleteApp(assetList);
        }


        private void DeleteApp(AssetFileList assetList)
        {
            Dictionary<string, bool> deleteIgnoreDict = new Dictionary<string, bool>();
            deleteIgnoreDict.Add(AssetManagerSetting.FileName.GameConst, true);
            deleteIgnoreDict.Add("{0}/" + AssetManagerSetting.AssetBundleFileName.Config, true);
            deleteIgnoreDict.Add("{0}/" + AssetManagerSetting.AssetBundleFileName.Lua, true);
            deleteIgnoreDict.Add("{0}/" + AssetManagerSetting.FileName.AssetList_App, true);
            deleteIgnoreDict.Add("{0}/" + AssetManagerSetting.FileName.AssetList_LoadMap, true);
            deleteIgnoreDict.Add("{0}/" + AssetManagerSetting.FileName.AssetList_DontUnload, true);
            deleteIgnoreDict.Add("{0}/" + Platform.PlatformDirectoryName, true);

            string src;
            for(int i = 0; i < assetList.list.Count; i ++)
            {
                if (deleteIgnoreDict.ContainsKey(assetList.list[i].path))
                    continue;
                
                src = AssetManagerSetting.EditorRoot.Stream + AssetManagerSetting.GetPlatformPath(assetList.list[i].path);
                if (File.Exists(src))
                {
                    File.Delete(src);
                }
            }


            AssetDatabase.Refresh();
        }

    }
}
