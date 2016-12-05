using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.ihaiu
{
    public class ManifestAssetBundleManager : IAssetBundleManager 
    {
        #region log
        public enum LogMode { All, JustErrors };
        public enum LogType { Info, Warning, Error };

        static LogMode m_LogMode = LogMode.JustErrors;

        private static void Log(LogType logType, string text)
        {
            if (logType == LogType.Error)
                Debug.LogError("[ManifestAssetBundleManager] " + text);
            else if (m_LogMode == LogMode.All)
                Debug.Log("[ManifestAssetBundleManager] " + text);
        }

        private static void LogFormat(LogType logType, string text, params object[] args)
        {
            if (logType == LogType.Error)
                Debug.LogErrorFormat("[ManifestAssetBundleManager] " + text, args);
            else if (m_LogMode == LogMode.All)
                Debug.LogFormat("[ManifestAssetBundleManager] " + text, args);
        }

        #endregion



        #region init
        // 资源管理
        public AssetManager assetManager {get; set;}
        // 资源包清单
        public AssetBundleManifest assetBundleManifest {get; set;}


        // 清单路径
        public string manifestPath;

        public ManifestAssetBundleManager(AssetManager assetManager, string manifestPath)
        {
            this.assetManager = assetManager;
            this.manifestPath = manifestPath;
        }


        public Coroutine StartCoroutine (IEnumerator routine)
        {
            return assetManager.StartCoroutine_Auto (routine);
        }


        public IEnumerator LoadManifest()
        {
            WWW www = new WWW(manifestPath);
            yield return www;

            if(!string.IsNullOrEmpty(www.error))
            {
                LogFormat(LogType.Warning ,"OnLoadManifest manifest路径不对，或文件不存在 manifestPath={0}, www.error={1}", manifestPath, www.error);
                yield break;
            }

            AssetBundle assetBundle = www.assetBundle;

            AssetBundleRequest  assetBundleRequest  = assetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            yield return assetBundleRequest;
            assetBundleManifest = (AssetBundleManifest) assetBundleRequest.asset;

            assetManager.OnLoadManifest(this);
        }
        #endregion




        #region Load 对外方法
        /** 加载资源 */
        public AssetBundleLoadAssetOperation LoadAssetAsync (string assetBundleName, string assetName, System.Type type)
        {
            LogFormat(LogType.Info, "LoadAssetAsync assetBundleName={0}, assetName={1}, type={2}", assetBundleName, assetName, type);

            AssetBundleLoadAssetOperation operation = null;
            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateAssetBundle)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    Debug.LogErrorFormat("LoadAssetAsync用AssetDatabase模拟加载没有找到该文件  assetBundleName={0}, assetName={1}", assetBundleName, assetName);
                    return null;
                }

                // @TODO: Now we only get the main object from the first asset. Should consider type also.
                Object target = AssetDatabase.LoadAssetAtPath(assetPaths[0], type);
                operation = new AssetBundleLoadAssetOperationSimulation (target);
            }
            else
            #endif
            {
                assetBundleName = RemapVariantName (assetBundleName);
                LoadAssetBundle (assetBundleName);
                operation = new AssetBundleLoadAssetOperationFull (this, assetBundleName, assetName, type);

                m_InProgressOperations.Add (operation);
            }

            return operation;
        }

        /** 加载场景 */
        public AssetBundleLoadOperation LoadLevelAsync (string assetBundleName, string levelName, bool isAdditive)
        {

            LogFormat(LogType.Info, "LoadLevelAsync assetBundleName={0}, levelName={1}, isAdditive={2}", assetBundleName, levelName, isAdditive);

            AssetBundleLoadOperation operation = null;
            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateAssetBundle)
            {
                operation = new AssetBundleLoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
            }
            else
            #endif
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle (assetBundleName);
                operation = new AssetBundleLoadLevelOperation (this, assetBundleName, levelName, isAdditive);

                m_InProgressOperations.Add (operation);
            }

            return operation;
        }
        #endregion



        // 资源别名列表
        string[]                                    m_ActiveVariants            = { };
        // ”资源包信息“ 字典
        Dictionary<string, LoadedAssetBundle>       m_LoadedAssetBundles        = new Dictionary<string, LoadedAssetBundle>();
        // WWW 字典
        Dictionary<string, WWW>                     m_DownloadingWWWs           = new Dictionary<string, WWW>();
        // 加载出错 字典
        Dictionary<string, string>                  m_DownloadingErrors         = new Dictionary<string, string>();
        // ”资源包操作“列表
        List<AssetBundleLoadOperation>              m_InProgressOperations      = new List<AssetBundleLoadOperation>();
        // ”资源包“依赖列表 字典
        Dictionary<string, string[]>                m_Dependencies              = new Dictionary<string, string[]>();

        public Dictionary<string, LoadedAssetBundle> LoadedAssetBundles
        {
            get
            {
                return m_LoadedAssetBundles;
            }
		}

		/** 获取等待加载的依赖文件 */
		public List<string> GetWaitLoadResDependencies(string assetBundleName)
		{
			List<string> list = new List<string>();
			// 检查是否有依赖的“资源包”列表, 如果没有依赖，成功加载返回“资源包信息”
			string[] dependencies = null;
			if (m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
			{
				// 检测依赖的资源包是否加载完成
				foreach(var dependency in dependencies)
				{
					if (string.IsNullOrEmpty (dependency))
						continue;

					// 如果依赖的资源还没加载完成，返回null，让操作继续等待
					LoadedAssetBundle dependentBundle;
					m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
					if (dependentBundle == null)
					{
						list.Add(dependency);
					}
				}
			}

			return list;
		}





        /** 获取“资源包信息”、检测加载状态 */
        public LoadedAssetBundle GetLoadedAssetBundle (string assetBundleName, out string error, bool isTryForceGetBundle)
        {
            // 如果加载出错，返回空，并返回错
            if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
            {
                //              Debug.Log("m_DownloadingErrors="+error+",assetBundleName="+assetBundleName);
                return null;
            }

            // 如果”资源信息包“还没找到说明还没加载完成，返回null，让操作继续等待
            LoadedAssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
            {
                //              Debug.Log("bundle loading="+error+",assetBundleName="+assetBundleName);
                return null;
            }

            if (isTryForceGetBundle)
            {
                return bundle;
            }

            // 检查是否有依赖的“资源包”列表, 如果没有依赖，成功加载返回“资源包信息”
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
            {
                //              Debug.Log("bundle no depend="+error+",assetBundleName="+assetBundleName);
                return bundle;
            }

            // 检测依赖的资源包是否加载完成
            foreach(var dependency in dependencies)
            {
                if(string.IsNullOrEmpty(dependency)) continue;

                // 检测到依赖的资源加载出错，返回自己的资源，并返回依赖的资源错误,
                if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
                {
                    //                  Debug.Log("m_Downloading depend Errors="+error+",assetBundleName="+assetBundleName);
                    return bundle;
                }

                // 如果依赖的资源还没加载完成，返回null，让操作继续等待
                LoadedAssetBundle dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                {
                    //                  Debug.Log("m_Downloading dependency loading="+error+",assetBundleName="+assetBundleName+",dependentBundle="+dependency);
                    return null;
                }
            }

            // 成功加载返回“资源包信息”
            return bundle;
        }

        /** 获取资源包别名（高清、标清，中文、英文等版本）*/
        protected string RemapVariantName(string assetBundleName)
        {
            // 获取所有资源包别名列表
            string[] bundlesWithVariant = assetBundleManifest.GetAllAssetBundlesWithVariant();

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;

                int found = System.Array.IndexOf(m_ActiveVariants, curSplit[1]);

                // If there is no active variant found. We still want to use the first 
                if (found == -1)
                    found = int.MaxValue-1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue-1)
            {
                Debug.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
            }

            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }

        /**  加载”资源包“和他依赖的资源包 */
        protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
        {
            LogFormat(LogType.Info, "LoadAssetBundle assetBundleName={0}", assetBundleName);

            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateAssetBundle)
                return;
            #endif

            if (!isLoadingAssetBundleManifest)
            {
                if (assetBundleManifest == null)
                {
                    Debug.LogError("请先加载初始化AssetBundleManifest,  调用LoadManifest()");
                    return;
                }
            }

            // 检测该AssetBundle是否已经设置处理
            bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

            // 如果是第一次加载该AssetBundle那么就检测依赖并加载依赖
            if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
                LoadDependencies(assetBundleName);
        }

        /** 检测是否已经创建了WWW,没有就创建WWW */
        protected bool LoadAssetBundleInternal (string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            // Already loaded.
            LoadedAssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                bundle.m_ReferencedCount++;
                return true;
            }

            //            string path = AssetManagerSetting.GetAbsoluteAssetBundlePath(assetBundleName);
            //            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            //            m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(assetBundle) );
            //            return false;

            // @TODO: Do we need to consider the referenced count of WWWs?
            // In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
            // But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
            if (m_DownloadingWWWs.ContainsKey(assetBundleName) )
                return true;

            WWW download = null;
            string url = AssetManagerSetting.GetAbsoluteAssetBundleURL(assetBundleName);

            // For manifest assetbundle, always download it as we don't have hash for it.
            if (isLoadingAssetBundleManifest)
                download = new WWW(url);
            else
                download = WWW.LoadFromCacheOrDownload(url, assetBundleManifest.GetAssetBundleHash(assetBundleName), 0); 

            m_DownloadingWWWs.Add(assetBundleName, download);

            return false;
        }

        /** 加载依赖资源 */
        protected void LoadDependencies(string assetBundleName)
        {
            if (assetBundleManifest == null)
            {
                Debug.LogError("请先加载初始化AssetBundleManifest,  调用LoadManifest()");
                return;
            }

            // 获取依赖的文件列表
            string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
                return;

            // 查找别名
            for (int i=0;i<dependencies.Length;i++)
                dependencies[i] = RemapVariantName (dependencies[i]);

            // Record and load all dependencies.
            m_Dependencies.Add(assetBundleName, dependencies);
            for (int i=0;i<dependencies.Length;i++)
                LoadAssetBundleInternal(dependencies[i], false);
        }

        /** 卸载资源包和他依赖的资源包 */
        public void UnloadAssetBundle(string assetBundleName)
        {
            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateAssetBundle)
                return;
            #endif

            LogFormat(LogType.Info, "卸载assetBundleName={0} 前, m_LoadedAssetBundles.Count={1}", assetBundleName, m_LoadedAssetBundles.Count);

            UnloadAssetBundleInternal(assetBundleName);
            UnloadDependencies(assetBundleName);

            LogFormat(LogType.Info, "卸载assetBundleName={0} 后, m_LoadedAssetBundles.Count={1}", assetBundleName, m_LoadedAssetBundles.Count);
        }

        /** 卸载依赖的资源包 */
        protected void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
                return;

            // Loop dependencies.
            foreach(var dependency in dependencies)
            {
                if(string.IsNullOrEmpty(dependency)) continue;
                
                UnloadAssetBundleInternal(dependency);
            }

            m_Dependencies.Remove(assetBundleName);
        }

        protected void UnloadAssetBundleInternal(string assetBundleName)
        {
            string error;
            LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error, false);
            if (bundle == null)
                return;

            if (--bundle.m_ReferencedCount == 0)
            {
                bundle.m_AssetBundle.Unload(false);
                m_LoadedAssetBundles.Remove(assetBundleName);

                LogFormat(LogType.Info, "卸载assetBundleName={0}成功", assetBundleName);
            }
        }



        /** 更新检测状态 */
        public void Update()
        {
            // 将要移除的www列表
            var keysToRemove = new List<string>();

            // 遍历www
            foreach (var keyValue in m_DownloadingWWWs)
            {
                WWW download = keyValue.Value;

                // 如果加载出错，添加到出错列表，添加到将要移除的WWW列表
                if (download.error != null)
                {
                    m_DownloadingErrors.Add(keyValue.Key, string.Format("加载失败 assetBundleName{0}, download.url={1}, download.error={2}", keyValue.Key, download.url, download.error));
                    keysToRemove.Add(keyValue.Key);
                    continue;
                }

                // If downloading succeeds.
                if(download.isDone)
                {
                    AssetBundle bundle = download.assetBundle;
                    if (bundle == null)
                    {
                        m_DownloadingErrors.Add(keyValue.Key, string.Format("加载出错: 该WWW数据不是AssetBundle类型。 download.assetBundle=null。 assetBundleName{0}, download.url={1}", keyValue.Key, download.url));
                        keysToRemove.Add(keyValue.Key);
                        continue;
                    }

                    //Debug.Log("Downloading " + keyValue.Key + " is done at frame " + Time.frameCount);
                    m_LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle) );
                    keysToRemove.Add(keyValue.Key);
                }
            }

            // Remove the finished WWWs.
            foreach( var key in keysToRemove)
            {
                WWW download = m_DownloadingWWWs[key];
                m_DownloadingWWWs.Remove(key);
                download.Dispose();
            }

            // Update all in progress operations
            for (int i=0;i<m_InProgressOperations.Count;)
            {
                if (!m_InProgressOperations[i].Update())
                {
                    m_InProgressOperations.RemoveAt(i);
                }
                else
                    i++;
            }
        }


    }
}