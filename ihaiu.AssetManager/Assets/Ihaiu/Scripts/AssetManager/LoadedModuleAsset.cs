using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public class LoadedModuleAsset
    {
        public AssetInfo    assetInfo;
        public int          referencedCount;






        #region pool
        public static Stack<LoadedModuleAsset> pool = new Stack<LoadedModuleAsset>();

        public static LoadedModuleAsset Spawn()
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }

            return new LoadedModuleAsset();
        }


        public static void Despawn(LoadedModuleAsset item)
        {
            item.assetInfo          = null;
            item.referencedCount    = 0;
            pool.Push(item);
        }
        #endregion
    }
}