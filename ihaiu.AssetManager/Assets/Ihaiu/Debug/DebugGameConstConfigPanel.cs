using UnityEngine;
using System.Collections;
using Games;

using System;
using System.Reflection;
using System.Collections.Generic;
using com.ihaiu;

public class DebugGameConstConfigPanel : MonoBehaviour {
    public GameConstConfig config;
    public float itemHeight = 80;
    public DebugGameConstConfigItem prefabItem;
    public RectTransform content;

    List<DebugGameConstConfigItem> items = new List<DebugGameConstConfigItem>();


    void OnEnable()
    {
        Load();
    }


    public void Load()
    {
        config = GameConstConfig.Load(AssetManagerSetting.GameConstPath);

        Type type = typeof(GameConstConfig);


        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        for(int i = 0; i < fields.Length; i ++)
        {
            FieldInfo fieldInfo = fields[i];

            DebugGameConstConfigItem item;
            if (i < items.Count)
            {
                item = items[i];
            }
            else
            {
                GameObject go = GameObject.Instantiate(prefabItem.gameObject);
                item = go.GetComponent<DebugGameConstConfigItem>();
                item.transform.SetParent(content, false);
                items.Add(item);
                item.gameObject.SetActive(true);
            }

            item.fieldInfo = fieldInfo;
            item.SetObject(fieldInfo.Name, fieldInfo.GetValue(config));
            RectTransform rectTransform = (RectTransform) item.transform;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -itemHeight * i);
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, itemHeight * fields.Length);

      
    }

    public void Save()
    {
        for(int i = 0; i < items.Count; i ++)
        {
            DebugGameConstConfigItem item = items[i];
            item.fieldInfo.SetValue(config, item.GetObject());
        }
        config.Save(AssetManagerSetting.GameConstPath);
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
