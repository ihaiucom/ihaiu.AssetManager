using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class HGUILayout
    {

        private static GUIStyle _boxMPStyle;
        public static GUIStyle boxMPStyle
        {
            get
            {
                if (_boxMPStyle == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                    style.padding = new RectOffset(15, 15, 15, 15);
                    style.margin = new RectOffset(20, 20, 5, 5);

                    _boxMPStyle = style;
                }
                return _boxMPStyle;
            }
        }



        private static GUIStyle _boxModuleStyle;
        public static GUIStyle boxModuleStyle
        {
            get
            {

                if (_boxModuleStyle == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                    style.padding = new RectOffset(20, 20, 20, 20);
                    style.margin = new RectOffset(5, 5, 5, 5);
                    _boxModuleStyle = style;
                }
                return _boxModuleStyle;
            }
        }

        private static GUIStyle _boxMiddleCenterStyle;
        public static GUIStyle boxMiddleCenterStyle
        {
            get
            {

                if (_boxMiddleCenterStyle == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                    style.alignment = TextAnchor.MiddleCenter;
                    _boxMiddleCenterStyle = style;
                }
                return _boxMiddleCenterStyle;
            }
        }


        private static GUIStyle _textFieldStyle_Normal;
        public static GUIStyle textFieldStyle_Normal
        {
            get
            {
                if (_textFieldStyle_Normal == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.miniTextField);
                    _textFieldStyle_Normal = style;
                }
                return _textFieldStyle_Normal;
            }
        }


        private static GUIStyle _textFieldStyle_Disable;
        public static GUIStyle textFieldStyle_Disable
        {
            get
            {
                if (_textFieldStyle_Disable == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.miniTextField);
                    style.normal.textColor = Color.gray;
                    _textFieldStyle_Disable = style;
                }
                return _textFieldStyle_Disable;
            }
        }


        private static GUIStyle _labelCenterStyle;
        public static GUIStyle labelCenterStyle
        {
            get
            {

                if (_labelCenterStyle == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 16;

                    _labelCenterStyle = style;
                }
                return _labelCenterStyle;
            }
        }

        private static GUIStyle _labelRichStyle;
        public static GUIStyle labelRichStyle
        {
            get
            {

                if (_labelRichStyle == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.richText = true;
                    _labelRichStyle = style;
                }
                return _labelRichStyle;
            }
        }


    }
}
