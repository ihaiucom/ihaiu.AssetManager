using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {
        Dictionary<string, AssetBundleLoadAssetOperation>   AssetOperationDict  = new Dictionary<string, AssetBundleLoadAssetOperation>();
        public List<AssetBundleLoadAssetOperation>          AssetOperationList  = new List<AssetBundleLoadAssetOperation>();

        public AssetBundleLoadAssetOperation LoadOperation(string filename)
        {
            return LoadOperation(filename, tmpObjType);
        }

        public AssetBundleLoadAssetOperation LoadOperation(string filename, Type type)
        {
            AssetBundleLoadAssetOperation operation = null;

            string filenameLower = filename.ToLower();

            if (AssetOperationDict.ContainsKey(filenameLower))
            {
                operation = AssetOperationDict[filenameLower];
                operation.referencedCount++;

                AssetInfo fileInfo;
                if (assetInfoDict.TryGetValue(filenameLower, out fileInfo))
                {
                    AddToModule(fileInfo);
                }

                return operation;
            }

            if (AssetManagerSetting.IsConfigFile(filename))
            {
                operation = new AssetBundleLoadAssetOperationSimulation(LoadConfig(filename));
            }
            else
            {

                AssetInfo fileInfo;
                if (!assetInfoDict.TryGetValue(filenameLower, out fileInfo))
                {
                    operation = new AssetBundleLoadAssetOperationSimulation(Resources.Load(filename, type));
                }
                else
                {

                    AddToModule(fileInfo);

                    if (fileInfo.objType != null && (type == null || type == tmpObjType))
                    {
                        type = fileInfo.objType;
                    }


                    if (fileInfo.loadType == AssetLoadType.AssetBundle)
                    {
                        #if UNITY_EDITOR
                        if (AssetManagerSetting.EditorSimulateAssetBundle)
                        {
                            UnityEngine.Object target = UnityEditor.AssetDatabase.LoadAssetAtPath(fileInfo.path, type);
                            operation = new AssetBundleLoadAssetOperationSimulation(target);
                        }
                        else
                        #endif
                        {
                            operation = manifestAssetBundleManager.LoadAssetAsync(fileInfo.assetBundleName, fileInfo.assetName, type);
                        }
                    }
                    else
                    {
                        operation = new AssetBundleLoadAssetOperationSimulation(Resources.Load(fileInfo.path, type));
                    }
                }
            }

            operation.referencedCount++;
            operation.key = filenameLower;
            AssetOperationDict.Add(filenameLower, operation);
            AssetOperationList.Add(operation);

            return operation;
        }

        public void UnloadOperation(string filename, bool isSetLastTime)
        {
            filename = filename.ToLower();
            if (AssetOperationDict.ContainsKey(filename))
            {
                AssetOperationDict[filename].referencedCount--;
                if (isSetLastTime)
                {
                    AssetOperationDict[filename].lastTime = Time.unscaledTime;
                }
            }
        }

        public void RemoveLoadOperation(string filename)
        {
            filename = filename.ToLower();
            if (AssetOperationDict.ContainsKey(filename))
            {
                AssetOperationList.Remove(AssetOperationDict[filename]);
                AssetOperationDict.Remove(filename);
            }
        }




        public void CheckLoadOperateCache()
        {
            if (!isLoadOperateCacheChecking)
            {
                isLoadOperateCacheChecking = true;
                loadOperateCheckCacheCoroutiner = StartCoroutine(OnCheckLoadOperateCache());
            }
        }

        bool isLoadOperateCacheChecking = false;
        Coroutine loadOperateCheckCacheCoroutiner;
        IEnumerator OnCheckLoadOperateCache()
        {
            AssetBundleLoadAssetOperation operation;
            while (true)
            {
                if (AssetManagerSetting.CheckCacheActive)
                {
                    for (int i = AssetOperationList.Count - 1; i >= 0; i--)
                    {
                        operation = AssetOperationList[i];
                        if (operation.referencedCount <= 0 && Time.unscaledTime - operation.lastTime > AssetManagerSetting.CacheAssetTime)
                        {
                            RemoveLoadOperation(operation.key);
                        }

                        yield return new WaitForEndOfFrame();

                        if (i >= AssetOperationList.Count)
                        {
                            i = AssetOperationList.Count - 1;
                        }
                    }
                }

                yield return new WaitForSeconds(AssetManagerSetting.CheckCacheAssetRate);
            }
        }


        public void ClearLoadOperateCache()
        {
            AssetBundleLoadAssetOperation operation;
            for(int i = AssetOperationList.Count - 1; i >= 0; i --)
            {
                operation = AssetOperationList[i];
                if (operation.referencedCount <= 0 && Time.unscaledTime - operation.lastTime > AssetManagerSetting.CacheAssetTime)
                {
                    RemoveLoadOperation(operation.key);
                }
            }
        }


    }
}