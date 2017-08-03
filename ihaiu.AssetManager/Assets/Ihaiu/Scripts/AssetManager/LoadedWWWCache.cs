using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public class LoadedWWWCache
    {
        public string url;
        public Type objType;

        public int createCount;
        public float lastTime = 0;

        private int _referencedCount = 0;
        public int referencedCount
        {
            get
            {
                return _referencedCount;
            }

            set
            {
                if (value != 0 && value > _referencedCount)
                {
                    lastTime = Time.unscaledTime;
                }

                _referencedCount = value;
            }
        }

        public object obj;
        public List<CallbackStruct> callbacks = new List<CallbackStruct>();
        public void CallCallbacks()
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].Call(obj);
                callbacks[i].Clear();
            }
            callbacks.Clear();
        }

        public LoadedWWWCache(string url, Type objType)
        {

            this.url = url;
            this.objType = objType;
            this.referencedCount = 1;
            this.createCount = 1;

        }



        public void UnloadObj()
        {
            
            if (obj != null)
            {
                if (obj is UnityEngine.GameObject)
                {
                    UnityEngine.Object o = (UnityEngine.GameObject) obj;
                    if (objType == AssetManagerSetting.tmpGameObjectType)
                    {
                        UnityEngine.Object.DestroyImmediate(o, true);
                    }
                    else
                    {
                        Resources.UnloadAsset(o);
                    }
                }
                obj = null;
            }

            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].Clear();
            }
            callbacks.Clear();
        }

        public static int Compare(LoadedWWWCache a, LoadedWWWCache b)
        {
            if (a.obj == null)
                return -1;

            if (a.lastTime > b.lastTime)
                return 1;
            else if (b.lastTime > a.lastTime)
                return -1;
            
            return 0;
        }


    }
}