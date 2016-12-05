using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
	public abstract class AssetBundleLoadOperation : IEnumerator
	{
		public IAssetBundleManager      assetBundleManager;

		public object Current
		{
			get
			{
				return null;
			}
		}
		public bool MoveNext()
		{
			return !IsDone();
		}

		public void Reset()
		{
		}

		abstract public bool Update ();

		abstract public bool IsDone ();
	}
}