using UnityEngine;
using System.Collections;

namespace Ihaiu.Assets
{

	public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
	{
		protected string                m_AssetBundleName;
		protected string                m_AssetName;
		protected string                m_DownloadingError;
		protected System.Type           m_Type;
		protected AssetBundleRequest    m_Request = null;

		public AssetBundleLoadAssetOperationFull (IAssetBundleManager assetBundleManager, string bundleName, string assetName, System.Type type)
		{
			this.assetBundleManager = assetBundleManager;
			m_AssetBundleName = bundleName;
			m_AssetName = assetName;
			m_Type = type;
		}

		public override T GetAsset<T>()
		{
			if (m_Request != null && m_Request.isDone)
				return m_Request.asset as T;
			else
				return null;
		}

		// Returns true if more Update calls are required.
		public override bool Update ()
		{
			if (m_Request != null)
				return false;

			LoadedAssetBundle bundle = assetBundleManager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError);
			if (bundle != null)
			{
				///@TODO: When asset bundle download fails this throws an exception...
				m_Request = bundle.m_AssetBundle.LoadAssetAsync (m_AssetName, m_Type);
				return false;
			}
			else
			{
				return true;
			}
		}

		public override bool IsDone ()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null && m_DownloadingError != null)
			{
				Debug.LogError(m_DownloadingError);
				return true;
			}

			return m_Request != null && m_Request.isDone;
		}
	}

}