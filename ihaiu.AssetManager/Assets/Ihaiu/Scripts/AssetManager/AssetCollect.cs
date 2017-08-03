using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace com.ihaiu
{
    /// <summary>
    /// 资源加载情况收集
    /// </summary>
    [System.Serializable]
    public class AssetCollect 
    {
        #region internal
        public Dictionary<string, AssetCollectInternalItem> internalDict = new Dictionary<string, AssetCollectInternalItem>();
        [SerializeField]
        public List<AssetCollectInternalItem>               internalList = new List<AssetCollectInternalItem>();

        public void OnLoadInternal(string assetBundleName)
        {
            if (!AssetManagerSetting.IsCollect)
                return;
            
            if (internalDict.ContainsKey(assetBundleName))
            {
                internalDict[assetBundleName].loadNum++;
            }
            else
            {
                AddInternal(assetBundleName);
            }
        }

        public void AddInternal(string assetBundleName)
        {
            AssetCollectInternalItem item = new AssetCollectInternalItem();
            item.url = assetBundleName;
            item.loadNum = 1;
            internalDict.Add(item.url, item);
            internalList.Add(item);
        }

        public void OnUnloadInteral(string assetBundleName)
        {
            if (!AssetManagerSetting.IsCollect)
                return;

            
            if (internalDict.ContainsKey(assetBundleName))
            {
                internalDict[assetBundleName].unloadNum++;
            }
            else
            {
                AssetCollectInternalItem item = new AssetCollectInternalItem();
                item.url = assetBundleName;
                item.unloadNum = 1;
                internalDict.Add(item.url, item);
                internalList.Add(item);
            }
        }

        public void InternalListToDict()
        {
            for(int i = 0; i < internalList.Count; i ++)
            {
                if (!internalDict.ContainsKey(internalList[i].url))
                {
                    internalDict.Add(internalList[i].url, internalList[i]);
                }
            }
        }
        #endregion



        public void Save(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                #if UNITY_EDITOR
                path = AssetManagerSetting.EditorRoot.WorkspaceAssetCollect + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")  + ".json";
                #else
                path = AssetManagerSetting.RootPathPersistent + "AssetCollect.json";
                #endif
            }

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(path, json);
        }


        public static AssetCollect Load(string path = null)
        {

            if (string.IsNullOrEmpty(path))
            {
                path = AssetManagerSetting.RootPathPersistent + "AssetCollect.json";
            }

            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<AssetCollect>(json);
        }



        public void SaveGuide()
        {
            #if UNITY_EDITOR
            AssetCollect collect = new AssetCollect();
            collect.internalList = this.internalList;
            collect.internalDict = this.internalDict;

//            // role head
//            for(int id = 80001; id <= 80040; id ++)
//            {
//                AvatarConfig config = Goo.avatar.GetConfig(id);
//                if(config != null)
//                {
//                    if(string.IsNullOrEmpty(config.icon))
//                    {
//                        string asset = config.icon.Trim().ToLower() + AssetManagerSetting.AssetbundleExt;
//                        if(!collect.internalDict.ContainsKey(asset))
//                        {
//                            collect.OnLoadInternal(asset);
//                        }
//                            
//                    }
//                }
//            }

            collect.Save(AssetManagerSetting.EditorWorkspaceFilePath.AssetCollectGuide);
            #endif
        }

        public void MergeToGuide()
        {
            #if UNITY_EDITOR
            AssetCollect guide = LoadGuide();
            guide.InternalListToDict();

            for(int i = 0; i < internalList.Count; i ++)
            {
                if (!guide.internalDict.ContainsKey(internalList[i].url))
                {
                    guide.AddInternal(internalList[i].url);

                    Debug.Log(internalList[i].url);
                }
            }



            guide.Save(AssetManagerSetting.EditorWorkspaceFilePath.AssetCollectGuide);
            #endif
        }

        public static AssetCollect LoadGuide()
        {
            #if UNITY_EDITOR
            return Load(AssetManagerSetting.EditorWorkspaceFilePath.AssetCollectGuide);
            #endif
            return null;
        }

        #if UNITY_EDITOR
        [UnityEditor.MenuItem ("Assets/EditorMerge AssetCollect")]
        public static void EditorMerge()
        {
            AssetCollect guide = LoadGuide();
            guide.InternalListToDict();
            AssetCollect now = Load(AssetManagerSetting.EditorRoot.WorkspaceAssetCollect + "2017-04-18_15-15-15.json");
            for(int i = 0; i < now.internalList.Count; i ++)
            {
                if (!guide.internalDict.ContainsKey(now.internalList[i].url))
                {
                    guide.internalDict.Add(now.internalList[i].url, now.internalList[i]);
                    guide.internalList.Add(now.internalList[i]);

                    Debug.Log(now.internalList[i].url);
                }
            }

            guide.Save(AssetManagerSetting.EditorWorkspaceFilePath.AssetCollectGuide);
        }

        [UnityEditor.MenuItem ("Assets/EditorMerge AssetCollect LoadAssetBundleFromFileDict")]
        public static void EditorMergeLoadAssetBundleFromFileDict()
        {
            AssetCollect guide = LoadGuide();
            guide.InternalListToDict();

            string path = AssetManagerSetting.EditorRoot.WorkspaceAssetCollect + "c#_collect/LoadAssetBundleFromFileDict.txt";

            string outDir = AssetManagerSetting.EditorRoot.WorkspaceAssetCollect + "LoadError";
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            string[] lines = File.ReadAllLines(path);
            foreach(string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                if (!guide.internalDict.ContainsKey(line))
                {
                    guide.AddInternal(line);
                    Debug.Log(line);

                }

//                                    string dest = outDir + "/" + line;
//                                    if (!Directory.Exists(Path.GetDirectoryName(dest)))
//                                    {
//                                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
//                                    }
//                                    File.Copy(AssetManagerSetting.EditorRoot.WorkspaceStreamPlatform + line, dest);
            }
            guide.Save(AssetManagerSetting.EditorWorkspaceFilePath.AssetCollectGuide);
        }
        #endif

    }

    [System.Serializable]
    public class AssetCollectInternalItem
    {
        [SerializeField]
        public string   url;

        [SerializeField]
        public int      loadNum;

        [SerializeField]
        public int      unloadNum;
    }
}