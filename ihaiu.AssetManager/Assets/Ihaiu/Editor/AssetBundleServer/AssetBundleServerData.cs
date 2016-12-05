using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace com.ihaiu
{
    public class AssetBundleServerData 
    {

        public static void CopyUpdateAsset(string serverRoot)
        {
            PathUtil.CheckPath(serverRoot, false);
            string updateAssetListPath = null;
            if (File.Exists(AssetManagerSetting.EditorUpdateAssetListPath))
            {
                updateAssetListPath = AssetManagerSetting.EditorUpdateAssetListPath;
            }
            else if(File.Exists(AssetManagerSetting.EditorFileCsvForStreaming))
            {
                updateAssetListPath = AssetManagerSetting.EditorFileCsvForStreaming;
            }

            if (!string.IsNullOrEmpty(updateAssetListPath))
            {
                AssetFileList assetFileList = AssetFileList.Read(updateAssetListPath);
                assetFileList.Add(new AssetFile("{0}/" + AssetManagerSetting.UpdateAssetListName, ""));

                int count = assetFileList.list.Count;
                for(int i = 0; i < count; i ++)
                {
                    AssetFile item = assetFileList.list[i];
                    string path = AssetManagerSetting.GetPlatformPath(item.path);
                    string fromPath = AssetManagerSetting.EditorRootStream + "/"+ path;
                    string toPath = serverRoot + "/" + path;

                    PathUtil.CheckPath(toPath);
                    File.Copy(fromPath, toPath, true);

                    if(i % 10 == 0) UnityEditor.EditorUtility.DisplayProgressBar("拷贝目录", path, 1f *i / count);
                }
            }
            else
            {
                CopyAlleAsset(serverRoot);
            }

            UnityEditor.EditorUtility.ClearProgressBar();

        }

        public static void CopyAlleAsset(string serverRoot)
        {
            PathUtil.CheckPath(serverRoot, false);
            PathUtil.CopyDirectory(AssetManagerSetting.EditorRootStream, serverRoot, new List<string>(new string[]{".meta", ".manifest"}));
        }
    }
}