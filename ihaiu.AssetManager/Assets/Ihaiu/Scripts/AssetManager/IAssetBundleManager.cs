using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.ihaiu
{
	public interface IAssetBundleManager
	{
		// 资源管理
		AssetManager assetManager {get; set;}
		// 资源包清单
		AssetBundleManifest assetBundleManifest {get; set;}

		/** 获取等待加载的依赖文件 */
		List<string> GetWaitLoadResDependencies(string assetBundleName);

		/** 获取“资源包信息”、检测加载状态 */
        LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error, bool isTryForceGetBundle);

		/** 卸载资源包和他依赖的资源包 */
		void UnloadAssetBundle(string assetBundleName);

		/** 更新检测状态 */
		void Update();

	}
}
