using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

namespace Ihaiu.Assets
{
    public partial class AssetManager 
    {


        //==============================================
        // 读取资源文件配置
        //----------------------------------------------

        private Dictionary<string, AssetInfo> assetInfoDict = new Dictionary<string, AssetInfo>();
        private IEnumerator ReadFiles()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(AssetManagerSetting.AssetlistForResource);
            if(textAsset != null)
            {
                ParseInfo(textAsset.text, AssetLoadType.Resources);
            }


            string path = AssetManagerSetting.AssetlistForStreaming;
            WWW www = new WWW(path);
            yield return www;

            if(string.IsNullOrEmpty(www.error))
            {
                ParseInfo(www.text, AssetLoadType.AssetBundle);
            }

        }

        private void ParseInfo(string p, AssetLoadType loadType)
        {

            // path;objType
            if (loadType == AssetLoadType.Resources)
            {
                ParseInfoResources(p);
            }
            //path;md5;objType;assetName;assetBundleName
            else
            {
                ParseInfoStreaming(p);
            }

        }



        //path;md5;objType;assetName;assetBundleName
        private void ParseInfoStreaming(string p)
        {
            string path,  objType, assetName, assetBundleName;
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
                        path = seg[0];
                        objType         = length > 2 ? seg[2] : string.Empty;
                        assetBundleName = length > 3 ? seg[3] : string.Empty;
                        assetName       = length > 4 ? seg[4] : string.Empty;


                        filename = path.LastIndexOf('.') > 0 ? path.Substring(0, path.LastIndexOf('.')) : path;
                        filename = filename.Replace("{0}/", "").ToLower();

                        path = AssetManagerSetting.GetPlatformPath(path);


                        AssetInfo assetInfo;
                        if(!assetInfoDict.TryGetValue(filename, out assetInfo))
                        {
                            assetInfo = new AssetInfo();
                            assetInfo.name = filename;
                            assetInfoDict.Add(filename, assetInfo);
                        }

                        assetInfo.path           = path;
                        assetInfo.loadType       = AssetLoadType.AssetBundle;
                        assetInfo.objType        = AssetManagerSetting.GetObjType(objType);

                        assetInfo.assetName          = assetName;
                        assetInfo.assetBundleName    = assetBundleName;

                        assetInfo.isConfig = AssetManagerSetting.IsConfigFile(filename);



                    }
                }
            }

        }

        // path;objType
        private void ParseInfoResources(string p)
        {
            string path, objType;
            string filename;


            using(StringReader stringReader = new StringReader(p))
            {
                while(stringReader.Peek() >= 0)
                {
                    string line = stringReader.ReadLine();

                    if (string.IsNullOrEmpty(line))
                        continue;


                    string[] seg = line.Split(';');
                    path = seg[0];
                    objType = seg.Length > 1 ? seg[1] : string.Empty;

                    filename = path;
                    filename = filename.ToLower();



                    AssetInfo assetInfo;
                    if(!assetInfoDict.TryGetValue(filename, out assetInfo))
                    {
                        assetInfo = new AssetInfo();
                        assetInfo.name = filename;
                        assetInfoDict.Add(filename, assetInfo);
                    }

                    assetInfo.path = path;
                    assetInfo.loadType = AssetLoadType.Resources;
                    assetInfo.objType = AssetManagerSetting.GetObjType(objType);

                    assetInfo.isConfig = AssetManagerSetting.IsConfigFile(filename);
                }
            }
        }


        //-----------------------------------

        Type tmpType = typeof(System.Object);

       

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
            Load(filename, callback, callbackArgs, tmpType);
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
           
            if(AssetManagerSetting.IsConfigFile(filename))
            {
                LoadConfig(filename, callback, callbackArgs);
                return;
            }


            string filenameLower = filename.ToLower();
            AssetInfo fileInfo;
            if(!assetInfoDict.TryGetValue(filenameLower, out fileInfo))
            {
                Debug.LogError("[AssetMananger]资源配置不存在或者加载出错 name="+filenameLower + "   assetInfo=" + fileInfo );
                if (AssetManagerSetting.IsStrict == false &&  callback != null && fileInfo == null)
                {
                    System.Object obj = Resources.Load(filename, type);
                    if (obj != null)
                    {
                        callback(filenameLower, obj, callbackArgs);
                        return;
                    }
                }

                if(callback != null) callback(filenameLower, null, callbackArgs);
                return;
            }

            if(fileInfo.objType != null &&  (type == null || type == typeof(System.Object)))
            {
                type = fileInfo.objType;
            }


            if(fileInfo.loadType == AssetLoadType.AssetBundle)
            {
                LoadAssetAsync(fileInfo.assetBundleName, fileInfo.assetName, type, callback, callbackArgs);
            }
            else
            {
                LoadResourceAsync(fileInfo.path, type, callback, callbackArgs);
            }
        }

    }
}