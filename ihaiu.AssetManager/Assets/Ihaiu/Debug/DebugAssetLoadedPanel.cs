using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using com.ihaiu;

public class DebugAssetLoadedPanel : MonoBehaviour {

    public float itemHeight = 80;
    public DebugAssetLoadedItem prefabItem;
    public RectTransform content;

    List<DebugAssetLoadedItem> items = new List<DebugAssetLoadedItem>();

    public int tabIndex = 0;
    public void SetTab(int index)
    {
        tabIndex = index;
        ShowList();
    }


    void OnEnable()
    {
        ShowList();
    }


    public void ShowList()
    {
        if (tabIndex == 0)
        {
            ShowList_Resouce();
        }
        else
        {
            ShowList_AssetBundle();
        }
    }

    public void ShowList_AssetBundle()
    {
        int index = 0;

        Dictionary<string, LoadedAssetBundle>   LoadedAssetBundles = AssetManager.Instance.LoadedAssetBundles;
        if (LoadedAssetBundles != null)
        {
            foreach (var kvp in LoadedAssetBundles)
            {
                DebugAssetLoadedItem item;
                if (index < items.Count)
                {
                    item = items[index];
                }
                else
                {
                    GameObject go = GameObject.Instantiate(prefabItem.gameObject);
                    item = go.GetComponent<DebugAssetLoadedItem>();
                    item.transform.SetParent(content, false);
                    items.Add(item);
                }
                item.gameObject.SetActive(true);

                item.SetLoadedAssetBundle(kvp.Key, kvp.Value, index);
                RectTransform rectTransform = (RectTransform)item.transform;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -itemHeight * (index + 1));
                index++;
            }
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, itemHeight * index);


        for(int i = index; i < items.Count; i ++)
        {
            DebugAssetLoadedItem item = items[i];
            item.gameObject.SetActive(false);
        }
    }

    public void ShowList_Resouce()
    {
        Dictionary<Type, Dictionary<string, LoadedResource>>   LoadedResources = AssetManager.Instance.LoadedResources;
        int index = 0;
        foreach(var kvp in LoadedResources)
        {
            foreach(var itemKVP in kvp.Value)
            {
                DebugAssetLoadedItem item;
                if (index < items.Count)
                {
                    item = items[index];
                }
                else
                {
                    GameObject go = GameObject.Instantiate(prefabItem.gameObject);
                    item = go.GetComponent<DebugAssetLoadedItem>();
                    item.transform.SetParent(content, false);
                    items.Add(item);
                }
                item.gameObject.SetActive(true);

                item.SetLoadedResource(itemKVP.Value, index);
                RectTransform rectTransform = (RectTransform) item.transform;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -itemHeight * (index + 1));
                index++;

            }
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, itemHeight * index);

        for(int i = index ; i < items.Count; i ++)
        {
            DebugAssetLoadedItem item = items[i];
            item.gameObject.SetActive(false);
        }
    }


    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }


}
