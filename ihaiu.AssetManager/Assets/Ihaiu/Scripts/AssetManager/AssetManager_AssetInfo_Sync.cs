using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {

        //==============================================
        // 读取资源文件配置
        //----------------------------------------------
        private void ReadFilesSync()
        {
            ParseInfo(AssetManagerSetting.SyncLoadFile.AssetListLoaMap());
            AssetManagerSetting.dontUnloadAssetFileList = AssetFileList.Deserialize(AssetManagerSetting.SyncLoadFile.AssetListDontUnload());
        }





        /// <summary>
        /// 同步--加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源).</param>
        public void LoadSync(string filename, Action<string, object> callback)
        {
            LoadSync(filename, (string path, object obj, object[] args) => { callback(path, obj); }, null,typeof(System.Object));
        }

        /// <summary>
        /// 同步--加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源).</param>
        /// <param name="type">资源类型.</param>
        public void LoadSync(string filename, Action<string, object> callback, Type type)
        {
            LoadSync(filename, (string path, object obj, object[] args) => { callback(path, obj); }, null,type);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源, 回调参数).</param>
        public void LoadSync(string filename, Action<string, object, object[]> callback)
        {
            LoadSync(filename, callback, null,typeof(System.Object));
        }

        /// <summary>
        /// 同步--加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源, 回调参数).</param>
        /// <param name="type">资源类型.</param>
        public void LoadSync(string filename, Action<string, object, object[]> callback, Type type)
        {
            LoadSync(filename, callback, null,type);
        }


        /// <summary>
        /// 同步--加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源, 回调参数).</param>
        /// <param name="arg">回调参数.</param>
        public void LoadSync(string filename, Action<string, object, object[]> callback, object[] callbackArgs)
        {
            LoadSync(filename, callback, callbackArgs, tmpObjType);
        }





        /// <summary>
        /// 同步--加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源, 回调参数).</param>
        /// <param name="arg">回调参数.</param>
        /// <param name="type">资源类型.</param>
        public void LoadSync(string filename, Action<string, object, object[]> callback, object[] callbackArgs, Type type)
        {
            object obj =  LoadSync(filename, type);
            if(callback != null) callback(filename, obj, callbackArgs);
        }



        /// <summary>
        /// 同步--加载.
        /// </summary>
        /// <returns>返回资源</returns>
        /// <param name="filename">文件名.</param>
        /// <typeparam name="T">要加载的资源类型</typeparam>
        public T LoadSync<T>(string filename)
        {
            return (T) LoadSync(filename, typeof(T));
        }


        /// <summary>
        /// 同步--加载.
        /// </summary>
        /// <returns>返回资源</returns>
        /// <param name="filename">文件名.</param>
        /// <typeparam name="T">要加载的资源类型</typeparam>
        public object LoadSync(string filename, Type type)
        {
            if (string.IsNullOrEmpty(filename))
            {
                Debug.LogErrorFormat("LoadSync filename=" + filename);
                return null;
            }

            if(AssetManagerSetting.IsConfigFile(filename))
            {
                return LoadConfigSync(filename);
            }

            string filenameLower = filename.ToLower();
            AssetInfo fileInfo;
            if(!assetInfoDict.TryGetValue(filenameLower, out fileInfo))
            {
                //Debug.LogError("[AssetMananger] LoadSync 资源配置不存在或者加载出错 name="+filenameLower + "   assetInfo=" + fileInfo );
               
                return LoadResourceSync(filename, type);
            }


            AddToModule(fileInfo);

            if(fileInfo.objType != null &&  (type == null || type == tmpObjType))
            {
                type = fileInfo.objType;
            }


            if(fileInfo.loadType == AssetLoadType.AssetBundle)
            {
                #if UNITY_EDITOR
                if (AssetManagerSetting.EditorSimulateAssetBundle)
                {
                    try
                    {
                        return UnityEditor.AssetDatabase.LoadAssetAtPath(fileInfo.path, type);
                    }
                    catch(Exception e)
                    {
                        Debug.LogErrorFormat("LoadSync fileInfo.path={0}  e={1}", fileInfo.path, e);
                        return null;
                    }
                }
                else
                #endif
                {
                    return LoadAssetSync(fileInfo.assetBundleName, fileInfo.assetName, type);
                }
            }
            else
            {
                return LoadResourceSync(fileInfo.path, type);
            }
        }



       

      

    }
}