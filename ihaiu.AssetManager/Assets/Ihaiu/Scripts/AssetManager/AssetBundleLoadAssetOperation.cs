using UnityEngine;
using System.Collections;

namespace Ihaiu.Assets
{
    public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
    {
        public abstract T GetAsset<T>() where T : UnityEngine.Object;
    }
}