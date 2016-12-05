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

        private Dictionary<string, AssetInfo> assetInfoDict = new Dictionary<string, AssetInfo>();
        private IEnumerator ReadFiles()
        {
            string path = AssetManagerSetting.LoadAssetListURL;
            WWW www = new WWW(path);
            yield return www;

            if(string.IsNullOrEmpty(www.error))
            {
                ParseInfo(www.text);
            }

            path = AssetManagerSetting.DontUnloadAssetListURL;
            www = new WWW(path);
            yield return www;

            if(string.IsNullOrEmpty(www.error))
            {
                AssetManagerSetting.dontUnloadAssetFileList = AssetFileList.Deserialize(www.text);
            }
        }

        private void ParseInfo(string p)
        {
            AssetLoadType loadType;
            string path,  objType, assetBundleName, assetName, ext;
            string filename;

            using(StringReader stringReader = new StringReader(p))
            {
                while(stringReader.Peek() >= 0)
                {
                    string line = stringReader.ReadLine();
                    if(!string.IsNullOrEmpty(line))
                    {
                        string[] seg = line.Split(';');
                        int length = seg.Length;
                        loadType = (AssetLoadType) Convert.ToInt32(seg[0]);
                        path = seg[1];
                        objType         = length > 2 ? seg[2] : string.Empty;
                        assetBundleName = length > 3 ? seg[3] : string.Empty;
                        assetName       = length > 4 ? seg[4] : string.Empty;


                        filename = path.Replace("{0}/", "").ToLower();

                        #if UNITY_EDITOR
                        ext             = length > 5 ? seg[5] : string.Empty;
                        if (AssetManagerSetting.EditorSimulateAssetBundle)
                        {
                            path = string.Format(path, AssetManagerSetting.EditorRootMResources) + ext;
                        }
                        else
                        #endif
                        {
                            path = AssetManagerSetting.GetPlatformPath(path);
                        }


                        AssetInfo assetInfo;
                        if(!assetInfoDict.TryGetValue(filename, out assetInfo))
                        {
                            assetInfo = new AssetInfo();
                            assetInfo.name = filename;
                            assetInfoDict.Add(filename, assetInfo);
                        }

                        assetInfo.path           = path;
                        assetInfo.loadType       = loadType;
                        assetInfo.objType        = AssetManagerSetting.GetObjType(objType);

                        assetInfo.assetName          = assetName;
                        assetInfo.assetBundleName    = assetBundleName;


                    }
                }
            }


        }



        //-----------------------------------

        Type tmpObjType = typeof(System.Object);



        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源).</param>
        public void Load(string filename, Action<string, object> callback)
        {
            Load(filename, (string path, object obj, object[] args) => { callback(path, obj); }, null,typeof(System.Object));
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源).</param>
        /// <param name="type">资源类型.</param>
        public void Load(string filename, Action<string, object> callback, Type type)
        {
            Load(filename, (string path, object obj, object[] args) => { callback(path, obj); }, null,type);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源, 回调参数).</param>
        public void Load(string filename, Action<string, object, object[]> callback)
        {
            Load(filename, callback, null,typeof(System.Object));
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源, 回调参数).</param>
        /// <param name="type">资源类型.</param>
        public void Load(string filename, Action<string, object, object[]> callback, Type type)
        {
            Load(filename, callback, null,type);
        }


        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源, 回调参数).</param>
        /// <param name="arg">回调参数.</param>
        public void Load(string filename, Action<string, object, object[]> callback, object[] callbackArgs)
        {
            Load(filename, callback, callbackArgs, tmpObjType);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源, 回调参数).</param>
        /// <param name="arg">回调参数.</param>
        /// <param name="type">资源类型.</param>
        public void Load(string filename, Action<string, object, object[]> callback, object[] callbackArgs, Type type)
        {
            if (string.IsNullOrEmpty(filename))
            {
                if (callback != null)
                    callback(filename, null, callbackArgs);

                Debug.LogErrorFormat("Load filename=" + filename);
                return;
            }

            if(AssetManagerSetting.IsConfigFile(filename))
            {
                LoadConfig(filename, callback, callbackArgs);
                return;
            }


            string filenameLower = filename.ToLower();
            AssetInfo fileInfo;
            if(!assetInfoDict.TryGetValue(filenameLower, out fileInfo))
            {
                //                Debug.LogError("[AssetMananger]资源配置不存在或者加载出错 name="+filenameLower + "   assetInfo=" + fileInfo );
                if (callback != null && fileInfo == null)
                {
                    LoadResourceAsync(filename, type, callback, callbackArgs);
                }
                return;
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
                        UnityEngine.Object target = UnityEditor.AssetDatabase.LoadAssetAtPath(fileInfo.path, type);

                        if(callback != null)
                        {
                            callback(filename, target, callbackArgs);
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.LogErrorFormat("fileInfo.path={0}  e={1}", fileInfo.path, e);
                    }
                }
                else
                #endif
                {
                    LoadAssetAsync(filename, fileInfo.assetBundleName, fileInfo.assetName, type, callback, callbackArgs);
                }
            }
            else
            {
                LoadResourceAsync(fileInfo.path, type, callback, callbackArgs);
            }
        }




        public void Unload(string filename)
        {
            Unload(filename, tmpObjType, 1, false);
        }

        public void Unload(string filename, int count)
        {
            Unload(filename, tmpObjType, count, false);
        }



        public void Unload(string filename, int count, bool isSetLastTime)
        {
            Unload(filename, tmpObjType, count, isSetLastTime);
        }



        public void Unload(string filename, Type type)
        {
            Unload(filename, type, 1, false);
        }

        public void Unload(string filename, Type type, int count, bool isSetLastTime)
        {
            UnloadOperation(filename, isSetLastTime);

            string filenameLower = filename.ToLower();
            AssetInfo fileInfo;
            if (!assetInfoDict.TryGetValue(filenameLower, out fileInfo))
            {
                UnloadResource(filename, type);
                return;
            }


            if(fileInfo.objType != null &&  (type == null || type == tmpObjType))
            {
                type = fileInfo.objType;
            }


            if(fileInfo.loadType == AssetLoadType.AssetBundle)
            {
                UnloadAssetBundle(fileInfo.assetBundleName, fileInfo.assetName, type, count, isSetLastTime);
            }
            else
            {
                UnloadResource(fileInfo.path, type, count, isSetLastTime);
            }
        }





        public bool IsFileExist(string filename)
        {
            filename = filename.ToLower();
            AssetInfo fileInfo;
            if(!assetInfoDict.TryGetValue(filename, out fileInfo))
            {
                return false;
            }
            return true;
        }

    }
}