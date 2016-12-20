using UnityEngine;
using System.Collections;
using UnityEditor;
using Games;
using System.IO;
using System.Collections.Generic;
using System;


namespace com.ihaiu
{
    public class GitServerItem
    {
        public string   name;
        public string   rootPath;
        public string   url;

        public string   gitServer       = "git@127.0.0.1:/repositories/test.version.git";
        public bool     needPassword    = false;
        public string   password        = "git";

        public List<CenterSwitcher.CenterItem> centerList = new List<CenterSwitcher.CenterItem>();

        public GitServerItem(string name, string rootPath)
        {
            this.name       = name;
            this.rootPath   = rootPath;
        }

        public GitServerItem AddCenter(CenterSwitcher.CenterItem centerItem)
        {
            centerList.Add(centerItem);
            return this;
        }

        public string GetHostUpdateUrlRoot(string tag)
        {
            return "http://127.0.0.1:8080/?p=.git;a=blob_plain;hb=refs/tags/"+ tag +";f="+ Platform.PlatformDirectoryName.ToLower() +"/verres/";
        }
    }

    public class GitServerEditor
    {
        
        public List<GitServerItem>  gitServerList;
        public GitServerItem        gitServer;
        string[]                    gitServerNames;
        int                         gitServerSelectIndex;


        public void Init()
        {
            if (gitServerList == null || gitServerList.Count == 0)
            {
                gitServerList = new List<GitServerItem>();

                GitServerItem gitServer;
                List<string> centerNames;

                gitServer = new GitServerItem("国内服务器", Application.dataPath + "/../../../test.version/");
                centerNames= new List<string>{"Official", "XiaoMi", "WeiXin", "UC"};
                foreach(CenterSwitcher.CenterItem item in CenterSwitcher.centerItemList)
                {
                    if(centerNames.IndexOf(item.name) != -1)
                    {
                        gitServer.AddCenter(item);
                    }
                }
                gitServerList.Add(gitServer);



                gitServer = new GitServerItem("海外服务器", Application.dataPath + "/../../../test.version/");
                centerNames= new List<string>{"Official"};
                foreach(CenterSwitcher.CenterItem item in CenterSwitcher.centerItemList)
                {
                    if(centerNames.IndexOf(item.name) != -1)
                    {
                        gitServer.AddCenter(item);
                    }
                }
                gitServerList.Add(gitServer);



                gitServerNames = new string[gitServerList.Count];
                for(int i = 0; i < gitServerList.Count; i ++)
                {
                    gitServerNames[i] = gitServerList[i].name;
                }

                gitServerSelectIndex = 0;
                gitServer = gitServerList[gitServerSelectIndex];

            }
        }


        public void OnGUI()
        {
            gitServerSelectIndex = EditorGUILayout.Popup("git server:", gitServerSelectIndex, gitServerNames);
            gitServer = gitServerList[gitServerSelectIndex];
        }




        /** 目录--Git版本 */
        public string gitRoot
        {
            get
            {
                return gitServer.rootPath;
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

        public string GetGitVerinfoPath(string center, bool isTest = false)
        {
            return gitVerinfoRoot + (isTest ? "test_" : "") + "ver_" + center.ToLower() + ".txt";
        }

        public string GetGitHostUpdateUrlRoot(string tag)
        {
            return gitServer.GetHostUpdateUrlRoot(tag);
        }

        public List<CenterSwitcher.CenterItem> centerList
        {
            get
            {
                return gitServer.centerList;
            }
        }



    }

    public partial class VersionPushWindow : EditorWindow
    {

       


       


        public static VersionPushWindow window;
//		[MenuItem ("资源管理/版本推送面板", false, 900)]
        public static void Open () 
        {
            window = EditorWindow.GetWindow <VersionPushWindow>(true, "版本推送" );
            window.minSize = new Vector2(400, 700);
            window.Show();
        }

        public GitServerEditor gitServerEditor  = new GitServerEditor();
        public bool     gitPullVisiableWindow   = false;

        public bool     gitNeedPassword
        {
            get
            {
                return gitServerEditor.gitServer.needPassword;
            }

            set
            {
                gitServerEditor.gitServer.needPassword = value;
            }
        }


        public string    gitPassword
        {
            get
            {
                return gitServerEditor.gitServer.password;
            }

            set
            {
                gitServerEditor.gitServer.password = value;
            }
        }


        public string gitRoot
        {
            get
            {
                return gitServerEditor.gitRoot;
            }
        }


        public List<CenterSwitcher.CenterItem> centerList
        {
            get
            {
                return gitServerEditor.centerList;
            }
        }


        string GetTag(string center)
        {
            return string.Format("{0}_ver{1}_{2}", Platform.PlatformDirectoryName, version, center).ToLower();
        }

        string GetBranchTag()
        {
            return  string.Format("{0}_ver{1}", Platform.PlatformDirectoryName, version).ToLower();
        }

        public VersionReadFile versionReadFile = VersionReadFile.StreamingAssets;
        public string version
        {
            get
            {
                if (versionReadFile == VersionReadFile.StreamingAssets)
                {
                    return GameConstConfig.Load().Version;
                }
                else
                {
                    return GameConstConfig.Load(gitServerEditor.gitGameConstPath).Version;
                }
            }
        }



        Vector2 scrollPos;
        bool foldout_center             = true;
		bool foldout_res                = true;
		bool foldout_res_popups         = true;
		bool foldout_res_option         = false;
        bool foldout_res_option2        = false;
        bool foldout_verinfo            = true;
        bool foldout_verinfo_option     = false;


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

        /** 版本号读取文件 */
        public enum VersionReadFile
        {
            StreamingAssets,
            Git,
        }

        /** 如果已经存在相同tag执行方案 */
        AlreadyExistTagPlan     alreadyExistTagPlan     = AlreadyExistTagPlan.Kill;
        /** 拷贝内容方式 */
        CopyType                copyType                = CopyType.Update;



        void OnGUI ()
        {
            gitServerEditor.Init();
            Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.Space(20);
            gitServerEditor.OnGUI();

			GUILayout.Space(20);

            // center
            foldout_center = EditorGUILayout.Foldout(foldout_center, "发行商选择");
            if (foldout_center)
            {
                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));
                for(int i = 0; i < centerList.Count; i ++)
                {
                    CenterSwitcher.CenterItem item = centerList[i];
                    item.gitToggle = EditorGUILayout.ToggleLeft(item.name, item.gitToggle, GUILayout.Width(250));
                }

                GUILayout.Space(20);

                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("全选", GUILayout.MinHeight(30), GUILayout.MaxWidth(200)))
                {
                    for(int i = 0; i < centerList.Count; i ++)
                    {
                        CenterSwitcher.CenterItem item = centerList[i];
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
					gitNeedPassword		= EditorGUILayout.ToggleLeft("git是否需要密码提交", gitNeedPassword);
					gitPassword			= EditorGUILayout.TextField("git密码：", gitPassword);

                    GUILayout.Space(20);
                    gitPullVisiableWindow   = EditorGUILayout.ToggleLeft("Pull时是否显示命令窗口", gitPullVisiableWindow);

                    GUILayout.Space(20);
                    versionReadFile                = (VersionReadFile)                EditorGUILayout.EnumPopup("版本号读取目录: ", versionReadFile);
                    EditorGUILayout.LabelField(version);

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


            // 版本信息
            foldout_verinfo = EditorGUILayout.Foldout(foldout_verinfo, "版本信息");
            if (foldout_verinfo)
            {
                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));
                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("推送正式版本信息", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
                {
                    Step_Begin();
                    Step_Verinfo_CopyFromTest();
                    Step_CommitBranch(true);
                    if (stepIsContinue)
                        Step_PushBranch();


                }

                if (GUILayout.Button("打开版本信息编辑面板", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
                {
                    VersionInfoWindow.Open();
                }
                HGUILayout.EndCenterHorizontal();


                //  选项
                foldout_verinfo_option = EditorGUILayout.Foldout(foldout_verinfo_option, "选项");
                if (foldout_verinfo_option)
                {

                    GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.Height(50));

                    if (GUILayout.Button("Copy From Test", GUILayout.MinHeight(30)))
                    {
                        Step_Verinfo_CopyFromTest();
                    }


                    if (GUILayout.Button("commit branch", GUILayout.MinHeight(30)))
                    {
                        Step_CommitBranch(true);
                    }



                    if (GUILayout.Button("push branch", GUILayout.MinHeight(30)))
                    {
                        Step_PushBranch();
                    }


                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();


        }


      

    }


}
