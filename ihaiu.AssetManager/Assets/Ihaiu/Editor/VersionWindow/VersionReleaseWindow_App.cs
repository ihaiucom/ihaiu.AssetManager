using UnityEngine;
using System.Collections;
using UnityEditor;
using Games;
using Games;
using System.IO;


namespace com.ihaiu
{
    public partial class VersionReleaseWindow
    {
        Version _appVersion;
        public Version appVersion
        {
            get
            {
                if (_appVersion == null)
                {
                    _appVersion = new Version();

                    _appVersion.verType = VersionType.App;
                }


                return _appVersion;
            }
        }


        /** App */
        void OnGUI_App()
        {




            Version version = appVersion;


            GUILayout.BeginHorizontal();
            HGUILayout.Version("App版本", version, VersionType.App);
            GUILayout.BeginVertical();
            if (GUILayout.Button("最后一次",GUILayout.Width(100), GUILayout.Height(30)))
            {
                VersionList.Read(true);

                version.Copy(VersionList.lastAppVersion);
//                version.revised = 0;
                version.verType = VersionType.App;
            }

            if (GUILayout.Button("自动",GUILayout.Width(100), GUILayout.Height(30)))
            {
                VersionList.Read(true);

                version.Copy(VersionList.lastAppVersion);
                //                version.minor++;
                //                version.revised = 0;
                version.revised++;
                version.verType = VersionType.App;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);




            HGUILayout.BeginCenterHorizontal();
            if (GUILayout.Button("生成版本信息", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
            {
                version.SetNowDatetime();


                bool isRefresh = false;

                if (currentDvancedSettingData.GetValue(DvancedSettingType.ClearWorkspacePlatformDirctory))
                {
                    ClearWorkspacePlatformDirctory(runtimePlatform);
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.ClearAllPlatformDirctory))
                {
                    PathUtil.ClearAllPlatformDirctory();
                    isRefresh = true;
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.ClearOtherPlatformDirctory))
                {
                    PathUtil.ClearOtherPlatformDirctory(runtimePlatform);
                    isRefresh = true;
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.ClearTestData))
                {
                    PathUtil.ClearTestData();
                    isRefresh = true;
                }

                if (isRefresh)
                {
                    AssetDatabase.Refresh();
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.Clear_AssetBundleName))
                {
                    AssetBundleEditor.ClearAssetBundleNames();
                    AssetDatabase.RemoveUnusedAssetBundleNames();
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.Set_AssetBundleName))
                {
                    AssetBundleEditor.SetNames();
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.AB_AssetBundle))
                {
                    AssetBundleEditor.BuildAssetBundles();
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.AB_luacode))
                {
                    AB.Lua();
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.AB_config))
                {
                    AB.Config();
                }


              

                if (currentDvancedSettingData.GetValue(DvancedSettingType.GameConstConfig))
                {
                    GameConstConfig config = GameConstConfig.Load();
                    config.DevelopMode  = false;
                    config.Version      = version.ToConfig();
                    config.Save();
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.GeneratorLoadAssetListCsv))
                {
                    AssetListCsvLoadMap.Generator(false);
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.GeneratorStreamingAssetsFilesCSV))
                {
                    AssetListCsvFile.Generator();
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.PlayerSettings))
                {
                    SetPlayerSettings(runtimePlatform);
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.PlayerSettingsVersion))
                {
                    SetPlayerSettingsVersion(appVersion);
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.GenerateVersionInfo))
                {
                    AssetListCsvFile.CopyStreamFilesCsvToVersion(version);
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.GeneratorUpdateAssetList))
                {
                    AssetListCsvFile.GeneratorUpdateList(null);
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.GenerateResZip))
                {
                    ResZipEditor.Install.Generator();
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.CopyWorkspaceStreamToStreamingAssets_UnResZip))
                {
                    ResZipEditor.Install.CopyToStreaming_UnZip();
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.CopyWorkspaceStreamToStreamingAssets_All))
                {
                    ResZipEditor.Install.CopyToStreaming_All();
                }
            }
            HGUILayout.EndCenterHorizontal();



        }
    }
}
