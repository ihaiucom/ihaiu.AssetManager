using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Games;
using System;


namespace com.ihaiu
{
    public partial class VersionReleaseWindow : EditorWindow
    {
        public static VersionReleaseWindow window;
//        [MenuItem ("资源管理/版本设置面板", false, 900)]
        public static void Open () 
        {
            window = EditorWindow.GetWindow <VersionReleaseWindow>("版本设置");
            window.minSize = new Vector2(800, 500);
            window.Show();
        }

        public enum TabType
        {
            [HelpAttribute("开发")]
            Develop,

            [HelpAttribute("App")]
            App,

            [HelpAttribute("补丁")]
            Patch,
        }

        HGUILayout.TabGroupData<TabType> _tabGroupData;
        HGUILayout.TabGroupData<TabType> tabGroupData
        {
            get
            {
                if (_tabGroupData == null)
                {
                    _tabGroupData = new HGUILayout.TabGroupData<TabType>();
                    _tabGroupData.AddTab("开发", TabType.Develop);
                    _tabGroupData.AddTab("App", TabType.App);
                    _tabGroupData.AddTab("补丁", TabType.Patch);
                }
                return _tabGroupData;
            }
        }




        public RuntimePlatform runtimePlatform
        {
            get
            {
                return Platform.GetRuntimePlatform(EditorUserBuildSettings.activeBuildTarget);
            }
        }



        #region 高级设置
        public enum DvancedSettingType
        {
            [HelpAttribute("生成LoadAssetList.csv")]
            GeneratorLoadAssetListCsv,
            [HelpAttribute("生成StreamingAssets files.csv")]
            GeneratorStreamingAssetsFilesCSV,
            [HelpAttribute("修改game_const.json")]
            GameConstConfig,
            [HelpAttribute("生成版本信息文件")]
            GenerateVersionInfo,
            [HelpAttribute("生成更新列表")]
            GeneratorUpdateAssetList,


            [HelpAttribute("打包luacode.assetbundle")]
            AB_luacode,
            [HelpAttribute("打包config.assetbundle")]
            AB_config,


            [HelpAttribute("清除AssetBundleName")]
            Clear_AssetBundleName,

            [HelpAttribute("设置AssetBundleName")]
            Set_AssetBundleName,

            [HelpAttribute("打包AssetBundle")]
            AB_AssetBundle,




            [HelpAttribute("清除所有平台数据")]
            ClearAllPlatformDirctory,
            [HelpAttribute("清理其他平台数据")]
            ClearOtherPlatformDirctory,
            [HelpAttribute("清理测试数据")]
            ClearTestData,


            [HelpAttribute("PlayerSettings")]
            PlayerSettings,

            [HelpAttribute("PlayerSettings Version")]
            PlayerSettingsVersion,
        }

        public string[] davancedSetingNames = new string[]{ 
            "生成LoadAssetList.csv",
            "生成StreamingAssets files.csv",
            "修改game_const.json",
            "生成版本信息文件",
            "生成更新列表",

            "打包luacode-assetbundle",
            "打包config-assetbundle",

            "清除AssetBundleName",
            "设置AssetBundleName",
            "打包AssetBundle",


            "清除所有平台数据",
            "清理其他平台数据",
            "清理测试数据",

            "PlayerSettings",
            "PlayerSettingsVersion",

            "",
            "",
            "",
        };

        public class DvancedSettingItem
        {
            public DvancedSettingType type;
            public string name;
            public bool value;

            public DvancedSettingItem(DvancedSettingType type, string name, bool value)
            {
                this.type = type;
                this.name = name;
                this.value = value;
            }
        }

        public class DvancedSettingData
        {
            public bool foldout = true;
            public TabType tabType;
            public Dictionary<DvancedSettingType, DvancedSettingItem> dict = new Dictionary<DvancedSettingType, DvancedSettingItem>();
            public List<DvancedSettingItem> list = new List<DvancedSettingItem>();

            public DvancedSettingData(TabType tabType)
            {
                this.tabType = tabType;
            }

            public DvancedSettingData Add(DvancedSettingItem item)
            {
                if (!dict.ContainsKey(item.type))
                {
                    dict.Add(item.type, item);
                    list.Add(item);
                }

                return this;
            }

            public bool GetValue(DvancedSettingType itemType)
            {
                if (dict.ContainsKey(itemType))
                {
                    return dict[itemType].value;
                }
                return false;
            }
        }

        public DvancedSettingItem CreateDvancedSettingItem(DvancedSettingType type, bool value = true)
        {
            return new DvancedSettingItem(type, davancedSetingNames[(int)type], value);
        }

        private Dictionary<TabType, DvancedSettingData> _dvancedSettingDataDict;
        public Dictionary<TabType, DvancedSettingData> dvancedSettingDataDict
        {
            get
            {
                if (_dvancedSettingDataDict == null)
                {
                    _dvancedSettingDataDict = new Dictionary<TabType, DvancedSettingData>();

                    DvancedSettingData data = new DvancedSettingData(TabType.Develop);
                    _dvancedSettingDataDict.Add(data.tabType, data);
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GameConstConfig));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GeneratorLoadAssetListCsv));


                    data = new DvancedSettingData(TabType.App);
                    _dvancedSettingDataDict.Add(data.tabType, data);
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.ClearAllPlatformDirctory, false));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.ClearOtherPlatformDirctory, true));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.ClearTestData, true));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.Clear_AssetBundleName));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.Set_AssetBundleName));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.AB_AssetBundle));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.AB_luacode));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.AB_config));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GameConstConfig));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GeneratorLoadAssetListCsv));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GeneratorStreamingAssetsFilesCSV));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.PlayerSettings));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.PlayerSettingsVersion));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GenerateVersionInfo));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GeneratorUpdateAssetList, false));


                    data = new DvancedSettingData(TabType.Patch);
                    _dvancedSettingDataDict.Add(data.tabType, data);
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.ClearAllPlatformDirctory, false));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.ClearOtherPlatformDirctory, false));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.ClearTestData, false));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.Clear_AssetBundleName));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.Set_AssetBundleName));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.AB_AssetBundle));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.AB_luacode));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.AB_config));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GameConstConfig));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GeneratorLoadAssetListCsv));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GeneratorStreamingAssetsFilesCSV));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GenerateVersionInfo));
                    data.Add(CreateDvancedSettingItem(DvancedSettingType.GeneratorUpdateAssetList));
                }
                return _dvancedSettingDataDict;
            }
        }

        public DvancedSettingData currentDvancedSettingData;
        #endregion



        Vector2 scrollPos;
        void OnGUI ()
        {
            VersionList.Read();



            TabType tabType = HGUILayout.TabGroup<TabType>(tabGroupData);

           
           


            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);


            GUILayout.Space(20);


            currentDvancedSettingData = dvancedSettingDataDict[tabType];
            switch(tabType)
            {
                case TabType.Develop:
                    OnGUI_Develop();
                    break;

                case TabType.App:
                    OnGUI_App();
                    break;

                case TabType.Patch:
                    OnGUI_Patch();
                    break;
            }



            currentDvancedSettingData.foldout = EditorGUILayout.Foldout(currentDvancedSettingData.foldout, "执行选项");

            if (currentDvancedSettingData.foldout)
            {
                GUILayout.BeginVertical(HGUILayout.boxMPStyle);
                for(int i = 0; i < currentDvancedSettingData.list.Count; i ++)
                {
                    DvancedSettingItem item = currentDvancedSettingData.list[i];



                    GUILayout.BeginHorizontal();
                    item.value = EditorGUILayout.ToggleLeft(item.name, item.value, GUILayout.Width(250));



                    if(GUILayout.Button(item.name, GUILayout.Width(250)))
                    {
                        switch(item.type)
                        {
                            case DvancedSettingType.ClearAllPlatformDirctory:
                                PathUtil.ClearAllPlatformDirctory();
                                AssetDatabase.Refresh();
                                break;


                            case DvancedSettingType.ClearOtherPlatformDirctory:
                                PathUtil.ClearOtherPlatformDirctory(runtimePlatform);
                                AssetDatabase.Refresh();
                                break;


                            case DvancedSettingType.ClearTestData:
                                PathUtil.ClearTestData();
                                AssetDatabase.Refresh();
                                break;


                            case DvancedSettingType.AB_luacode:
                                AB.Lua();
                                break;

                            case DvancedSettingType.AB_config:
                                AB.Config();
                                break;




                            case DvancedSettingType.Clear_AssetBundleName:
                                AssetBundleEditor.ClearAssetBundleNames();
                                AssetDatabase.RemoveUnusedAssetBundleNames();
                                break;

                            case DvancedSettingType.Set_AssetBundleName:
                                switch (tabType)
                                {
                                    case TabType.Develop:
                                        AssetBundleEditor.SetNames_Develop();
                                        break;
                                    default:
                                        AssetBundleEditor.SetNames();
                                        break;
                                }

                                break;


                            case DvancedSettingType.AB_AssetBundle:
                                AssetBundleEditor.BuildAssetBundles();
                                break;




                            case DvancedSettingType.GameConstConfig:

                                GameConstConfig config = GameConstConfig.Load();
                                switch (tabType)
                                {
                                    case TabType.Develop:
                                        config.DevelopMode      = true;
                                        config.TestVersionMode  = false;
                                        break;

                                    case TabType.App:
                                        config.DevelopMode  = false;
                                        config.Version      = appVersion.ToConfig();
                                        break;

                                    case TabType.Patch:
                                        config.DevelopMode  = false;
                                        config.Version      = patchVersion.ToConfig();
                                        break;
                                }

                                config.Save();
                                break;


                            case DvancedSettingType.GeneratorStreamingAssetsFilesCSV:
                                FilesCsvForStreamingAssets.Generator();
                                break;


                            case DvancedSettingType.GeneratorLoadAssetListCsv:
                                LoadAssetListCsv.Generator();
                                break;


                            case DvancedSettingType.PlayerSettings:
                                SetPlayerSettings(runtimePlatform);
                                break;

                            case DvancedSettingType.PlayerSettingsVersion:
                                switch (tabType)
                                {
                                    case TabType.App:
                                        SetPlayerSettingsVersion(appVersion);
                                        break;

                                    case TabType.Patch:
                                        SetPlayerSettingsVersion(patchVersion);
                                        break;
                                }
                                break;


                            case DvancedSettingType.GenerateVersionInfo:
                                switch (tabType)
                                {
                                    case TabType.App:
                                        appVersion.SetNowDatetime();
                                        FilesCsvForStreamingAssets.CopyStreamFilesCsvToVersion(appVersion);
                                        break;

                                    case TabType.Patch:
                                        patchVersion.SetNowDatetime();
                                        FilesCsvForStreamingAssets.CopyStreamFilesCsvToVersion(patchVersion);
                                        break;
                                }
                                break;

                            case DvancedSettingType.GeneratorUpdateAssetList:
                                switch (tabType)
                                {
                                    case TabType.App:
                                        FilesCsvForStreamingAssets.GeneratorUpdateList(null);
                                        break;
                                    case TabType.Patch:
                                        FilesCsvForStreamingAssets.GeneratorUpdateList(compareVersion);
                                        break;

                                }
                                break;

                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);
                }
                GUILayout.EndVertical();

            }



            switch(tabType)
            {
                case TabType.App:
                case TabType.Patch:
                    OnGUI_AssetBundleServer();
                    OnGUI_TestVersionModel();
                    break;
            }

            EditorGUILayout.EndScrollView();

        }


        void SetPlayerSettings(RuntimePlatform platform)
        {
            PlayerSettings.companyName = "mb";
            PlayerSettings.productName = "空城计-全民竞技";
            PlayerSettings.showUnitySplashScreen = false;


            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.useAnimatedAutorotation = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            if(runtimePlatform != RuntimePlatform.Android)
            {
                PlayerSettings.bundleIdentifier = "com.mb.crsg";


//                Texture2D[] icons = new Texture2D[1];
//                icons[0] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Game/PlayerSettings/icon.png");
//                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, icons);
//                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, icons);
//
//                Texture2D splash = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Game/PlayerSettings/splash.jpg");
//                PlayerSettings.virtualRealitySplashScreen = splash;
            }


            switch (platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    PlayerSettings.aotOptions = "nrgctx-trampolines=8096,nimt-trampolines=8096,ntrampolines=4048";

                    break;
            }

        }


        void SetPlayerSettingsVersion(Version version)
        {
            PlayerSettings.bundleVersion = version.ToConfig();
            if (runtimePlatform == RuntimePlatform.Android)
            {
                PlayerSettings.Android.bundleVersionCode = version.ToConfig().Replace(".", "").ToInt32();
            }
            else if(runtimePlatform == RuntimePlatform.IPhonePlayer)
            {
                PlayerSettings.iOS.buildNumber = version.ToConfig().Replace(".", "");
            }
        }

        bool assetBundleServer_foldout = true;
        AssetBundleServerGUI assetBundleServerGUI = new AssetBundleServerGUI();
        void OnGUI_AssetBundleServer()
        {
            assetBundleServer_foldout = EditorGUILayout.Foldout(assetBundleServer_foldout, "Local AssetBundle Server");

            if (assetBundleServer_foldout)
            {
                assetBundleServerGUI.OnGUI();
            }
        }


        bool testVersionModel_foldout = true;
        bool testVersionModel_value = false;
        bool testVersionModel_webIsDevelop = false;
        void OnGUI_TestVersionModel()
        {
            testVersionModel_foldout = EditorGUILayout.Foldout(testVersionModel_foldout, "测试模拟版本模式");

            if (testVersionModel_foldout)
            {
                if (GameConstConfig.last == null)
                    GameConstConfig.Load();


                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));
                testVersionModel_value = GameConstConfig.last.TestVersionMode;
                GameConstConfig.last.TestVersionMode = EditorGUILayout.ToggleLeft("测试模拟版本模式", GameConstConfig.last.TestVersionMode);
                if (testVersionModel_value != GameConstConfig.last.TestVersionMode)
                {
                    testVersionModel_value = GameConstConfig.last.TestVersionMode;
                    if (testVersionModel_value)
                    {
                        GameConstConfig.last.DevelopMode = false;
                    }

                    GameConstConfig.last.Save();
                }


                testVersionModel_webIsDevelop = EditorGUILayout.ToggleLeft("WebUrl Is Develop", GameConst.WebUrlIsDevelop);
                if (testVersionModel_webIsDevelop != GameConst.WebUrlIsDevelop)
                {
                    GameConst.WebUrlIsDevelop = testVersionModel_webIsDevelop;
                }

                GUILayout.EndVertical();
            }
        }


    }
}
