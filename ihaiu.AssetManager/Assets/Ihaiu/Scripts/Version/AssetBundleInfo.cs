using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
    public class AssetBundleInfo 
    {
        public string path;
        public string assetBundleName;
        public string assetName;
        public string objType;

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3}", path, assetBundleName, assetName, objType);
        }
    }
}