using UnityEngine;
using System.Collections;

namespace com.ihaiu
{

	public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
	{
		protected string                m_AssetBundleName;
		protected string                m_AssetName;
		protected string                m_DownloadingError;
		protected System.Type           m_Type;
		protected AssetBundleRequest    m_Request = null;
		private float waitTime = 0;
		private bool waitTimeError = false;

		public AssetBundleLoadAssetOperationFull (IAssetBundleManager assetBundleManager, string bundleName, string assetName, System.Type type)
		{
			this.assetBundleManager = assetBundleManager;
			m_AssetBundleName = bundleName;
			m_AssetName = assetName;
			m_Type = type;
			waitTime = 0;
			waitTimeError = false;
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

			if (waitTimeError == false)
			{
				waitTime += Time.deltaTime;
                if (waitTime > AssetManagerSetting.LoadTimeOut)
				{
					waitTimeError = true;
                    Debug.LogErrorFormat("AssetBundleLoadAssetOperationFull Update 加载超时 {0} WaitLoadResDependencies={1}", ToString(), assetBundleManager.GetWaitLoadResDependencies(m_AssetBundleName).ToStr());
				}
			}

            LoadedAssetBundle bundle = assetBundleManager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError, waitTimeError);
			if (bundle != null)
			{
				///@TODO: When asset bundle download fails this throws an exception...
				m_Request = bundle.m_AssetBundle.LoadAssetAsync (m_AssetName, m_Type);
				if (m_Request == null)
				{
					Debug.LogErrorFormat ("AssetBundleLoadAssetOperationFull Update m_Request = null m_AssetBundleName={0}, m_AssetName={1}, m_Type={2}", m_AssetBundleName, m_AssetName, m_Type);
				}
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

		public override string ToString ()
		{
			return string.Format ("[AssetBundleLoadAssetOperationFull] m_AssetBundleName={0}, m_AssetName={1}, m_Type={2}, m_Request={3}, m_DownloadingError={4}",
				m_AssetBundleName, m_AssetName, m_Type, m_Request, m_DownloadingError
			);
		}
	}

}