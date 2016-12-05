using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class HGUILayout
    {
        private static GUIStyle _tabBoxStyle;
        protected static GUIStyle tabBoxStyle()
        {

            if (_tabBoxStyle == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                style.alignment = TextAnchor.MiddleCenter;
                _tabBoxStyle = style;
            }
            return _tabBoxStyle;
        }


        private static GUIStyle _tabStyle_Normal;
        protected static GUIStyle tabStyle_Normal()
        {

            if (_tabStyle_Normal == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.miniButtonMid);
                style.fontSize = 16;
                style.normal.textColor = Color.gray;
                style.margin = new RectOffset(0, 0, 0, 0);

                _tabStyle_Normal = style;
            }
            return _tabStyle_Normal;
        }

        private static GUIStyle _tabStyle_Select;
        protected static GUIStyle tabStyle_Select()
        {

            if (_tabStyle_Select == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.miniButtonMid);
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.white;
                style.normal.background = EditorStyles.miniButtonMid.active.background;
                style.margin = new RectOffset(0, 0, 0, 0);

                _tabStyle_Select = style;
            }
            return _tabStyle_Select;
        }



        public class TabGroupData<T>
        {
            public List<TabData<T>> list = new List<TabData<T>>();
            public T selectVal;

            public TabGroupData<T> AddTab(string label, T val)
            {
                list.Add(new TabData<T>(label, val, this));
                return this;
            }

            public TabGroupData<T> SetSelect(T selectVal)
            {
                this.selectVal = selectVal;
                return this;
            }

        }

        public class TabData<T>
        {
            public TabGroupData<T> group;
            public string label;
            public T val;

            public TabData(string label, T val, TabGroupData<T> group)
            {
                this.label = label;
                this.val = val;
                this.group = group;
            }

            public bool IsSelect
            {
                get
                {
                    return group.selectVal.Equals(val);
                }
            }
        }


        /** 标签按钮组 */
        public static T TabGroup<T>(TabGroupData<T> groupData)
        {

            GUILayout.BeginHorizontal();
            for(int i = 0; i < groupData.list.Count; i ++)
            {
                TabData<T> tabData = groupData.list[i];

                bool isClick = false;
                if (tabData.IsSelect)
                {
                    isClick = GUILayout.Button(tabData.label, tabStyle_Select(), GUILayout.MinHeight(50));
                }
                else
                {
                    isClick = GUILayout.Button(tabData.label, tabStyle_Normal(), GUILayout.MinHeight(50));
                }

                if (isClick)
                {
                    groupData.SetSelect(tabData.val);
                }


               
            }
            GUILayout.EndHorizontal();

            return groupData.selectVal;
        }
    }
}
