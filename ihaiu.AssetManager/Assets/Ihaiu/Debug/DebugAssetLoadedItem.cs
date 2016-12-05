using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.ihaiu;


public class DebugAssetLoadedItem : MonoBehaviour
{
    public Text indexText;
    public Text referenceCountText;
    public Text pathText;

    public void SetLoadedResource(LoadedResource loaded, int index)
    {
        indexText.text = index + "";
        referenceCountText.text = loaded.referencedCount + "";
        pathText.text = "[" + loaded.objType + "]" +  loaded.path;
    }


    public void SetLoadedAssetBundle(string assetBundle, LoadedAssetBundle loaded, int index)
    {
        indexText.text = index + "";
        referenceCountText.text = loaded.m_ReferencedCount + "";
        pathText.text = assetBundle;
    }


	
}
