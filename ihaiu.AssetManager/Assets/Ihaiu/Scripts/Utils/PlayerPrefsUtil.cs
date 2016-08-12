using UnityEngine;
using System.Collections;
using Games;

public class PlayerPrefsUtil 
{
	public static bool UseUserId = true;

	/// <summary>
	/// 生成一个Key名
	/// </summary>
    public static string GetKey(string key) {
        return GetKey(key, true);
	}

    public static string GetKey(string key, bool isBindUserId) {
        if (UseUserId && isBindUserId)
            return GameConst.AppPrefix + GameConst.UserId + "_" + key;
        else
            return GameConst.AppPrefix + "_" + key;
    }

	
	
	/// <summary>
	/// 有没有值
	/// </summary>
    public static bool HasKey(string key) {
        return HasKey(key, true);
	}

    public static bool HasKey(string key, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        return PlayerPrefs.HasKey(name);
    }


    /// <summary>
    /// 取得Bool
    /// </summary>
    public static bool GetBool(string key) {
        return GetBool(key, true);
    }

    public static bool GetBool(string key, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        return PlayerPrefs.GetInt(name) == 1;
    }

    /// <summary>
    /// 保存Bool
    /// </summary>
    public static void SetBool(string key, bool value) {
        SetBool(key, value, true);
    }

    public static void SetBool(string key, bool value, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.SetInt(name, value ? 1 : 0);
        PlayerPrefs.Save();
    }
	
	/// <summary>
	/// 取得整型
	/// </summary>
    public static int GetInt(string key) {
        return GetInt(key, true);
	}

    public static int GetInt(string key, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        return PlayerPrefs.GetInt(name);
    }
	
	/// <summary>
	/// 保存整型
	/// </summary>
    public static void SetInt(string key, int value) {
        SetInt(key, value, true);
	}

    public static void SetInt(string key, int value, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.SetInt(name, value);
        PlayerPrefs.Save();
    }

    /// <summary>
	/// 取得整型，不通过用户id获取key
	/// </summary>
    public static int GetIntSimple(string key)
    {
		//Debug.Log("=====key:" + key);
        return PlayerPrefs.GetInt(key);
    }

    /// <summary>
    /// 保存整型，不通过用户id获取key
    /// </summary>
    public static void SetIntSimple(string key, int value)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }


    /// <summary>
    /// 取得整型
    /// </summary>
    public static float GetFloat(string key) {
        return GetFloat(key, true);
	}

    public static float GetFloat(string key, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        return PlayerPrefs.GetFloat(name);
    }
	
	/// <summary>
	/// 保存整型
	/// </summary>
    public static void SetFloat(string key, float value) {
        SetFloat(key, value, true);
	}

    public static void SetFloat(string key, float value, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.SetFloat(name, value);
        PlayerPrefs.Save();
    }

	
	/// <summary>
	/// 取得数据
	/// </summary>
    public static string GetString(string key) {
        return GetString(key, true);
	}

    public static string GetString(string key, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        return PlayerPrefs.GetString(name);
    }
	
	/// <summary>
	/// 保存数据
	/// </summary>
    public static void SetString(string key, string value) {
        SetString(key, value, true);
	}

    public static void SetString(string key, string value, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.SetString(name, value);
        PlayerPrefs.Save();
    }
	
	/// <summary>
	/// 删除数据
	/// </summary>
    public static void RemoveData(string key) {
        RemoveData(key, true);
	}

    public static void RemoveData(string key, bool isBindUserId) {
        string name = GetKey(key, isBindUserId);
        PlayerPrefs.DeleteKey(name);
    }

	/// <summary>
	/// 删除所有数据
	/// </summary>
	public static void RemoveAllData() {
		PlayerPrefs.DeleteAll();
	}

}
