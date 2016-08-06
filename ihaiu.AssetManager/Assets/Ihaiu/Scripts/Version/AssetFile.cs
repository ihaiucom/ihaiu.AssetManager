using UnityEngine;
using System.Collections;

namespace Ihaiu.Assets
{
    public class AssetFile 
    {
        public string path;
        public string md5;

        public override string ToString()
        {
            return string.Format("{0};{1}", path, md5);
        }
    }
}