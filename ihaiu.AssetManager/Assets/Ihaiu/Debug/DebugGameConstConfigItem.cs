using UnityEngine;
using System.Collections;
using Games;
using UnityEngine.UI;
using System.Reflection;

public class DebugGameConstConfigItem : MonoBehaviour 
{
    public FieldInfo fieldInfo;
    public string key;
    public object value;
    public Text             label;
    public InputField       input;
    public Toggle           toggle;

    public void SetObject(string key, object value)
    {
        this.key = key;
        this.value = value;
        if (value is bool)
        {
            SetBool(key, (bool) value);
        }
        else
        {
            SetString(key, value.ToString());
        }
    }

    public void SetString(string key, string value)
    {
        label.text = key;
        input.text = value;

        input.gameObject.SetActive(true);
        toggle.gameObject.SetActive(false);
    }


    public void SetBool(string key, bool value)
    {
        label.text = key;
        toggle.isOn = value;

        input.gameObject.SetActive(false);
        toggle.gameObject.SetActive(true);
    }

    public string GetString()
    {
        return input.text;
    }

    public bool GetBool()
    {
        return toggle.isOn;
    }

    public object GetObject()
    {
        if (value is bool)
        {
            return GetBool();
        }
        else
        {
            return GetString();
        }
    }
}
