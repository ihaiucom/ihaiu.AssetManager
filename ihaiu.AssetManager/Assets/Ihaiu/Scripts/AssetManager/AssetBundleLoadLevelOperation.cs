using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace com.ihaiu
{
	public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
	{
		protected string                m_AssetBundleName;
		protected string                m_LevelName;
		protected bool                  m_IsAdditive;
		protected string                m_DownloadingError;
		protected AsyncOperation        m_Request;

		public AssetBundleLoadLevelOperation (IAssetBundleManager assetBundleManager, string assetbundleName, string levelName, bool isAdditive)
		{
			this.assetBundleManager = assetBundleManager;
			m_AssetBundleName = assetbundleName;
			m_LevelName = levelName;
			m_IsAdditive = isAdditive;
		}

		public override bool Update ()
		{
			if (m_Request != null)
				return false;

            LoadedAssetBundle bundle = assetBundleManager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError, false);
			if (bundle != null)
			{
				if (m_IsAdditive)
					m_Request = SceneManager.LoadSceneAsync (m_LevelName, LoadSceneMode.Additive);
				else
					m_Request = SceneManager.LoadSceneAsync (m_LevelName, LoadSceneMode.Single);
				return false;
			}
			else
				return true;
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