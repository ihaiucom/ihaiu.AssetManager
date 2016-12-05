using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {

        // ”资源包信息“ 字典
        Dictionary<Type, Dictionary<string, LoadedResource>>       m_LoadedResources        = new Dictionary<Type, Dictionary<string, LoadedResource>>();
        List<LoadedResource>    LoadedResourceList = new List<LoadedResource>();

        public Dictionary<Type, Dictionary<string, LoadedResource>>   LoadedResources
        {
            get
            {
                return m_LoadedResources;
            }
        }

        public void LoadResourceAsync(string path, System.Type type, Action<string, object, object[]> callback, params object[] callbackArgs)
        {
            if (AssetManagerSetting.IsCacheResourceAsset)
            {
                LoadedResource loaded = GetLoadedResource(path, type);
                if (loaded != null)
                {
                    loaded.referencedCount++;
                    if(callback != null) callback(path, loaded.obj, callbackArgs);
                    return;
                }
            }
            StartCoroutine(OnLoadResourceAsync(path, type, callback, callbackArgs));
        }

        private Dictionary<string, int> _preAysncFrameDict = new Dictionary<string, int>();
        IEnumerator OnLoadResourceAsync(string path, System.Type type, Action<string, object, object[]> callback, params object[] callbackArgs)
        {
            if (AssetManagerSetting.ForcedResourceAsynLoadWaitFrame)
            {
                if (_preAysncFrameDict.ContainsKey(path))
                {
                    _preAysncFrameDict[path] = Time.frameCount;
                }
                else
                {
                    _preAysncFrameDict.Add(path, Time.frameCount);
                }
            }

            ResourceRequest resourceRequest = Resources.LoadAsync(path, type);
            yield return resourceRequest;
            if (AssetManagerSetting.ForcedResourceAsynLoadWaitFrame)
            {
                if (Time.frameCount - _preAysncFrameDict[path] == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            CreateLoadedResource(path, type, resourceRequest.asset);

            if(callback != null)
            {
                callback(path, resourceRequest.asset, callbackArgs);
            }
        }



        LoadedResource GetLoadedResource(string path, Type type)
        {
            if(m_LoadedResources.ContainsKey(type))
            {
                if(m_LoadedResources[type].ContainsKey(path))
                    return m_LoadedResources[type][path];
            }

            return null;
        }

        void CreateLoadedResource(string path, Type type, UnityEngine.Object obj)
        {
            LoadedResource loaded = GetLoadedResource(path, type);
            if (loaded != null)
            {
                if (AssetManagerSetting.IsCacheResourceAsset)
                {
                    loaded.obj = obj;
                }
                loaded.referencedCount++;
            }
            else
            {
                loaded = new LoadedResource(path, type);
                if (AssetManagerSetting.IsCacheResourceAsset)
                {
                    loaded.obj = obj;
                }

                if (!m_LoadedResources.ContainsKey(type))
                {
                    m_LoadedResources.Add(type, new Dictionary<string, LoadedResource>());
                }

                m_LoadedResources[type].Add(path, loaded);

                LoadedResourceList.Add(loaded);
            }
        }

        void RemoveLoadedResource(string path, Type type)
        {
            if (m_LoadedResources.ContainsKey(type))
            {
                if (m_LoadedResources[type].ContainsKey(path))
                {
                    LoadedResourceList.Remove(m_LoadedResources[type][path]);
                    m_LoadedResources[type].Remove(path);
                }
            }
        }





        #region UnloadResource
        public void UnloadResource(string path)
        {
            UnloadResource(path, tmpObjType, 1, false);
        }


        public void UnloadResource(string path, Type type)
        {
            UnloadResource(path, type, 1, false);
        }

        public void UnloadResource(string path, Type type, int count, bool isSetLastTime)
        {
            LoadedResource loaded = GetLoadedResource(path, type);
            if (loaded != null)
            {
                if (count < 0)
                {
                    loaded.referencedCount = 0;
                }
                else
                {
                    loaded.referencedCount -= count;
                }



                if (loaded.referencedCount <= 0)
                {
                    if (isSetLastTime)
                    {
                        loaded.lastTime = Time.unscaledTime;
                    }

                    if (!AssetManagerSetting.UseCacheAssetTime)
                    {
                        UnloadLoadedResourceCache(loaded);
                    }
                }

            }
        }


        void UnloadLoadedResourceCache(LoadedResource loaded)
        {
            if (loaded.obj != null)
            {
                try
                {
                    Resources.UnloadAsset(loaded.obj);
                }
                catch(Exception e)
                {
                    Debug.LogError("AssetManager_Resoures UnloadLoadedResourceCache e=" + e);
                }
            }

            loaded.obj = null;
            RemoveLoadedResource(loaded.path, loaded.objType);
        }







        public void CheckResourceCache()
        {
            if (!isResourceCacheChecking)
            {
                isResourceCacheChecking = true;
                resourceCheckCacheCoroutiner = StartCoroutine(OnCheckResourceCache());
            }
        }

        bool isResourceCacheChecking = false;
        Coroutine resourceCheckCacheCoroutiner;
        IEnumerator OnCheckResourceCache()
        {
            LoadedResource loaded;
            while (true)
            {
                if (AssetManagerSetting.CheckCacheActive)
                {
                    for (int i = LoadedResourceList.Count - 1; i >= 0; i--)
                    {
                        loaded = LoadedResourceList[i];
                        if (loaded.referencedCount <= 0 && Time.unscaledTime - loaded.lastTime > AssetManagerSetting.CacheAssetTime)
                        {
                            UnloadLoadedResourceCache(loaded);
                        }

                        yield return new WaitForEndOfFrame();

                        if (i >= LoadedResourceList.Count)
                        {
                            i = LoadedResourceList.Count - 1;
                        }
                    }
                }

                yield return new WaitForSeconds(AssetManagerSetting.CheckCacheAssetRate);
            }
        }


        public void ClearResourceCache()
        {
            LoadedResource loaded;
            for(int i = LoadedResourceList.Count - 1; i >= 0; i --)
            {
                loaded = LoadedResourceList[i];
                if (loaded.referencedCount <= 0 && Time.unscaledTime - loaded.lastTime > AssetManagerSetting.CacheAssetTime)
                {
                    UnloadLoadedResourceCache(loaded);
                }
            }
        }
        #endregion



    }
}