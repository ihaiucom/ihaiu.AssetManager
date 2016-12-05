using UnityEngine;
using System.Collections;
using System;

namespace com.ihaiu
{
    public enum AssetLoadType
    {
        AssetBundle,
        Resources
    }

    public class AssetInfo 
    {
        /** 加载方式 */
        public AssetLoadType loadType;

        /** 名称 */
        public string name;
        /** 路径 */
        public string path;
        /** 获取资源类型 */
        public Type objType;


        public string assetBundleName;
        public string assetName;


        public bool isConfig;
    }
}