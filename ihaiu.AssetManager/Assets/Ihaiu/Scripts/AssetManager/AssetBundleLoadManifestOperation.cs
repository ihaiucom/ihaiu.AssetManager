using UnityEngine;
using System.Collections;

namespace Ihaiu.Assets
{
    public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
    {
        public AssetBundleLoadManifestOperation (string bundleName, string assetName, System.Type type)
            : base(bundleName, assetName, type)
        {
        }

        public override bool Update ()
        {
            base.Update();

            if (m_Request != null && m_Request.isDone)
            {
                assetBundleManager.assetBundleManifest = GetAsset<AssetBundleManifest>();
                return false;
            }
            else
                return true;
        }
    }
}