using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {


        /// <summary>
        /// Lua加载
        /// </summary>
        /// <param name="table">模块Table.</param>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源).</param>
//        public void LuaLoad(LuaTable table, string filename, LuaFunction callback)
//        {
//
//            if(string.IsNullOrEmpty(filename))
//            {
//                Debug.Log(string.Format("<color=red>AssetManager.LuaLoad filename={0}</color>", filename));
//            }
//
//            LuaCallback luaCB = new LuaCallback(table, callback);
//            Load(filename,  luaCB.AssetLoadCallback);
//        }



        /// <summary>
        /// Lua同步加载
        /// </summary>
        /// <param name="table">模块Table.</param>
        /// <param name="filename">文件名.</param>
        /// <param name="callback">回调函数(文件名, 资源).</param>
//        public void LuaLoadSync(LuaTable table, string filename, LuaFunction callback)
//        {
//
//            if(string.IsNullOrEmpty(filename))
//            {
//                Debug.Log(string.Format("<color=red>AssetManager.LuaLoadSync filename={0}</color>", filename));
//            }
//
//            System.Object obj = LoadSync(filename, tmpObjType);
//
//
//            if (callback != null)
//            {
//                callback.Call(table, name, obj);
//                callback.Dispose();
//            }
//
//            if(table != null) table.Dispose();
//
//            callback = null;
//            table = null;
//        }


    }
}