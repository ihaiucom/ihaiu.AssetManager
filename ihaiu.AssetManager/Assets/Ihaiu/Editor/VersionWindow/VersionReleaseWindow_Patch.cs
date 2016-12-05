using UnityEngine;
using System.Collections;
using UnityEditor;
using Games;


namespace com.ihaiu
{



    public partial class VersionReleaseWindow
    {

        int _compareIndex = -1;
        public int compareIndex
        {
            get
            {
                if (_compareIndex == -1)
                {
                    _compareIndex = VersionList.appVersionStrArr.Length - 1;
                    if (_compareIndex < 0)
                        _compareIndex = 0;
                }
                return _compareIndex;
            }

            set
            {
                _compareIndex = value;
            }
        }

        Version _compareVersion;
        public Version compareVersion
        {
            get
            {
                if (_compareVersion == null)
                {
                    _compareVersion = new Version();
                    _patchVersion.Copy(VersionList.lastAppVersion);
                }


                return _compareVersion;
            }
        }

        Version _patchVersion;
        public Version patchVersion
        {
            get
            {
                if (_patchVersion == null)
                {
                    _patchVersion = new Version();
                    _patchVersion.Copy(VersionList.GetLastAppRevised(compareVersion));
                    _patchVersion.verType = VersionType.Patch;
                }


                return _patchVersion;
            }
        }


        /** 补丁 */
        void OnGUI_Patch()
        {
            Version version = patchVersion;

            if (version.revised <= 0)
                version.revised = 1;

            GUILayout.BeginHorizontal(HGUILayout.boxMPStyle, GUILayout.Height(50));
            EditorGUILayout.LabelField("参照App版本", HGUILayout.labelCenterStyle, GUILayout.Width(150), GUILayout.Height(25));
            int preCompareIndex = compareIndex;
            compareIndex = EditorGUILayout.Popup(compareIndex, VersionList.appVersionStrArr);
            if (preCompareIndex != compareIndex && compareIndex != -1)
            {
                Version v = VersionList.appVersionList[compareIndex];

                compareVersion.Copy(v);
                version.Copy(VersionList.GetLastAppRevised(compareVersion));
                version.verType = VersionType.Patch;
                version.revised++;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            HGUILayout.Version("补丁版本", version, VersionType.Patch);
            GUILayout.BeginVertical();

            if (GUILayout.Button("最后一次",GUILayout.Width(100), GUILayout.Height(30)))
            {
                VersionList.Read(true);

                compareVersion.Copy(VersionList.lastAppVersion);

                version.Copy(VersionList.GetLastAppRevised(compareVersion));
                version.verType = VersionType.Patch;
                compareIndex = VersionList.appVersionStrList.IndexOf(compareVersion.ToString());

            }

            if (GUILayout.Button("自动",GUILayout.Width(100), GUILayout.Height(30)))
            {
                VersionList.Read(true);

                compareVersion.Copy(VersionList.lastAppVersion);
                version.Copy(VersionList.GetLastAppRevised(compareVersion));
                version.verType = VersionType.Patch;
                version.revised++;
                compareIndex = VersionList.appVersionStrList.IndexOf(compareVersion.ToString());
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);


            HGUILayout.BeginCenterHorizontal();
            if (GUILayout.Button("生成版本信息", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
            {

                version.SetNowDatetime();

                bool isRefresh = false;
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
                    LoadAssetListCsv.Generator();
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.GeneratorStreamingAssetsFilesCSV))
                {
                    FilesCsvForStreamingAssets.Generator();
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
                    FilesCsvForStreamingAssets.CopyStreamFilesCsvToVersion(version);
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.GeneratorUpdateAssetList))
                {
                    FilesCsvForStreamingAssets.GeneratorUpdateList(compareVersion);
                }

            }
            HGUILayout.EndCenterHorizontal();
        }

    }
}
