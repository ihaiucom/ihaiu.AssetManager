using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {

        /** 同步加载 */
        public UnityEngine.Object LoadResourceSync(string path, System.Type type)
        {
            if (AssetManagerSetting.IsCacheResourceAsset)
            {
                LoadedResource loaded = GetLoadedResource(path, type);
                if (loaded != null)
                {
                    loaded.referencedCount++;
                    return loaded.obj;
                }
            }

            UnityEngine.Object obj = Resources.Load(path, type);
            CreateLoadedResource(path, type, obj);
            return obj;
        }


    }
}