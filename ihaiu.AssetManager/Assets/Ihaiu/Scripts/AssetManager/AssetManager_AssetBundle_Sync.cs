using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace com.ihaiu
{
    public partial class AssetManager 
    {


        public void InitManifestSync()
        {
            if(manifestAssetBundleManager == null) manifestAssetBundleManager =  new ManifestAssetBundleManager(this);
            #if UNITY_EDITOR
            if(!AssetManagerSetting.EditorSimulateAssetBundle)
            #endif
            {
                manifestAssetBundleManager.LoadManifestSync();
            }
        }



        #region Load Methods
        public UnityEngine.Object LoadAssetSync(string assetBundleName, string assetName, System.Type type)
        {
            if (AssetManagerSetting.IsCacheAssetBundleAsset)
            {
                LoadedAssetBundleCache loaded = GetLoadedAssetBundleCache(assetBundleName, assetName, type);
				if (loaded != null && loaded.obj != null)
                {
                    loaded.referencedCount++;
                    return loaded.obj;
                }
            }

            UnityEngine.Object obj = manifestAssetBundleManager.LoadAssetSync(assetBundleName, assetName, type);

            if (AssetManagerSetting.IsCacheAssetBundleAsset)
            {
                if (obj == null) 
                {
                    Debug.LogErrorFormat ("AssetManager_AssetBundle LoadAssetSync obj=null, assetBundleName={0}, assetName={1}, type={2}", assetBundleName, assetName, type);
                }
                CreateLoadedAssetBundleCache(assetBundleName, assetName, type, obj);
            }

           
            StartCoroutine(CheckImmediatelyUnloadAssetBundle(assetBundleName));

            return obj;

        }

       

//        //=================
//        public void LoadLevelSync(string assetBundleName, string levelName, bool isAdditive, Action<string, string, object[]> callback, params object[] callbackArgs)
//        {
//            StartCoroutine(OnLoadLevelAsync(assetBundleName, levelName, isAdditive, callback, callbackArgs));
//        }
//

        #endregion


    }
}