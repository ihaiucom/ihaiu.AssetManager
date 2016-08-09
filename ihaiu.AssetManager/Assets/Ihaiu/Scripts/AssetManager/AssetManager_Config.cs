using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Ihaiu.Assets
{
    public partial class AssetManager 
    {
        public AssetBundle configAssetBundle;

        public void LoadConfig(string filename, Action<string, object, object[]> callback, params object[] args)
        {
            if(callback == null) return;

            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateConfig)
            {
                TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(AssetManagerSetting.EditorGetConfigPath(filename));
                if(textAsset != null)
                {
                    callback(filename, textAsset.text, args);
                }
                else
                {
                    callback(filename, null, args);
                }
            }
            else
            #endif
            {
                TextAsset textAsset = (TextAsset)configAssetBundle.LoadAsset(AssetManagerSetting.GetConfigAssetName(filename));
                if(textAsset != null)
                {
                    callback(filename, textAsset.text, args);
                }
                else
                {
                    callback(filename, null, args);
                }
            }
        }

    }
}