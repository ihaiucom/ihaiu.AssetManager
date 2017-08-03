using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager : MonoBehaviour 
    {
        
        public void InitializeSync()
        {
            InitManifestSync();
            ReadFilesSync();

            CheckCache();
        }

    }
}