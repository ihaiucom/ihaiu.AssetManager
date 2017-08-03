using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Games;
using System;
using System.IO;


namespace com.ihaiu
{
    public partial class SeeAssetBundleInfoWindow : EditorWindow
    {
        public static SeeAssetBundleInfoWindow window;
        public static void Open () 
        {
            window = EditorWindow.GetWindow <SeeAssetBundleInfoWindow>("查看AssetBundle");
            window.Show();
        }

        public int assetBundleIndex = 0;
        public string assetBundleName;
        public string manifestPath;



        Vector2 scrollPos;
        string assetNameStr = "";
        int    assetNameHeight = 200;   

        string dependencieStr = "";
        int    dependencieHeight = 200;  

        string[] allAssetBundleNames;
        void OnGUI ()
        {

            EditorGUILayout.BeginVertical();
            GUILayout.Space(20);

            if (allAssetBundleNames == null || allAssetBundleNames.Length == 0)
            {
                allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            }

            if (string.IsNullOrEmpty(manifestPath))
            {
                manifestPath = AssetManagerSetting.EditorRoot.StreamPlatform + Platform.PlatformDirectoryName;
             
            }


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ManifestAssetBundle:", GUILayout.Width(100));
            manifestPath = EditorGUILayout.TextField(manifestPath);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AssetBuild:", GUILayout.Width(100));
            assetBundleIndex = EditorGUILayout.Popup(assetBundleIndex, allAssetBundleNames);
            assetBundleName = allAssetBundleNames.Length > 0 ? allAssetBundleNames[assetBundleIndex] : "";
            EditorGUILayout.EndHorizontal();


            if (!string.IsNullOrEmpty(assetBundleName))
            {
                if (GUILayout.Button("查看资源"))
                {
                    string assetBundlePath = AssetManagerSetting.EditorRoot.StreamPlatform + assetBundleName;
                    AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

                    assetNameStr = "";
                    dependencieStr = "";
                    dependencieHeight = assetNameHeight = 200;
                    if (assetBundle != null)
                    {
                        string[] assetNames = assetBundle.GetAllAssetNames();
                        assetNameHeight = Mathf.Max(200, assetNames.Length * 20 + 40);

                        assetNameStr = "";
                        for (int i = 0; i < assetNames.Length; i++)
                        {
                            assetNameStr += i + "  " + assetNames[i] + "\n";
                        }

                        AssetBundle manifestAssetBundle = AssetBundle.LoadFromFile(manifestPath);
                        if (manifestAssetBundle != null)
                        {
                            AssetBundleManifest manifest = manifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                            if (manifest != null)
                            {
                                string[] dependencies = manifest.GetAllDependencies(assetBundleName);
                                for (int i = 0; i < dependencies.Length; i++)
                                {
                                    dependencieStr += i + "  " + dependencies[i] + "\n";
                                }

                                dependencieHeight = Mathf.Max(200, dependencies.Length * 20 + 40);
                            }
                            else
                            {
                                this.ShowNotification(new GUIContent("manifest=null"));
                            }

                            manifestAssetBundle.Unload(true);
                        }
                        else
                        {
                            this.ShowNotification(new GUIContent("manifestAssetBundle=null"));
                        }

                        assetBundle.Unload(true);
                    }
                    else
                    {
                        this.ShowNotification(new GUIContent("assetBundle=null"));
                    }

                }
            }







            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.Space(20);
            EditorGUILayout.LabelField("AssetNames:");
            assetNameStr = GUILayout.TextArea(assetNameStr, GUILayout.Height(assetNameHeight));

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Dependencies:");
            dependencieStr = GUILayout.TextArea(dependencieStr, GUILayout.Height(dependencieHeight));

            EditorGUILayout.EndScrollView();




            EditorGUILayout.EndVertical();
        }
    }

}
