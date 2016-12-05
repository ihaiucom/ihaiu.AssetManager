using UnityEngine;
using System.Collections;

using System.IO;
using System.Security.Cryptography;
using System.Text;
using com.ihaiu;

namespace Games
{
    public class GameLaunch : MonoBehaviour
    {
        public LaunchPanel launchPanel;
        void Awake()
        {
            launchPanel = GameObject.Find("LaunchPanel").GetComponent<LaunchPanel>();
            DontDestroyOnLoad(gameObject);
            StartCoroutine(Initialize());
        }

        IEnumerator Initialize()
        {
            Game.assetManager = gameObject.AddComponent<AssetManager>();

            VersionManager versionManager = gameObject.AddComponent<VersionManager>();
            launchPanel.Show(versionManager);
            yield return versionManager.CheckVersion();
            if (versionManager.yieldbreak)
                yield break;
            
            yield return Game.assetManager.Initialize();

            #if UNITY_EDITOR
            if (!GameConst.DevelopMode)
            #endif
            {
                yield return InitConfig();
            }

            GameObject.Destroy(launchPanel.gameObject);
            launchPanel = null;
            TestModeule();

        }

        IEnumerator InitConfig()
        {
            string path = AssetManagerSetting.ConfigAssetBundleURL;
            WWW www = new WWW(path);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogErrorFormat("[GameLaunch.InitConfig] path={0}, www.error={1}", path, www.error);
                yield break;
            }


            byte[] bytes = www.bytes;
            www.Dispose();
            www = null;

            bytes = DecryptBytes(bytes);

            AssetBundleCreateRequest assetBundleCreateRequest = LoadFromMemoryAsync(bytes);
            yield return assetBundleCreateRequest;
            AssetBundle assetBundle = assetBundleCreateRequest.assetBundle;
            Game.assetManager.configAssetBundle = assetBundle;
        }
       

        AssetBundleCreateRequest LoadFromMemoryAsync(byte[] bytes)
        {
            #if UNITY_5_2
            return AssetBundle.CreateFromMemory(bytes);
            #else
            return AssetBundle.LoadFromMemoryAsync(bytes);
            #endif
        }



        private byte[] DecryptBytes(byte[] data, string sKey = null)
        {
            if (sKey == null) sKey = "zengfeng";

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateDecryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return result;
        }


        void TestModeule()
        {
            Game.assetManager.Load("modules/ModuleA", OnLoadModule);
            Game.assetManager.Load("modules/ModuleB", OnLoadModule);
        }

        void OnLoadModule(string filename, object obj)
        {
            Debug.Log("filename=" + filename + "   obj=" + obj);
            GameObject go = GameObject.Instantiate((GameObject)obj);
            RectTransform canvas = (RectTransform) GameObject.Find("Canvas").transform;
            go.transform.SetParent(canvas, false);
        }

    }
}
