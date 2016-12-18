using UnityEngine;
using System.Collections;
using UnityEditor;
using Games;
using System.IO;
using System.Collections.Generic;
using System;


namespace com.ihaiu
{
    public partial class VersionPushWindow : EditorWindow
    {
        

        public static VersionPushWindow window;
		[MenuItem ("资源管理/版本推送面板", false, 900)]
        public static void Open () 
        {
            window = EditorWindow.GetWindow <VersionPushWindow>("版本推送" );
            window.minSize = new Vector2(400, 700);
            window.Show();
        }

		public bool 	gitNeedPassword 	= false;
		public string 	gitPassword 		= "git";

        public string gitRoot
        {
            get
            {
                return AssetManagerSetting.EditorGitRoot;
            }
        }

        public string gitPlatformRoot
        {
            get
            {
                return gitRoot + Platform.PlatformDirectoryName.ToLower() + "/";
            }
        }

        public string gitVerresRoot
        {
            get
            {
                return gitPlatformRoot + "verres/";
            }
        }

        public string gitVerinfoRoot
        {
            get
            {
                return gitPlatformRoot + "verinfo/";
            }
        }

        public string gitGameConstPath
        {
            get
            {
				return gitVerresRoot + "game_const.json";
            }
        }

        string GetTag(string center)
        {
            return string.Format("{0}_ver{1}_{2}", Platform.PlatformDirectoryName, GameConstConfig.Load(gitGameConstPath).Version, center).ToLower();
        }

        string GetBranchTag()
        {
            return  string.Format("{0}_ver{1}", Platform.PlatformDirectoryName, GameConstConfig.Load(gitGameConstPath).Version).ToLower();
        }



        Vector2 scrollPos;
        bool foldout_center             = true;
		bool foldout_res                = true;
		bool foldout_res_popups         = true;
		bool foldout_res_option         = false;
		bool foldout_res_option2        = false;


        /** 拷贝内容方式 */
        public enum CopyType
        {
            /** 不拷贝内容 */
            None,
            /** 拷贝更新数据到该目录 */
            Update,
            /** 拷贝所有数据到该目录 */
            All
        }


        /** 如果已经存在相同tag执行方案 */
        public enum AlreadyExistTagPlan
        {
            /** 终止 */
            Kill,
            /** 替换之前的 */
            Replace,
            /** 加后缀 */
            Suffix
        }

        /** 如果已经存在相同tag执行方案 */
        AlreadyExistTagPlan     alreadyExistTagPlan     = AlreadyExistTagPlan.Kill;
        /** 拷贝内容方式 */
        CopyType                copyType                = CopyType.Update;

        void OnGUI ()
        {
            Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);


			GUILayout.Space(20);

            // center
            foldout_center = EditorGUILayout.Foldout(foldout_center, "发行商选择");
            if (foldout_center)
            {
                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));
                for(int i = 0; i < CenterSwitcher.centerItemList.Length; i ++)
                {
                    CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                    item.gitToggle = EditorGUILayout.ToggleLeft(item.name, item.gitToggle, GUILayout.Width(250));
                }

                GUILayout.Space(20);

                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("全选", GUILayout.MinHeight(30), GUILayout.MaxWidth(200)))
                {
                    for(int i = 0; i < CenterSwitcher.centerItemList.Length; i ++)
                    {
                        CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                        item.gitToggle = true;
                    }
                }


                GUILayout.Space(20);

                if (GUILayout.Button("全不选", GUILayout.MinHeight(30), GUILayout.MaxWidth(200)))
                {
                    for(int i = 0; i < CenterSwitcher.centerItemList.Length; i ++)
                    {
                        CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                        item.gitToggle = false;
                    }
                }

                HGUILayout.EndCenterHorizontal();

                GUILayout.EndVertical();
            }

            GUILayout.Space(30);

            // Res
            foldout_res = EditorGUILayout.Foldout(foldout_res, "资源");
            if (foldout_res)
            {
                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));


                //  选项
                foldout_res_popups = EditorGUILayout.Foldout(foldout_res_popups, "选项");
                if (foldout_res_popups)
                {
                    GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));
                    copyType                = (CopyType)                EditorGUILayout.EnumPopup("拷贝内容方式: ", copyType);
                    GUILayout.Space(20);
                    alreadyExistTagPlan     = (AlreadyExistTagPlan)     EditorGUILayout.EnumPopup("如果已经存在相同tag执行方案: ", alreadyExistTagPlan);

					GUILayout.Space(20);
					gitNeedPassword		= EditorGUILayout.ToggleLeft("git是否需要密码提交：", gitNeedPassword);
					gitPassword			= EditorGUILayout.TextField("git密码：", gitPassword);
					GUILayout.EndVertical();
                }
                GUILayout.Space(20);




                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("推送资源", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
                {

					Step_Begin();
					Step_CheckBranch();

					if (stepIsContinue)
						Step_CheckTag();
					
					if (stepIsContinue)
						Step_Copy();

					if (stepIsContinue)
						Step_CommitBranch();
					
					if (stepIsContinue)
						Step_AddTag();

					if (stepIsContinue)
						Step_PushBranchAndTag();

                }
                HGUILayout.EndCenterHorizontal();



                foldout_res_option = EditorGUILayout.Foldout(foldout_res_option, "高级");
                if (foldout_res_option)
                {
                    GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));


                    if (GUILayout.Button("Begin", GUILayout.MinHeight(30)))
                    {
                        Step_Begin();
                    }


                    if (GUILayout.Button("检测分支", GUILayout.MinHeight(30)))
                    {
                        Step_CheckBranch();
                    }


                    if (GUILayout.Button("检查tag是否已经存在", GUILayout.MinHeight(30)))
                    {
                        Step_CheckTag();
                    }

                    if (GUILayout.Button("拷贝内容", GUILayout.MinHeight(30)))
                    {
                        Step_Copy();
                    }


                    if (GUILayout.Button("commit branch", GUILayout.MinHeight(30)))
                    {
                        Step_CheckTag();
                        Step_CommitBranch();
                    }

					if (GUILayout.Button("add tag", GUILayout.MinHeight(30)))
					{
						Step_CheckTag();
						if (stepIsContinue)
							Step_AddTag();
					}



                    if (GUILayout.Button("push", GUILayout.MinHeight(30)))
                    {
                        Step_PushBranchAndTag();
                    }





                    GUILayout.EndVertical();
                }


				foldout_res_option2 = EditorGUILayout.Foldout(foldout_res_option2, "其他");
				if (foldout_res_option2)
				{

					GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));
					if (GUILayout.Button("清除所有本地tag", GUILayout.MinHeight(30)))
					{
						ClearAllTagLoacl();
					}


					if (GUILayout.Button("清除所有服务器tag", GUILayout.MinHeight(30)))
					{
						ClearAllTagOrigin();
					}

					GUILayout.EndVertical();
				}

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();


        }


      

    }


}
