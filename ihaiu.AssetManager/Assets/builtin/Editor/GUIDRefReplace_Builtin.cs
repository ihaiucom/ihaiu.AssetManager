using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// 解决项目中 一样的资源（名字或者路径不同）存在两份的问题  （多人做UI出现的问题， 或者美术没有管理好资源）
/// 如果是要替换资源的话， 那就直接替换好了
/// 
/// 以上可以这么操作的基础是，你的Unity项目内的.prefab .Unity 都可以直接用文本开看到数据，而不是乱码（二进制）。这一步很关键，怎么设置呢？
/// 打开项目Unity编辑器：Edit —-> Project Settings —-> Editor 这样就会调到你的Inspector面板的Editor Settings 
/// 设置 Asset Serialization 的Mode类型为：Force Text(默认是Mixed); 这样你就能看到你的prefab文件引用了哪些贴图，字体，prefab 等资源了
/// </summary>
public class GUIDRefReplace_Builtin : EditorWindow
{


    #region style
    private static GUIStyle _boxStyle;
    public static GUIStyle boxStyle
    {
        get
        {

            if (_boxStyle == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                style.padding = new RectOffset(20, 20, 20, 20);
                style.margin = new RectOffset(5, 5, 5, 5);
                _boxStyle = style;
            }
            return _boxStyle;
        }
    }

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
    #endregion




    private static GUIDRefReplace_Builtin _window;
    private Object _sourceOld;
    private Object _sourceNew;

    private string _oldGuid;
    private string _newGuid;

    private bool isContainScene = true;
    private bool isContainPrefab = true;
    private bool isContainMat = true;
    private bool isContainAsset = false;

    private List<string> withoutExtensions = new List<string>();

    private bool isPreview = false;


    [MenuItem("Window/GUIDRefReplaceWin (Builitin) ")]   // 菜单开启并点击的   处理
    public static void GUIDRefReplaceWin()
    {

        // true 表示不能停靠的
        _window = (GUIDRefReplace_Builtin)EditorWindow.GetWindow(typeof(GUIDRefReplace_Builtin), true, "Builitin 引用替换 (●'◡'●)");
        _window.minSize = new Vector2(760, 550);
        _window.Show();

    }



    public enum TabType
    {
        Material,
        Sprite,
        Texture,
        Shader,
    }


    HGUILayout.TabGroupData<TabType> _tabGroupData;
    HGUILayout.TabGroupData<TabType> tabGroupData
    {
        get
        {
            if (_tabGroupData == null)
            {
                _tabGroupData = new HGUILayout.TabGroupData<TabType>();
                _tabGroupData.AddTab("Material", TabType.Material);
                _tabGroupData.AddTab("Sprite", TabType.Sprite);
                _tabGroupData.AddTab("Texture", TabType.Texture);
                _tabGroupData.AddTab("Shader", TabType.Shader);
            }
            return _tabGroupData;
        }
    }


    public TabType tabType = TabType.Material;
    Vector2 scrollPos = Vector2.zero;
    void OnGUI()
    {

        tabType = HGUILayout.TabGroup<TabType>(tabGroupData);


        // 要被替换的（需要移除的）
        GUILayout.Space(20);

        switch (tabType)
        {

            case TabType.Material:
            case TabType.Sprite:
                // 在那些类型中查找（.unity\.prefab\.mat）
                GUILayout.Label("要在哪些类型中查找替换：");
                EditorGUILayout.BeginHorizontal();


                isContainScene = GUILayout.Toggle(isContainScene, ".unity");
                isContainPrefab = GUILayout.Toggle(isContainPrefab, ".prefab");
                isContainMat = tabType == TabType.Material ? false : GUILayout.Toggle(isContainMat, ".mat");
                isContainAsset = GUILayout.Toggle(isContainAsset, ".asset");

                EditorGUILayout.EndHorizontal();
                break;
            }


        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Space(20);
        switch (tabType)
        {
            case TabType.Material:
                GUI_MaterialList();
                break;
            case TabType.Sprite:
                GUI_SpriteList();
                break;
            case TabType.Texture:
                GUI_SpriteList();
                break;
            case TabType.Shader:
                if (GUILayout.Button("替换内建Shader!"))
                {
                    RefreshMaterialShader.RefreshMat();
                }
                break;
        }



        GUILayout.Space(20);
        EditorGUILayout.EndScrollView();
    }



    string Sprite_GUID_NEW = "{fileID: 10905, guid: __GUID__, type: 0}";

    void GUI_SpriteList()
    {
        
    }

    void GUI_SpriteItem(string spriteName, Material newMaterial, string oladGuid)
    {
        EditorGUILayout.BeginVertical(boxStyle);

        newMaterial = (Material) EditorGUILayout.ObjectField(spriteName, newMaterial, typeof(Material), true);

        string newGuid = "";
        if (newMaterial != null)
        {
            newGuid = MATERIAL_GUID_NEW.Replace("__GUID__", AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newMaterial)));
        }

        GUILayout.Space(10);
        oladGuid = EditorGUILayout.TextField("old:", oladGuid);
        GUILayout.Space(5);
        newGuid = EditorGUILayout.TextField("new:", newGuid);


        GUILayout.Space(10);
        if (GUILayout.Button("查找预览!"))
        {
            OnClickReplace(oladGuid, newGuid, true);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("开始替换!"))
        {
            OnClickReplace(oladGuid, newGuid, false);
        }


        EditorGUILayout.EndVertical();
    }






    string MATERIAL_GUID_NEW = "- {fileID: 2100000, guid: __GUID__, type: 2}";


    Material    Sprites_Default;
    Material    Default_Material;
    Material    Default_Diffuse;
    Material    Default_Particle;
    Material    Default_Skybox;

    string      Sprites_Default_old         = "- {fileID: 10754, guid: 0000000000000000f000000000000000, type: 0}";
    string      Default_Material_old        = "- {fileID: 10303, guid: 0000000000000000f000000000000000, type: 0}";
    string      Default_Diffuse_old         = "- {fileID: 10302, guid: 0000000000000000f000000000000000, type: 0}";
    string      Default_Particle_old        = "- {fileID: 10301, guid: 0000000000000000f000000000000000, type: 0}";
    string      Default_Skybox_old          = "- {fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}";


    void GUI_MaterialList()
    {
        if (Sprites_Default == null)
        {
            Sprites_Default = AssetDatabase.LoadAssetAtPath<Material>("Assets/builtin/builtin_materials/Sprites-Default.mat");
        }

        if (Default_Material == null)
        {
            Default_Material = AssetDatabase.LoadAssetAtPath<Material>("Assets/builtin/builtin_materials/Default-Material.mat");
        }

        if (Default_Diffuse == null)
        {
            Default_Diffuse = AssetDatabase.LoadAssetAtPath<Material>("Assets/builtin/builtin_materials/Default-Diffuse.mat");
        }

        if (Default_Particle == null)
        {
            Default_Particle = AssetDatabase.LoadAssetAtPath<Material>("Assets/builtin/builtin_materials/Default-Particle.mat");
        }

        if (Default_Skybox == null)
        {
            Default_Skybox = AssetDatabase.LoadAssetAtPath<Material>("Assets/builtin/builtin_materials/Default-Skybox.mat");
        }



        GUI_MaterialItem("Sprites_Default", Sprites_Default, Sprites_Default_old);
        GUILayout.Space(20);
        GUI_MaterialItem("Default_Material", Default_Material, Default_Material_old);
        GUILayout.Space(20);
        GUI_MaterialItem("Default_Diffuse", Default_Diffuse, Default_Diffuse_old);
        GUILayout.Space(20);
        GUI_MaterialItem("Default_Particle", Default_Particle, Default_Particle_old);
        GUILayout.Space(20);
        GUI_MaterialItem("Default_Skybox", Default_Skybox, Default_Skybox_old);
    }

    void GUI_MaterialItem(string masterName, Material newMaterial, string oladGuid)
    {
        EditorGUILayout.BeginVertical(boxStyle);

        newMaterial = (Material) EditorGUILayout.ObjectField(masterName, newMaterial, typeof(Material), true);

        string newGuid = "";
        if (newMaterial != null)
        {
            newGuid = MATERIAL_GUID_NEW.Replace("__GUID__", AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newMaterial)));
        }

        GUILayout.Space(10);
        oladGuid = EditorGUILayout.TextField("old:", oladGuid);
        GUILayout.Space(5);
        newGuid = EditorGUILayout.TextField("new:", newGuid);


        GUILayout.Space(10);
        if (GUILayout.Button("查找预览!"))
        {
            OnClickReplace(oladGuid, newGuid, true);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("开始替换!"))
        {
            OnClickReplace(oladGuid, newGuid, false);
        }


        EditorGUILayout.EndVertical();
    }





    private void OnClickReplace(string _oldGuid, string _newGuid, bool isPreview)
    {
        this._oldGuid = _oldGuid;
        this._newGuid = _newGuid;
        this.isPreview = isPreview;

        if (Check())
        {
            StartReplace();
        }
    }

    bool Check()
    {
        if (EditorSettings.serializationMode != SerializationMode.ForceText)
        {
            Debug.LogError("需要设置序列化模式为 SerializationMode.ForceText");
            ShowNotification(new GUIContent("需要设置序列化模式为 SerializationMode.ForceText"));
        }
        else if (!isContainScene && !isContainPrefab && !isContainMat && !isContainAsset)
        {
            Debug.LogError("要选择一种 查找替换的类型");
            ShowNotification(new GUIContent("要选择一种 查找替换的类型"));
        }
        else if(string.IsNullOrEmpty(_oldGuid))
        {
            Debug.LogError("_oldGuid=" + _oldGuid);
            ShowNotification(new GUIContent("_oldGuid=" + _oldGuid));
        }
        else if(string.IsNullOrEmpty(_newGuid))
        {
            Debug.LogError("_newGuid=" + _newGuid);
            ShowNotification(new GUIContent("_newGuid=" + _newGuid));
        }
        else   // 执行替换逻辑
        {
            return true;
        }

        return false;
    }


    private void StartReplace()
    {
       
        Debug.Log("oldGUID = " + _oldGuid + "  " + "_newGuid = " + _newGuid);

        withoutExtensions = new List<string>();
        if (isContainScene)
        {
            withoutExtensions.Add(".unity");
        }
        if (isContainPrefab)
        {
            withoutExtensions.Add(".prefab");
        }
        if (isContainMat)
        {
            withoutExtensions.Add(".mat");
        }
        if (isContainAsset)
        {
            withoutExtensions.Add(".asset");
        }

        Find();
    }

    //void RefreshMat(string findType)
    //{
    //    var guids = AssetDatabase.FindAssets("t:" + findType);  // Material      // 这一句牛逼呀， 直接按照类型获取   file:///D:/Program%20Files/Unity%205.2.0b4/Unity/Editor/Data/Documentation/en/ScriptReference/AssetDatabase.FindAssets.html
    //    foreach (var guid in guids)
    //    {

    //    }
    //}

    /// <summary>
    /// 查找  并   替换 
    /// </summary>
    private void Find()
    {
        if (withoutExtensions == null || withoutExtensions.Count == 0)
        {
            withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
        }

        string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        int startIndex = 0;

        if (files == null || files.Length == 0)
        {
            Debug.Log("没有找到 筛选的引用");
            return;
        }

        EditorApplication.update = delegate ()
            {
                string file = files[startIndex];

                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                var content = File.ReadAllText(file);
                if (Regex.IsMatch(content, _oldGuid))
                {
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));

                    if(!isPreview)
                    {
                        content = content.Replace(_oldGuid, _newGuid);

                        File.WriteAllText(file, content);
                    }
                }
                else
                {
//                    Debug.Log(file);
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;

                    AssetDatabase.Refresh();
                    Debug.Log("替换结束");
                }

            };
    }

    private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

}
