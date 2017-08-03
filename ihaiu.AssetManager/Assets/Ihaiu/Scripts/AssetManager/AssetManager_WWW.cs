using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {
        public Dictionary<string, LoadedWWWCache>   loadedWWWCacheDict = new Dictionary<string, LoadedWWWCache>();
        public List<LoadedWWWCache>                 loadedWWWCacheList = new List<LoadedWWWCache>();

		public void UnloadWWWCache(string url)
		{
			if(loadedWWWCacheDict.ContainsKey(url))
			{
				LoadedWWWCache item = loadedWWWCacheDict[url];
				loadedWWWCacheDict.Remove(item.url);
				loadedWWWCacheList.Remove(item);
				item.UnloadObj();
			}
		}

        public void CheckLoadedWWWCache()
        {
            if (loadedWWWCacheList.Count > AssetManagerSetting.WWWCacheMaxNum)
            {
                loadedWWWCacheList.Sort(LoadedWWWCache.Compare);
                while(loadedWWWCacheList.Count > AssetManagerSetting.WWWCacheMaxNum)
                {
                    LoadedWWWCache item = loadedWWWCacheList[0];
                    loadedWWWCacheDict.Remove(item.url);
                    loadedWWWCacheList.Remove(item);
                    item.UnloadObj();
                }
            }
        }


        public void LoadWWWAsync(string url, Type type, Action<string, object, object[]> callback, params object[] callbackArgs)
        {
            if (loadedWWWCacheDict.ContainsKey(url))
            {
                LoadedWWWCache cache = loadedWWWCacheDict[url];
                if (cache != null)
                {
                    cache.referencedCount++;
                    cache.lastTime = Time.unscaledTime;
                    if (cache.obj != null)
                    {
                        callback(url, cache.obj, callbackArgs);
                        return;
                    }
                    cache.callbacks.Add(new CallbackStruct(url, callback, callbackArgs));
                    return;
                }
            }
            else
            {
                CheckLoadedWWWCache();
                LoadedWWWCache cache = new LoadedWWWCache(url, type);
                loadedWWWCacheDict.Add(url, cache);
                loadedWWWCacheList.Add(cache);
            }

            StartCoroutine(OnLoadWWWAsync(url, type, callback, callbackArgs));
        }

        IEnumerator OnLoadWWWAsync(string url, Type type, Action<string, object, object[]> callback, params object[] callbackArgs)
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

                if(www.texture != null)
                {
                    obj = www.texture;
//                    if (type == AssetManagerSetting.tmpSpriteType)
                    {
                        Texture2D texture = www.texture;
                        obj  = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(texture.width * 0.5F, texture.height * 0.5F));
                    }
                }
                else if(www.GetAudioClip() != null)
                {
                    obj = www.GetAudioClip();
                }
                else if(www.assetBundle != null)
                {
                    obj = www.assetBundle;
				}
				else if(www.text != null)
				{
					obj = www.text;
				}
                else
                {
                    obj = www.bytes;
                }

                callback(url, obj, callbackArgs);

                if (loadedWWWCacheDict.ContainsKey(url))
                {
                    LoadedWWWCache cache = loadedWWWCacheDict[url];
                    if (cache != null)
                    {
                        cache.obj = obj;
                        cache.CallCallbacks();
                    }
                }
            }

            www.Dispose();
            www = null;
        }
    }
}