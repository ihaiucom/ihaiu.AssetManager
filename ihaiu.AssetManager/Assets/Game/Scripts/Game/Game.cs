using UnityEngine;
using System.Collections;
using Ihaiu.Assets;

namespace Games
{
    public class Game : MonoBehaviour
    {
        public static AssetManager assetManager;



        void Awake()
        {
            assetManager = gameObject.AddComponent<AssetManager>();
        }
    }
}