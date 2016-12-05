#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace com.ihaiu
{
    public partial class AssetManager 
    {
        public static T EditorLoad<T>(string path) where T : UnityEngine.Object
        {
            string ext = Path.GetExtension(path);
            string assetPath = AssetManagerSetting.EditorRootMResources + "/" + path;
            if (string.IsNullOrEmpty (ext))
            {
                if(typeof(T) == typeof(GameObject))
                {
                    ext = ".prefab";
                }
                else if(typeof(T) == typeof(Sprite) || typeof(T) == typeof(Texture2D))
                {
                    ext = ".png";
                    if (!File.Exists (assetPath + ext)) 
                    {
                        ext = ".jpeg";
                    }

                    if (!File.Exists (assetPath + ext)) 
                    {
                        ext = ".jpg";
                    }
                }

                assetPath = PathUtil.ChangeExtension (assetPath, ext);
            }


            return AssetDatabase.LoadAssetAtPath<T> (assetPath);
        }
    }
}
#endif