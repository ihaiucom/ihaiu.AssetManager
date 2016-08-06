using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Ihaiu.Assets
{
    public partial class AssetManager 
    {
        public void LoadResourceAsync(string path, System.Type type, Action<string, object, object[]> callback, params object[] callbackArgs)
        {
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

            if(callback != null)
            {
                callback(path, resourceRequest.asset, callbackArgs);
            }
        }
    }
}