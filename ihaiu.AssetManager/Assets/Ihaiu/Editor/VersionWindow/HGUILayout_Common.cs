using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class HGUILayout
    {
        public static void BeginCenterHorizontal()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box("", GUIStyle.none, GUILayout.ExpandWidth(true));
        }

        public static void EndCenterHorizontal()
        {

            GUILayout.Box("", GUIStyle.none, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }


        //---
        public static void BeginMiddleVertical(float height)
        {
            GUILayout.BeginVertical(GUILayout.Height(height));

            GUILayout.Box("", GUIStyle.none, GUILayout.ExpandHeight(true));
        }

        public static void EndMiddleVertical()
        {

            GUILayout.Box("", GUIStyle.none, GUILayout.ExpandHeight(true));
            GUILayout.EndVertical();
        }
    }
}
