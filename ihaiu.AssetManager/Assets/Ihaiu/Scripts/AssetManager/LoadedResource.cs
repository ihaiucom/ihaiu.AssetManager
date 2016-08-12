using UnityEngine;
using System.Collections;
using System;

namespace Ihaiu.Assets
{
    public class LoadedResource
    {
        public string   path;
        public Type     objType;
        public int      referencedCount;

        public UnityEngine.Object   obj;

        public LoadedResource(string path, Type objType)
        {
            this.path = path;
            this.objType = objType;
            this.referencedCount = 1;
        }
    }
}