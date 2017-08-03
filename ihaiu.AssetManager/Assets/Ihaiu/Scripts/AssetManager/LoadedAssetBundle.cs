using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
    public class LoadedAssetBundle
    {
        public AssetBundle m_AssetBundle;
        public int m_ReferencedCount;
		public string url;

		public LoadedAssetBundle(AssetBundle assetBundle, string url)
        {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = 1;
			this.url = url;
        }
    }
}