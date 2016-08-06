using UnityEngine;
using System.Collections;

namespace Ihaiu.Assets
{
    public abstract class AssetBundleLoadOperation : IEnumerator
    {
        public ManifestAssetBundleManager      assetBundleManager;

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