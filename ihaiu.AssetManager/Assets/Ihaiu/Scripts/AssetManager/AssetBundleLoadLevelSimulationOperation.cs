#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
	public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation
	{
		AsyncOperation m_Operation = null;


		public AssetBundleLoadLevelSimulationOperation (string assetBundleName, string levelName, bool isAdditive)
		{
			string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
			if (levelPaths.Length == 0)
			{
				///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
				//        from that there right scene does not exist in the asset bundle...

				Debug.LogError("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
				return;
			}

			if (isAdditive)
				m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
			else
				m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
		}

		public override bool Update ()
		{
			return false;
		}

		public override bool IsDone ()
		{       
			return m_Operation == null || m_Operation.isDone;
		}
	}
}
#endif