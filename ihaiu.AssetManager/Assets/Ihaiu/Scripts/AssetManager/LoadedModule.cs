using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public class LoadedModule
    {
        public AssetManager assetManager;
        public int moduleId;
        public Dictionary<string, LoadedModuleAsset> dict = new Dictionary<string, LoadedModuleAsset>();
        public int          openCount;
        public int          referencedCount;
        public int          maxAssetCount;

        private string _name;
        public string name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
//                    MenuConfig config = Coo.menuManager.configSet[moduleId];
//                    if (config != null)
//                    {
//                        _name = config.name;
//                    }
//                    else
                    {
                        _name = "未知";
                    }
                }

                return _name;
            }
        }


        public LoadedModule(int moduleId, AssetManager assetManager)
        {
            this.moduleId = moduleId;
            this.assetManager = assetManager;
        }



        public void Add(AssetInfo asset)
        {
            LoadedModuleAsset loaded;
            if(!dict.TryGetValue(asset.name, out loaded))
            {
                loaded = LoadedModuleAsset.Spawn();
                loaded.assetInfo = asset;
                dict.Add(asset.name, loaded);

                if(maxAssetCount < dict.Count) maxAssetCount = dict.Count;
            }

            loaded.referencedCount++;
        }


        List<string> willRemoveKeys = new List<string>();
        public void Unload()
        {
            willRemoveKeys.Clear();

            foreach(var kvp in dict)
            {
                if (!AssetManagerSetting.dontUnloadAssetFileList.Has(kvp.Key))
                {
                    //                    assetManager.Unload(kvp.Value.assetInfo.name, kvp.Value.referencedCount, moduleId == MenuType.WarScene);
                    assetManager.Unload(kvp.Value.assetInfo.name, kvp.Value.referencedCount);
                    LoadedModuleAsset.Despawn(kvp.Value);
                    willRemoveKeys.Add(kvp.Key);
                }
            }

            for(int i = willRemoveKeys.Count - 1; i >= 0; i --)
            {
                dict.Remove(willRemoveKeys[i]);
            }

            willRemoveKeys.Clear();
        }

    }
}