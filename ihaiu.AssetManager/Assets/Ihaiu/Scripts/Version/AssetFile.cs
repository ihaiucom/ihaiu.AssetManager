using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
    public class AssetFile 
    {
        public string path;
        public string md5;

        public AssetFile()
        {
        }

        public AssetFile(string path, string md5)
        {
            this.path = path;
            this.md5 = md5;
        }

        public override string ToString()
        {
            return string.Format("{0};{1}", path, md5);
        }
    }
}