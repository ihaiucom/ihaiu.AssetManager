using UnityEngine;
using System.Collections;
using UnityEditor;


namespace com.ihaiu
{
    public class AssetBundleServerGUI 
    {
        public const string ROOT_WWW                = "www";
        public const string ROOT_StreamingAssets    = "Assets/StreamingAssets";
        public int index = -1;
        public string[] serverRoots = new string[]{ROOT_WWW, ROOT_StreamingAssets};
        public string selectRoot;
        public void OnGUI()
        {
            if (index == -1)
            {
                index = LaunchAssetBundleServer.ServerRootPath == AssetManagerSetting.EditorAssetBundleServerRoot_WWW ? 0 : 1;

                if (index == 0)
                {
                    selectRoot = AssetManagerSetting.EditorAssetBundleServerRoot_WWW;
                }
                else
                {
                    selectRoot = AssetManagerSetting.EditorAssetBundleServerRoot_StreamingAssets;
                }
            }

            GUILayout.BeginHorizontal(HGUILayout.boxMPStyle, GUILayout.Height(50));
            EditorGUILayout.LabelField("选择服务器目录", HGUILayout.labelCenterStyle, GUILayout.Width(150), GUILayout.Height(25));
            int preSelectIndex = index;
            index = EditorGUILayout.Popup(index, serverRoots);
            if (preSelectIndex != index)
            {
                preSelectIndex = index;
                if (index == 0)
                {
                    selectRoot = AssetManagerSetting.EditorAssetBundleServerRoot_WWW;
                }
                else
                {
                    selectRoot = AssetManagerSetting.EditorAssetBundleServerRoot_StreamingAssets;
                }
            }
            GUILayout.EndHorizontal();


            if (index == 0)
            {
                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("清除该目录数据", GUILayout.MinHeight(30), GUILayout.MaxWidth(200)))
                {
                    PathUtil.ClearDirectory(selectRoot + "/StreamingAssets");
                }

                GUILayout.Space(20);

                if (GUILayout.Button("复制更新数据到该目录", GUILayout.MinHeight(30), GUILayout.MaxWidth(200)))
                {
                    AssetBundleServerData.CopyUpdateAsset(selectRoot + "/StreamingAssets");
                }


                GUILayout.Space(20);

                if (GUILayout.Button("复制所有数据到该目录", GUILayout.MinHeight(30), GUILayout.MaxWidth(200)))
                {
                    AssetBundleServerData.CopyAlleAsset(selectRoot + "/StreamingAssets");
                }

                HGUILayout.EndCenterHorizontal();
            }


            if (LaunchAssetBundleServer.IsRunning())
            {

                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));
                EditorGUILayout.LabelField("Host: " , LaunchAssetBundleServer.Host);
                EditorGUILayout.LabelField("RuningRoot: " , LaunchAssetBundleServer.ServerRootPath);
                EditorGUILayout.LabelField("SelectRoot: " , selectRoot);
                GUILayout.EndVertical();

                

                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("关闭服务器", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
                {
                    LaunchAssetBundleServer.KillRunningAssetBundleServer();
                    LaunchCheckServer.KillRunningServer();
                }

                GUILayout.Space(20);

                if (GUILayout.Button("重启服务器", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
                {
                    
                    LaunchAssetBundleServer.KillRunningAssetBundleServer();

                    LaunchAssetBundleServer.ServerRootPath = selectRoot;
                    LaunchAssetBundleServer.Run();
                }
                HGUILayout.EndCenterHorizontal();
            }
            else
            {
                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));
                EditorGUILayout.LabelField("SelectRoot: " , selectRoot);
                GUILayout.EndVertical();

                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("启动服务器", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
                {
                    LaunchAssetBundleServer.ServerRootPath = selectRoot;
                    LaunchAssetBundleServer.Run();
                    LaunchCheckServer.Run();
                }
                HGUILayout.EndCenterHorizontal();

            }


            GUILayout.Space(20);

        }
    }
}
