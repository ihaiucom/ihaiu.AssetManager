using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class HGUILayout
    {
       



        private static GUIStyle _versionStyle_Box;
        protected static GUIStyle VersionStyle_Box()
        {

            if (_versionStyle_Box == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                style.padding = new RectOffset(10, 10, 10, 10);
                style.margin = new RectOffset(20, 0, 5, 5);

                _versionStyle_Box = style;
            }
            return _versionStyle_Box;
        }

        private static GUIStyle _versionStyle_Field;
        protected static GUIStyle VersionStyle_Field()
        {

            if (_versionStyle_Field == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 10;
                style.richText = true;

                _versionStyle_Field = style;
            }
            return _versionStyle_Field;
        }

        private static GUIStyle _versionStyle_LabelName;
        protected static GUIStyle VersionStyle_LabelName()
        {

            if (_versionStyle_LabelName == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 16;

                _versionStyle_LabelName = style;
            }
            return _versionStyle_LabelName;
        }

        private static GUIStyle _versionStyle_LabelVer;
        protected static GUIStyle VersionStyle_LabelVer()
        {

            if (_versionStyle_LabelVer == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 10;
                style.richText = true;
                _versionStyle_LabelVer = style;
            }
            return _versionStyle_LabelVer;
        }




        /** 版本号编辑器 */
        public static void Version(string label, Version version, VersionType verType)
        {

            int cellWidth = 80;
            GUIStyle fieldStyle = VersionStyle_Field();
            string fieldFormat = "<color=#808080ff>{0}</color>";


            GUILayout.BeginHorizontal(VersionStyle_Box());


            int infoWidth = 250;
            GUILayout.BeginVertical(GUILayout.Width(infoWidth));
            GUILayout.Label(label, VersionStyle_LabelName(), GUILayout.Width(infoWidth));
            GUILayout.Label(string.Format("<color=#808080ff>({0})</color>", version), VersionStyle_LabelVer(), GUILayout.Width(infoWidth));
            GUILayout.EndVertical();






            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format(fieldFormat, "主版本号"), fieldStyle, GUILayout.Width(cellWidth));
            GUILayout.Label(string.Format(fieldFormat, "次版本号"), fieldStyle, GUILayout.Width(cellWidth));
            GUILayout.Label(string.Format(fieldFormat, "修订版"), fieldStyle, GUILayout.Width(cellWidth));
            GUILayout.Label(string.Format(fieldFormat, "阶段"), fieldStyle, GUILayout.Width(cellWidth));
            GUILayout.EndHorizontal();

            int master = version.master;
            int minor = version.minor;
//            int revised = version.revised;

            if (verType == VersionType.App)
            {
                GUILayout.BeginHorizontal();
                version.master = Convert.ToInt32(GUILayout.TextField(string.Format("{0}", version.master), GUILayout.Width(cellWidth)));
                version.minor = Convert.ToInt32(GUILayout.TextField(string.Format("{0}", version.minor), GUILayout.Width(cellWidth)));
				version.revised = Convert.ToInt32(GUILayout.TextField(string.Format("{0}", version.revised), GUILayout.Width(cellWidth)));
                version.stages = (VersionStages)EditorGUILayout.EnumPopup(version.stages, GUILayout.Width(cellWidth));
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                Convert.ToInt32(GUILayout.TextField(string.Format("{0}", version.master), textFieldStyle_Disable, GUILayout.Width(cellWidth)));
                Convert.ToInt32(GUILayout.TextField(string.Format("{0}", version.minor), textFieldStyle_Disable, GUILayout.Width(cellWidth)));
                version.revised = Convert.ToInt32(GUILayout.TextField(string.Format("{0}", version.revised), GUILayout.Width(cellWidth)));
                version.stages = (VersionStages)EditorGUILayout.EnumPopup(version.stages, GUILayout.Width(cellWidth));
                GUILayout.EndHorizontal();
            }

            if (master != version.master)
            {
                version.minor       = 0;
                version.revised     = 0;
            }

            if (minor != version.minor)
            {
                version.revised     = 0;
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

    }
}
