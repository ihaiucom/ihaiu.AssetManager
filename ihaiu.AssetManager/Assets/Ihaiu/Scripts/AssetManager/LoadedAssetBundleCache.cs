using UnityEngine;
using System.Collections;
using System;

namespace com.ihaiu
{
    public class LoadedAssetBundleCache
    {
        public string assetBundleName;
        public string assetName;
        public Type objType;

        public int createCount;
        public float lastTime = 0;

        private int _referencedCount = 0;
        public int referencedCount
        {
            get
            {
                return _referencedCount;
            }

            set
            {
                if (value != 0 && value > _referencedCount)
                {
                    lastTime = Time.unscaledTime;
                }

                _referencedCount = value;
            }
        }

        public UnityEngine.Object obj;

        public LoadedAssetBundleCache(string assetBundleName, string assetName, Type objType)
        {

            this.assetBundleName = assetBundleName;
            this.assetName = assetName;
            this.objType = objType;
            this.referencedCount = 1;
            this.createCount = 1;

        }

    }
}