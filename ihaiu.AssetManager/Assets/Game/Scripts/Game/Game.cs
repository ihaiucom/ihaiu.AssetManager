using UnityEngine;
using System.Collections;
using Ihaiu.Assets;

namespace Games
{
    public class Game 
    {
        public static AssetManager assetManager;

        public static void Init()
        {
            assetManager = AssetManager.Instance;
        }
    }
}