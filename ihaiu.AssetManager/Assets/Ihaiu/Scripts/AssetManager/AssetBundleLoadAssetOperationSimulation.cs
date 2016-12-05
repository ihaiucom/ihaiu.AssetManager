using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
	public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
	{
		Object                          m_SimulatedObject;

		public AssetBundleLoadAssetOperationSimulation (Object simulatedObject)
		{
			m_SimulatedObject = simulatedObject;
		}

		public override T GetAsset<T>()
		{
			return m_SimulatedObject as T;
		}

		public override bool Update ()
		{
			return false;
		}

		public override bool IsDone ()
		{
			return true;
		}
	}
}