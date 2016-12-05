using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
	public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
	{
		public abstract T GetAsset<T>() where T : UnityEngine.Object;


		public string key;
		public float lastTime = 0;
		private int _referencedCount = 0;
		public int referencedCount
		{
			get
			{
				return _referencedCount;
			}

			set
			{
				if (value != 0 && value > _referencedCount)
				{
					lastTime = Time.unscaledTime;
				}

				_referencedCount = value;
			}
		}
	}
}