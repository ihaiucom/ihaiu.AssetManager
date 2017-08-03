using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.ihaiu
{
    public partial class ManifestAssetBundleManager
    {
        public void LoadManifestSync()
        {
            if (abManifest != null)
                abManifest.Unload(true);
            

            abManifest = AssetManagerSetting.SyncLoadFile.Manifest();
            assetBundleManifest = abManifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }


        #region Load 对外方法
        /** 同步加载资源 */
        public Object LoadAssetSync (string assetBundleName, string assetName, System.Type type)
        {
            LogFormat(LogType.Info, "LoadAssetSync assetBundleName={0}, assetName={1}, type={2}", assetBundleName, assetName, type);

            AssetBundleLoadAssetOperation operation = null;
            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateAssetBundle)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    Debug.LogErrorFormat("LoadAssetSync用AssetDatabase模拟加载没有找到该文件  assetBundleName={0}, assetName={1}", assetBundleName, assetName);
                    return null;
                }

                return AssetDatabase.LoadAssetAtPath(assetPaths[0], type);
            }
            else
            #endif
            {
                assetBundleName = RemapVariantName (assetBundleName);

                AssetBundle assetBundle = LoadAssetBundleSync (assetBundleName);
                if (assetBundle != null)
                {
                    return assetBundle.LoadAsset(assetName, type);
                }
                else
                {
                    return null;
                }
            }
        }

        /** 同步加载场景 */
        public void LoadLevelSync (string assetBundleName, string levelName, bool isAdditive)
        {

            LogFormat(LogType.Info, "LoadLevelSync assetBundleName={0}, levelName={1}, isAdditive={2}", assetBundleName, levelName, isAdditive);

            AssetBundleLoadOperation operation = null;
            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateAssetBundle)
            {
                string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
                if (levelPaths.Length == 0)
                {
                    Debug.LogError("没有找到场景， 请检查Build Setting的Scnes In Build. \"" + levelName + "\" in " + assetBundleName);
                    return;
                }

                if (isAdditive)
                    UnityEditor.EditorApplication.LoadLevelAdditiveInPlayMode(levelPaths[0]);
                else
                    UnityEditor.EditorApplication.LoadLevelInPlayMode(levelPaths[0]);
            }
            else
            #endif
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundleSync (assetBundleName);

                if (isAdditive)
                    SceneManager.LoadScene (levelName, LoadSceneMode.Additive);
                else
                    SceneManager.LoadScene (levelName, LoadSceneMode.Single);
            }
        }

        #endregion



        /** 同步加载AssetBundle */
        public AssetBundle LoadAssetBundleSync(string assetBundleName, bool isLoadingAssetBundleManifest = false)
        {

            LogFormat(LogType.Info, "LoadAssetBundleSync assetBundleName={0}", assetBundleName);

            #if UNITY_EDITOR
            if (AssetManagerSetting.EditorSimulateAssetBundle)
                return null;
            #endif

            if (!isLoadingAssetBundleManifest)
            {
                if (assetBundleManifest == null)
                {
                    Debug.LogError("LoadAssetBundleSync 请先加载初始化AssetBundleManifest,  调用LoadManifest()");
                    return null;
                }
            }

            // 检测该AssetBundle是否已经设置处理
            AssetBundle assetBundle = LoadAssetBundleFromFile(assetBundleName);
            if (assetBundle == null)
                return null;

            // 如果是第一次加载该AssetBundle那么就检测依赖并加载依赖
            if (!isLoadingAssetBundleManifest)
                LoadDependenciesFromFile(assetBundleName);

            return assetBundle;
        }


        /** 检测是否已经创建了WWW,没有就创建WWW */
        protected AssetBundle LoadAssetBundleFromFile (string assetBundleName)
        {
            // Already loaded.
            LoadedAssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);

            if (bundle != null)
            {
                bundle.m_ReferencedCount++;
                return bundle.m_AssetBundle;
            }
            AssetManagerSetting.collect.OnLoadInternal(assetBundleName);

            string path = AssetManagerSetting.GetAbsoluteAssetBundlePath(assetBundleName);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle != null)
            {
                string url = AssetManagerSetting.GetAbsoluteAssetBundleURL(assetBundleName);
                m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(assetBundle, url));
            }
            else
            {
                Debug.LogErrorFormat("LoadAssetBundleFromFile assetBundle=null,  assetBundleName={0}, path={1}", assetBundleName, path);
            }
           
            return assetBundle;
        }

        /** 加载依赖资源 */
        protected void LoadDependenciesFromFile(string assetBundleName)
        {
            if (assetBundleManifest == null)
            {
                Debug.LogError("LoadDependenciesFromFile 请先加载初始化AssetBundleManifest,  调用LoadManifest()");
                return;
            }

            // 获取依赖的文件列表
            string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
                return;


            if (m_Dependencies.ContainsKey(assetBundleName))
            {
                Debug.LogErrorFormat("LoadDependenciesFromFile 已经存在过该assetBundleName的依赖字典 {0}", assetBundleName);
                return;
            }



            // 查找别名
            for (int i=0;i<dependencies.Length;i++)
                dependencies[i] = RemapVariantName (dependencies[i]);

            // Record and load all dependencies.
            m_Dependencies.Add(assetBundleName, dependencies);
            for (int i = 0; i < dependencies.Length; i++) {

                if (string.IsNullOrEmpty (dependencies [i]))
                    continue;

                LoadAssetBundleFromFile (dependencies [i]);
            }
        }


    }
}