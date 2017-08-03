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



        public void UnloadObj()
        {
            if (obj != null)
            {
                //                Debug.LogFormat("assetBundleName={0}, assetName={1}, objType={2}", assetBundleName, assetName, objType);
//                if (objType == AssetManagerSetting.tmpGameObjectType)
//                {
//                    UnityEngine.Object.DestroyImmediate(obj, true);
//                }
//                else
//                {
//                    Resources.UnloadAsset(obj);
//                }
                obj = null;
            }
        }


    }
}