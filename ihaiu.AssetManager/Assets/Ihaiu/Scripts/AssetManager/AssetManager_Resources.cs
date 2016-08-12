using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Ihaiu.Assets
{
    public partial class AssetManager 
    {

        // ”资源包信息“ 字典
        Dictionary<Type, Dictionary<string, LoadedResource>>       m_LoadedResources        = new Dictionary<Type, Dictionary<string, LoadedResource>>();

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
            }
        }

        void RemoveLoadedResource(string path, Type type)
        {
            if (m_LoadedResources.ContainsKey(type))
            {
                if (m_LoadedResources[type].ContainsKey(path))
                    m_LoadedResources[type].Remove(path);
            }
        }

        public void UnloadResource(string path)
        {
            UnloadResource(path, tmpObjType);
        }

        public void UnloadResource(string path, Type type)
        {
            LoadedResource loaded = GetLoadedResource(path, type);
            if (loaded != null)
            {
                loaded.referencedCount--;
                if (loaded.referencedCount <= 0)
                {
                    if (loaded.obj != null)
                    {
                        Resources.UnloadAsset(loaded.obj);
                    }

                    loaded.obj = null;
                    RemoveLoadedResource(path, type);
                }
            }
        }
    }
}