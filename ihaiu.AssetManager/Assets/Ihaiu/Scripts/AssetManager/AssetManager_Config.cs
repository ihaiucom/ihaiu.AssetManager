using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {
        public AssetBundle configAssetBundle;

        public void LoadConfig(string filename, Action<string, object, object[]> callback, params object[] args)
        {
            if(callback == null) return;

            TextAsset textAsset = LoadConfig(filename);
            if(textAsset != null)
            {
                callback(filename, textAsset.text, args);
            }
            else
            {
                callback(filename, null, args);
            }

        }

        public TextAsset LoadConfig(string filename)
        {
            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateConfig)
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(AssetManagerSetting.EditorGetConfigPath(filename));

            }
            else
            #endif
            {
                return (TextAsset)configAssetBundle.LoadAsset(AssetManagerSetting.GetConfigAssetName(filename));
            }
        }

    }
}