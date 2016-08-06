using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Ihaiu.Assets
{
    public partial class AssetManager : MonoBehaviour 
    {

        private static AssetManager _Instance;
        public static AssetManager Instance
        {
            get
            {
                if(_Instance == null)
                {
                    GameObject go = GameObject.Find("GameManagers");
                    if(go == null) go = new GameObject("GameManagers");

                    _Instance = go.GetComponent<AssetManager>();
                    if(_Instance == null) _Instance = go.AddComponent<AssetManager>();
                }
                return _Instance;
            }
        }

        void Awake()
        {
            _Instance = this;
            Initialize();
        }


        public void Initialize()
        {
            InitManifest();
//            StartCoroutine(ReadFiles());
        }


        #region Event Handling
        public delegate void OnPrepareFinal();

        public OnPrepareFinal prepareFinal;
        public void AddOnPrepareFinalDelegate(OnPrepareFinal call)
        {

            prepareFinal += call;
        }

        public void RemoveOnPrepareFinalDelegate(OnPrepareFinal call)
        {
            prepareFinal -= call;
        }


        public bool isPrepare = false;
        private void PrepareFinal()
        {
            isPrepare = true;
            if (prepareFinal != null)
            {
                prepareFinal();
            }
        }

        #endregion Event Handling


      




        public void Update () 
        {
            UpdateAssetBundle();
        }

    }
}