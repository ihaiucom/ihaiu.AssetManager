using UnityEngine;
using System.Collections;
using System;

namespace com.ihaiu
{
    public class LoadedResource
    {
        public string   path;
        public Type     objType;


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


        public UnityEngine.Object   obj;

        public LoadedResource(string path, Type objType)
        {
            this.path = path;
            this.objType = objType;
            this.referencedCount = 1;
        }
    }
}