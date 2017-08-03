using UnityEngine;
using System.Collections;
using System.IO;

namespace com.ihaiu
{
    public class AssetLoadSdk 
    {
        public static byte[] LoadBytes(string path)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            return AndroidAssetLoadSDK.LoadFile(path);
            #else
            return File.ReadAllBytes(path);
            #endif
        }

        public static string LoadText(string path)
        {
            byte[] bytes = LoadBytes(path);
            if (bytes == null)
                return null;

            return System.Text.Encoding.UTF8.GetString ( bytes );
        }

        public static AssetBundle LoadAssetBundle(string path)
        {
            byte[] bytes = LoadBytes(path);
            if (bytes == null)
                return null;

            return AssetBundle.LoadFromMemory(bytes);
        }

    }
}