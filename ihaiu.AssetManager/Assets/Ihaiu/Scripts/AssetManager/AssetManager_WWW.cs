using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {
        public void LoadWWWAsync(string url, Action<string, object, object[]> callback, params object[] callbackArgs)
        {
            StartCoroutine(OnLoadWWWAsync(url, callback, callbackArgs));
        }

        IEnumerator OnLoadWWWAsync(string url, Action<string, object, object[]> callback, params object[] callbackArgs)
        {
            if (callback == null)
                yield break;

            WWW www =  new WWW(url);
            yield return www;
            if(!string.IsNullOrEmpty(www.error))
            {
                Debug.LogErrorFormat("[AssetMananger OnLoadWWWAsync] 加载资源出错 url={0},  www.error={1}", url, www.error );
                callback(url, null, callbackArgs);
            }
            else
            {
                object obj = null;

                if(www.text != null)
                {
                    obj = www.text;
                }
                else if(www.texture != null)
                {
                    obj = www.texture;
                }
                else if(www.audioClip != null)
                {
                    obj = www.audioClip;
                }
                else if(www.assetBundle != null)
                {
                    obj = www.assetBundle;
                }
                else
                {
                    obj = www.bytes;
                }

                callback(url, obj, callbackArgs);
            }

            www.Dispose();
            www = null;
        }
    }
}