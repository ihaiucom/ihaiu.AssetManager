using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace com.ihaiu
{
	public partial class AssetManager 
	{
		ManifestAssetBundleManager  manifestAssetBundleManager;

		public Dictionary<string, LoadedAssetBundle>   LoadedAssetBundles
		{
			get
			{
				if (manifestAssetBundleManager != null)
				{
					return manifestAssetBundleManager.LoadedAssetBundles;
				}
				return null;
			}
		}





		public IEnumerator InitManifest()
		{
			manifestAssetBundleManager =  new ManifestAssetBundleManager(this, AssetManagerSetting.ManifestURL);
			#if UNITY_EDITOR
			if(!AssetManagerSetting.EditorSimulateAssetBundle)
			#endif
			{
				yield return StartCoroutine(manifestAssetBundleManager.LoadManifest());
			}
		}

		internal void OnLoadManifest(IAssetBundleManager manifest)
		{
			PrepareFinal();
		}


		void UpdateAssetBundle()
		{
			if(manifestAssetBundleManager != null)
				manifestAssetBundleManager.Update();
		}








		#region LoadedAssetBundleCaches
		List<LoadedAssetBundleCache> LoadedAssetBundleCacheList = new List<LoadedAssetBundleCache>();
		// ”资源包信息“ 字典
		Dictionary<string, Dictionary<string,Dictionary<System.Type, LoadedAssetBundleCache>>>       m_LoadedAssetBundleCaches        = new Dictionary<string, Dictionary<string,Dictionary<System.Type, LoadedAssetBundleCache>>> ();

		public Dictionary<string, Dictionary<string,Dictionary<System.Type, LoadedAssetBundleCache>>>    LoadedAssetBundleCaches
		{
			get
			{
				return m_LoadedAssetBundleCaches;
			}
		}

		void CreateLoadedAssetBundleCache(string assetBundleName, string assetName, System.Type type, UnityEngine.Object obj)
		{
			LoadedAssetBundleCache loaded = GetLoadedAssetBundleCache(assetBundleName, assetName, type);
			if (loaded != null)
			{
				loaded.obj = obj;
				loaded.referencedCount++;
				loaded.createCount++;
			}
			else
			{
				loaded = new LoadedAssetBundleCache(assetBundleName, assetName, type);
				loaded.obj = obj;

				if (!m_LoadedAssetBundleCaches.ContainsKey(assetBundleName))
				{
					m_LoadedAssetBundleCaches.Add(assetBundleName, new Dictionary<string, Dictionary<System.Type, LoadedAssetBundleCache> >() );
				}


				if (!m_LoadedAssetBundleCaches[assetBundleName].ContainsKey(assetName))
				{
					m_LoadedAssetBundleCaches[assetBundleName].Add(assetName, new Dictionary<System.Type, LoadedAssetBundleCache>());
				}

				m_LoadedAssetBundleCaches[assetBundleName][assetName].Add(type, loaded);

				LoadedAssetBundleCacheList.Add(loaded);
			}
		}

		LoadedAssetBundleCache GetLoadedAssetBundleCache(string assetBundleName, string assetName, System.Type type)
		{
			if(m_LoadedAssetBundleCaches.ContainsKey(assetBundleName))
			{
				if (m_LoadedAssetBundleCaches[assetBundleName].ContainsKey(assetName))
				{
					if (m_LoadedAssetBundleCaches[assetBundleName][assetName].ContainsKey(type))
					{
						return m_LoadedAssetBundleCaches[assetBundleName][assetName][type];
					}
				}
			}

			return null;
		}



		void RemoveLoadedAssetBundleCache(string assetBundleName, string assetName, System.Type type)
		{
			if(m_LoadedAssetBundleCaches.ContainsKey(assetBundleName))
			{
				if (m_LoadedAssetBundleCaches[assetBundleName].ContainsKey(assetName))
				{
					if (m_LoadedAssetBundleCaches[assetBundleName][assetName].ContainsKey(type))
					{
						LoadedAssetBundleCacheList.Add(m_LoadedAssetBundleCaches[assetBundleName][assetName][type]);
						m_LoadedAssetBundleCaches[assetBundleName][assetName].Remove(type);
					}
				}
			}
		}
		#endregion







		#region UnloadAssetBundle
		//----------------
		/** 卸载资源包和他依赖的资源包 */
		public void UnloadAssetBundle(string assetBundleName)
		{
			manifestAssetBundleManager.UnloadAssetBundle(assetBundleName);
		}


		public void UnloadAssetBundle(string assetBundleName, string assetName, System.Type type)
		{
			UnloadAssetBundle(assetBundleName, assetName, type, 1, false);
		}


		public void UnloadAssetBundle(string assetBundleName, string assetName, System.Type type, int count, bool isSetLastTime)
		{
			LoadedAssetBundleCache loaded = GetLoadedAssetBundleCache(assetBundleName, assetName, type);
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
						UnloadLoadedAssetBundleCache(loaded);
					}
				}



			}
			else
			{
				manifestAssetBundleManager.UnloadAssetBundle(assetBundleName);
			}
		}

		void UnloadLoadedAssetBundleCache(LoadedAssetBundleCache loaded)
		{

			//                    if (loaded.obj != null)
			//                    {
			//                        UnityEngine.Object.Destroy(loaded.obj);
			//                    }
			loaded.obj = null;

			for(int i = 0; i < loaded.createCount; i ++)
			{
				manifestAssetBundleManager.UnloadAssetBundle(loaded.assetBundleName);
			}

			RemoveLoadedAssetBundleCache(loaded.assetBundleName, loaded.assetName, loaded.objType);
		}


		public void CheckAssetBundleCache()
		{
			if (!isAssetBundleCacheChecking)
			{
				isAssetBundleCacheChecking = true;
				assetBundleCheckCacheCoroutiner = StartCoroutine(OnCheckAssetBundleCache());
			}
		}

        private bool isAssetBundleCacheChecking = false;
        private Coroutine assetBundleCheckCacheCoroutiner;
		IEnumerator OnCheckAssetBundleCache()
		{
			LoadedAssetBundleCache loaded;
			while (true)
			{
				if (AssetManagerSetting.CheckCacheActive)
				{
					for (int i = LoadedAssetBundleCacheList.Count - 1; i >= 0; i--)
					{
						loaded = LoadedAssetBundleCacheList[i];
						if (loaded.referencedCount <= 0 && Time.unscaledTime - loaded.lastTime > AssetManagerSetting.CacheAssetTime)
						{
							UnloadLoadedAssetBundleCache(loaded);
						}

						yield return new WaitForEndOfFrame();

						if (i >= LoadedAssetBundleCacheList.Count)
						{
							i = LoadedAssetBundleCacheList.Count - 1;
						}
					}
				}

				yield return new WaitForSeconds(AssetManagerSetting.CheckCacheAssetRate);
			}
		}


		public void ClearAssetBundleCache()
		{

			LoadedAssetBundleCache loaded;
			for(int i = LoadedAssetBundleCacheList.Count - 1; i >= 0; i --)
			{
				loaded = LoadedAssetBundleCacheList[i];
				if (loaded.referencedCount <= 0)
				{
					UnloadLoadedAssetBundleCache(loaded);
				}
			}
		}

		#endregion









		#region Load Methods
		//=================
		public AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
		{
			return manifestAssetBundleManager.LoadAssetAsync(assetBundleName, assetName, type);
		}

		public AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
		{
			return manifestAssetBundleManager.LoadLevelAsync(assetBundleName, levelName, isAdditive);
		}



		//=================
		public void LoadAssetAsync(string assetBundleName, string assetName, System.Type type, Action<string, string, object, object[]> callback, params object[] callbackArgs)
		{
			StartCoroutine(OnLoadAssetAsync(assetBundleName, assetName, type, callback, callbackArgs));
		}

		IEnumerator OnLoadAssetAsync(string assetBundleName, string assetName, System.Type type, Action<string, string, object, object[]> callback, params object[] callbackArgs)
		{
			AssetBundleLoadAssetOperation operation = manifestAssetBundleManager.LoadAssetAsync(assetBundleName, assetName, type);
			yield return operation;

			if (callback != null)
			{
				UnityEngine.Object obj = operation.GetAsset<UnityEngine.Object>();
				if (obj == null) {
					Debug.LogErrorFormat ("AssetManager_AssetBundle OnLoadAssetAsync obj=null, operation={0}", operation);
				}
				callback(assetBundleName, assetName, obj, callbackArgs);
			}
		}


		//=================
		public void LoadAssetAsync(string filename, string assetBundleName, string assetName, System.Type type, Action<string, object, object[]> callback, params object[] callbackArgs)
		{
			if (assetBundleName.IndexOf("runerewardeffect") != -1) {
				Debug.Log (filename);
			}
			if (AssetManagerSetting.IsCacheAssetBundleAsset)
			{
				LoadedAssetBundleCache loaded = GetLoadedAssetBundleCache(assetBundleName, assetName, type);
				if (loaded != null)
				{
					loaded.referencedCount++;
					if(callback != null) callback(filename, loaded.obj, callbackArgs);
					return;
				}
			}

			StartCoroutine(OnLoadAssetAsync(filename, assetBundleName, assetName, type, callback, callbackArgs));
		}

		IEnumerator OnLoadAssetAsync(string filename, string assetBundleName, string assetName, System.Type type, Action<string, object, object[]> callback, params object[] callbackArgs)
		{
			AssetBundleLoadAssetOperation operation = manifestAssetBundleManager.LoadAssetAsync(assetBundleName, assetName, type);
			yield return operation;

			UnityEngine.Object obj = null;
			if (AssetManagerSetting.IsCacheAssetBundleAsset)
			{
				obj = operation.GetAsset<UnityEngine.Object>();

				if (obj == null) {
					Debug.LogErrorFormat ("AssetManager_AssetBundle OnLoadAssetAsync obj=null, operation={0}", operation);
				}
				CreateLoadedAssetBundleCache(assetBundleName, assetName, type, obj);
			}

			if (callback != null)
			{
				if(obj == null) obj = operation.GetAsset<UnityEngine.Object>();

				if (obj == null) {
					Debug.LogErrorFormat ("AssetManager_AssetBundle OnLoadAssetAsync obj=null, operation={0}", operation);
				}
				callback(filename, obj, callbackArgs);
			}
		}

		//----------------
		public void LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive, Action<string, string, object[]> callback, params object[] callbackArgs)
		{
			StartCoroutine(OnLoadLevelAsync(assetBundleName, levelName, isAdditive, callback, callbackArgs));
		}

		IEnumerator OnLoadLevelAsync(string assetBundleName, string levelName, bool isAdditive, Action<string, string, object[]> callback, params object[] callbackArgs)
		{
			AssetBundleLoadOperation operation = manifestAssetBundleManager.LoadLevelAsync(assetBundleName, levelName, isAdditive);
			yield return operation;

			if (callback != null)
			{
				callback(assetBundleName, levelName, callbackArgs);
			}
		}

		#endregion


	}
}