using UnityEngine;
using System.Collections;

public class AndroidAssetLoadSDK 
{
    public static byte[] LoadFile(string path)
    {
        AndroidJavaClass    m_AndroidJavaClass = new AndroidJavaClass("com.ihaiu.assetloadsdk.AssetLoad");
        return m_AndroidJavaClass.CallStatic<byte[]>("loadFile", path);
    }
}
