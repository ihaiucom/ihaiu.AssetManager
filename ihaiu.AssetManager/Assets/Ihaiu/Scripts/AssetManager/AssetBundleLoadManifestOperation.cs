using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
	public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
	{
		public AssetBundleLoadManifestOperation (IAssetBundleManager assetBundleManager, string bundleName, string assetName, System.Type type)
			: base(assetBundleManager, bundleName, assetName, type)
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