using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Games;


namespace com.ihaiu
{
    public partial class VersionManager 
    {
       
        public void CheckFirstSync()
        {
            appGameConstConfig = JsonUtility.FromJson<GameConstConfig>(AssetManagerSetting.SyncLoadFile.GameConst_Streaming());

            #if UNITY_EDITOR
            appGameConstConfig.Set();

            if (appGameConstConfig.DevelopMode)
            {
                return;
            }

            if(!AssetManagerSetting.TestVersionMode)
            {
                appGameConstConfig.Set();
                return;
            }
            #endif

            appVer.Parse(appGameConstConfig.Version);
            VersionLocalInfo.Install.streamVersion.Parse(appGameConstConfig.Version);
            if (VersionLocalInfo.Install.IsResetAppRes)
            {
                appGameConstConfig.Set();
                curVer.Copy(appVer);
                InitDataSync();
            }
            else
            {
                ReadGameConst_Persistent_Sync();

                curGameConstConfig.Set();
                curVer.Parse(curGameConstConfig.Version);
            }

            if (VersionLocalInfo.Install.IsNewApp)
            {
                VersionLocalInfo.Install.localVersion.Parse(appGameConstConfig.Version);
                VersionLocalInfo.Install.Save();
            }
        }

       

        void InitDataSync()
        {
            Caching.CleanCache();
            InitAssetList(AssetManagerSetting.SyncLoadFile.AssetListApp_Streaming());

            List<string> list = new List<string>();
            list.Add(AssetManagerSetting.FileName.GameConst);



            string infile = "";
            string outfile = "";
            int count = list.Count;
            for (int i = 0; i < count; i ++)
            {
                string file = list[i];
               

                file = string.Format(file, Platform.PlatformDirectory);
                infile = AssetManagerSetting.RootPathStreaming + file; 
                outfile = AssetManagerSetting.RootPathPersistent + file;

                AssetManagerSetting.persistentAssetFileList.Add(file, "");

                PathUtil.CheckPath(outfile);

                if (Application.platform == RuntimePlatform.Android)
                {
                    byte[] bytes = AssetLoadSdk.LoadBytes(file);
                    if(bytes != null)
                    {
                        File.WriteAllBytes(outfile, bytes);
                    }
                }
                else
                {
                    File.Copy(infile, outfile, true);
                }

            }




            AssetManagerSetting.persistentAssetFileList.Save();

        }





        void ReadGameConst_Persistent_Sync()
        {   
            string path = AssetManagerSetting.PersistentFilePath.GameConst;
            if (File.Exists(path))
            {
                curGameConstConfig = JsonUtility.FromJson<GameConstConfig>(File.ReadAllText(path));
            }
        }

    }
}