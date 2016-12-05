using UnityEngine;
using System.Collections;
using UnityEditor;
using Games;


namespace com.ihaiu
{
    public partial class VersionReleaseWindow
    {

        /** 开发 */
        void OnGUI_Develop()
        {
            HGUILayout.BeginCenterHorizontal();
            if (GUILayout.Button("生成版本信息", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
            {
                if (currentDvancedSettingData.GetValue(DvancedSettingType.GameConstConfig))
                {
                    GameConstConfig config = GameConstConfig.Load();
                    config.DevelopMode      = true;
                    config.TestVersionMode  = false;
                    config.Save();
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.Clear_AssetBundleName))
                {
                    AssetBundleEditor.ClearAssetBundleNames();
                    AssetDatabase.RemoveUnusedAssetBundleNames();
                }


                if (currentDvancedSettingData.GetValue(DvancedSettingType.Set_AssetBundleName))
                {
                    AssetBundleEditor.SetNames_Develop();
                }

                if (currentDvancedSettingData.GetValue(DvancedSettingType.GeneratorLoadAssetListCsv))
                {
                    LoadAssetListCsv.Generator();
                }

            }
            HGUILayout.EndCenterHorizontal();

        }


    }
}
