using UnityEngine;
using System.Collections;
using UnityEditor;
using Games;
using System.IO;
using System.Collections.Generic;
using System;


namespace com.ihaiu
{
    public partial class VersionInfoWindow : EditorWindow
    {

       

        public static VersionInfoWindow window;
//        [MenuItem ("资源管理/版本信息编辑面板", false, 900)]
        public static void Open () 
        {
            window = EditorWindow.GetWindow <VersionInfoWindow>(true, "版本信息编辑" );
            window.minSize = new Vector2(500, 700);
            window.Show();
        }



        public GitServerEditor gitServerEditor  = new GitServerEditor();



        public List<CenterSwitcher.CenterItem> centerList
        {
            get
            {
                return gitServerEditor.centerList;
            }
        }



        Vector2 scrollPos;
        bool foldout_center             = true;
        bool foldout_global             = true;
        bool foldout_centerList         = false;
        bool foldout_git                = true;

        VersionInfo _versionInfo;
        VersionInfo versionInfo
        {
            get
            {
                if (_versionInfo == null)
                {
                    if (centerList.Count > 0)
                    {
                        _versionInfo = VersionInfo.Load(gitServerEditor.GetGitVerinfoPath(centerList[0].name, true));
                    }
                    else
                    {
                        _versionInfo = new VersionInfo();
                    }
                }

                return _versionInfo;
            }
        }

        void OnGUI ()
        {
            gitServerEditor.Init();


            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.Space(20);
            gitServerEditor.OnGUI();
            GUILayout.Space(30);

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



            // global
            foldout_global = EditorGUILayout.Foldout(foldout_global, "全局");
            if (foldout_global)
            {
                if (GUILayout.Button("刷新", GUILayout.MinHeight(25)))
                {
                    _versionInfo = null;
                }
                GUILayout.Space(30);


                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.ExpandWidth(true));
                versionInfo.version = EditorGUILayout.TextField("版本号: ", versionInfo.version, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("修改选择的应用商", GUILayout.MinHeight(25), GUILayout.MaxWidth(200)))
                {
                    for(int i = 0; i < CenterSwitcher.centerItemList.Length; i ++)
                    {
                        CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                        if (item.gitToggle)
                        {
                            VersionInfo itemVerInfo = VersionInfo.Load(gitServerEditor.GetGitVerinfoPath(item.name, true));
                            itemVerInfo.version = versionInfo.version;
                            itemVerInfo.Save(gitServerEditor.GetGitVerinfoPath(item.name, true));
                        }
                    }
                }
                GUILayout.Space(30);

                EditorGUILayout.LabelField("大版本更新通知:");
                versionInfo.noteDownApp = EditorGUILayout.TextArea(versionInfo.noteDownApp, GUILayout.ExpandWidth(true), GUILayout.Height(60));
                if (GUILayout.Button("修改选择的应用商", GUILayout.MinHeight(25), GUILayout.MaxWidth(200)))
                {
                    for(int i = 0; i < CenterSwitcher.centerItemList.Length; i ++)
                    {
                        CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                        if (item.gitToggle)
                        {
                            VersionInfo itemVerInfo = VersionInfo.Load(gitServerEditor.GetGitVerinfoPath(item.name, true));
                            itemVerInfo.noteDownApp = versionInfo.noteDownApp;
                            itemVerInfo.Save(gitServerEditor.GetGitVerinfoPath(item.name, true));
                        }
                    }
                }

                GUILayout.Space(30);
                EditorGUILayout.LabelField("热更新通知:");
                versionInfo.noteHostUpdate = EditorGUILayout.TextArea(versionInfo.noteHostUpdate,  GUILayout.ExpandWidth(true), GUILayout.Height(60));
                if (GUILayout.Button("修改选择的应用商", GUILayout.MinHeight(25), GUILayout.MaxWidth(200)))
                {
                    for(int i = 0; i < CenterSwitcher.centerItemList.Length; i ++)
                    {
                        CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                        if (item.gitToggle)
                        {
                            VersionInfo itemVerInfo = VersionInfo.Load(gitServerEditor.GetGitVerinfoPath(item.name, true));
                            itemVerInfo.noteHostUpdate = versionInfo.noteHostUpdate;
                            itemVerInfo.Save(gitServerEditor.GetGitVerinfoPath(item.name, true));
                        }
                    }
                }



                GUILayout.Space(30);
                versionInfo.isClose = EditorGUILayout.ToggleLeft("是否停服", versionInfo.isClose == 1) ? 1 : 0;
                if (GUILayout.Button("修改选择的应用商", GUILayout.MinHeight(25), GUILayout.MaxWidth(200)))
                {
                    for(int i = 0; i < CenterSwitcher.centerItemList.Length; i ++)
                    {
                        CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                        if (item.gitToggle)
                        {
                            VersionInfo itemVerInfo = VersionInfo.Load(gitServerEditor.GetGitVerinfoPath(item.name, true));
                            itemVerInfo.isClose = versionInfo.isClose;
                            itemVerInfo.Save(gitServerEditor.GetGitVerinfoPath(item.name, true));
                        }
                    }
                }
                GUILayout.EndVertical();
            }


            // center list
            foldout_centerList = EditorGUILayout.Foldout(foldout_centerList, "版本信息列表");
            if (foldout_centerList)
            {

                if (GUILayout.Button("刷新所有", GUILayout.MinHeight(25)))
                {
                    for (int i = 0; i < CenterSwitcher.centerItemList.Length; i++)
                    {
                        CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                        item.versionInfo = null;
                    }
                }
                GUILayout.Space(20);


                for (int i = 0; i < CenterSwitcher.centerItemList.Length; i++)
                {
                    GUILayout.Space(20);
                    CenterSwitcher.CenterItem item = CenterSwitcher.centerItemList[i];
                    GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.LabelField(item.name +"  ( "+ gitServerEditor.GetGitVerinfoPath(item.name, true).Replace(gitServerEditor.gitVerinfoRoot, "") + " )");
                  

                    GUILayout.Space(20);

                    if (item.versionInfo == null)
                    {
                        item.versionInfo = VersionInfo.Load(gitServerEditor.GetGitVerinfoPath(item.name, true));
                    }


                    item.versionInfo.version = EditorGUILayout.TextField("版本号: ", item.versionInfo.version, GUILayout.ExpandWidth(true));
                    GUILayout.Space(10);

                    EditorGUILayout.LabelField("大版本更新通知:");
                    item.versionInfo.noteDownApp = EditorGUILayout.TextArea(item.versionInfo.noteDownApp, GUILayout.ExpandWidth(true), GUILayout.Height(60));
                    GUILayout.Space(10);

                    EditorGUILayout.LabelField("热更新通知:");
                    item.versionInfo.noteHostUpdate = EditorGUILayout.TextArea(item.versionInfo.noteHostUpdate, GUILayout.ExpandWidth(true), GUILayout.Height(60));
                    GUILayout.Space(10);

                    item.versionInfo.isClose = EditorGUILayout.ToggleLeft("是否停服", item.versionInfo.isClose == 1) ? 1 : 0;
                    GUILayout.Space(10);

                    HGUILayout.BeginCenterHorizontal();

                    if (GUILayout.Button("保存", GUILayout.MinHeight(25), GUILayout.MaxWidth(200)))
                    {
                        item.versionInfo.Save(gitServerEditor.GetGitVerinfoPath(item.name, true));
                    }

                    if (GUILayout.Button("刷新", GUILayout.MinHeight(25), GUILayout.MaxWidth(200)))
                    {
                        item.versionInfo = null;
                    }
                    HGUILayout.EndCenterHorizontal();
                   
                    GUILayout.EndVertical();
                    GUILayout.Space(20);
                }
            }


            // git
            foldout_git = EditorGUILayout.Foldout(foldout_git, "git");
            if (foldout_git)
            {

                GUILayout.BeginVertical(HGUILayout.boxMPStyle, GUILayout.ExpandWidth(true));

                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("CopyFromTest", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
                {
                    Step_Verinfo_CopyFromTest();
                }
                HGUILayout.EndCenterHorizontal();
                GUILayout.Space(10);

                HGUILayout.BeginCenterHorizontal();
                if (GUILayout.Button("推送", GUILayout.MinHeight(50), GUILayout.MaxWidth(200)))
                {
                    Step_Begin();


//                    if (stepIsContinue)
//                        Step_CheckBranch();

                    if (stepIsContinue)
                        Step_CommitBranch();

                    if (stepIsContinue)
                        Step_PushBranchAndTag();
                    
                }
                HGUILayout.EndCenterHorizontal();




                GUILayout.EndVertical();
                GUILayout.Space(30);
            }



            EditorGUILayout.EndScrollView();

        }

        ShFile tmp
        {
            get
            {
                return ShFile.tmp;
            }
        }

        string branch
        {
            get
            {
                return  Platform.PlatformDirectoryName.ToLower();
            }
        }

        bool stepIsContinue = true;

        /** Begin */
        void Step_Begin()
        {
            stepIsContinue = true;
        }


        /** 检测分支 */
        void Step_CheckBranch()
        {
            string branch       = this.branch;
            string branchOrigin = "remotes/origin/" + branch;
            tmp.Clear();
            tmp.WriteLine("cd " + gitServerEditor.gitRoot);
            tmp.WriteLine("git fetch " );
            tmp.WriteLine(string.Format("git branch -a > {0}",  Shell.txt_vertmp));
            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);


            string branchSelect = "";
            bool hasBranchLocal     = false;
            bool hasBranchOrigin    = false;
            string txt =File.ReadAllText(Shell.txt_vertmp);
            string[] branchs = txt.Split('\n');
            for(int i = 0; i < branchs.Length; i ++)
            {
                if (branchs[i].IndexOf("*") != -1)
                {
                    branchSelect = branchs[i].Replace("*", "").Replace(" ", "");
                }

                if (branchs[i].Replace("*", "").Replace(" ", "") == branch)
                {
                    hasBranchLocal = true;
                }

                if (branchs[i].Replace(" ", "") == branchOrigin)
                {
                    hasBranchOrigin = true;
                }
            }



            // 检测当前文件状态
            tmp.Clear();
            tmp.WriteLine("cd " + gitServerEditor.gitRoot);
            tmp.WriteLine(string.Format("git status -s > {0}",  Shell.txt_vertmp));
            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);
            txt =File.ReadAllText(Shell.txt_vertmp);
            if (!string.IsNullOrEmpty(txt))
            {
                if(!EditorUtility.DisplayDialog("git status -s", "警告：当前分支存在修改。是否继续执行？ （建议先用git工具处理好再执行，否则将会合并到当前版本）\n" + txt, "继续执行", "终止执行"))
                {
                    stepIsContinue = false;
                    return;
                }
            }


            Debug.Log("branchSelect =" + branchSelect);
            Debug.Log("branch =" + branch);
            Debug.Log("branchOrigin =" + branchOrigin);
            Debug.Log("hasBranchLocal =" + hasBranchLocal);
            Debug.Log("hasBranchOrigin =" + hasBranchOrigin);

            // 拉取 更新 远程分支
            if (hasBranchLocal && hasBranchOrigin)
            {
                if (branchSelect != branch)
                {
                    // 切换分支
                    if (!CheckoutBranch())
                        return;
                }


                if(gitServerEditor.gitServer.needPassword)
                {
                    tmp.Clear(true);
                    tmp.WriteLine("cd " + gitServerEditor.gitRoot);
                    tmp.WriteLine(string.Format("git pull origin {0}",  branch),   gitServerEditor.gitServer.needPassword, gitServerEditor.gitServer.password);
                    tmp.Save();
                    Shell.RunTmp(Shell.sh_tmp);

                    if(!EditorUtility.DisplayDialog("git pull origin " + branch, "请检查拉取远程分支信息!  如果没有错误点‘继续’,否则点'终止'!!", "继续执行", "终止执行"))
                    {
                        stepIsContinue = false;
                        return;
                    }

                }
                else
                {
                    tmp.Clear(false);
                    tmp.WriteLine("cd " + gitServerEditor.gitRoot);
                    tmp.WriteLine(string.Format("git pull origin {1} 2>&1 | tee {0}",  Shell.txt_vertmp, branch));
                    tmp.Save();
                    Shell.RunTmp(Shell.sh_tmp);
                }



                txt = File.ReadAllText(Shell.txt_vertmp);
                if (txt.IndexOf("error:") != -1 || txt.IndexOf("fatal:") != -1)
                {
                    EditorUtility.DisplayDialog("git pull origin " + branch, "错误：拉取远程分支出错, 建议先用git工具处理好再执行!\n" + txt, "终止执行");

                    stepIsContinue = false;
                    return;
                }

            }
            // 拉取 远程分支，创建 本地分支
            else if (!hasBranchLocal && hasBranchOrigin)
            {
                tmp.Clear();
                tmp.WriteLine("cd " + gitServerEditor.gitRoot);
                tmp.WriteLine(string.Format("git checkout -b {1} origin/{1} 2>&1 | tee {0}",  Shell.txt_vertmp, branch));
                tmp.Save();
                Shell.RunTmp(Shell.sh_tmp);

                txt = File.ReadAllText(Shell.txt_vertmp);
                if (txt.IndexOf("error:") != -1)
                {
                    EditorUtility.DisplayDialog(string.Format("git checkout -b {0} origin.{0}",  branch), "错误：拉取远程分支，创建本地分支出错. 建议先用git工具处理好再执行!\n" + txt, "终止执行");

                    stepIsContinue = false;
                    return;
                }
            }
            // 创建本地分支
            else if(!hasBranchLocal && !hasBranchOrigin)
            {
                tmp.Clear();
                tmp.WriteLine("cd " + gitServerEditor.gitRoot);
                tmp.WriteLine(string.Format("git checkout -b {1} 2>&1 | tee {0}",  Shell.txt_vertmp, branch));
                tmp.Save();
                Shell.RunTmp(Shell.sh_tmp);


                txt = File.ReadAllText(Shell.txt_vertmp);
                if (txt.IndexOf("error:") != -1)
                {
                    EditorUtility.DisplayDialog(string.Format("git checkout -b {0}",  branch), "错误：创建本地分支出错. 建议先用git工具处理好再执行!\n" + txt, "终止执行");

                    stepIsContinue = false;
                    return;
                }
            }
        }


        // 切换分支
        bool CheckoutBranch()
        {
            tmp.Clear();
            tmp.WriteLine("cd " + gitServerEditor.gitRoot);
            tmp.WriteLine(string.Format("git checkout {1} 2>&1 | tee {0}", Shell.txt_vertmp, branch));
            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);

            string txt = File.ReadAllText(Shell.txt_vertmp);
            if (txt.IndexOf("error:") != -1 || txt.IndexOf("fatal:") != -1)
            {
                EditorUtility.DisplayDialog("git checkout " + branch, "错误：切换分支出错, 建议先用git工具处理好再执行!\n" + txt, "终止执行");
                return false;
            }

            return true;
        }


        /** 提交分支 */
        void Step_CommitBranch()
        {
            // 切换分支
            if (!CheckoutBranch())
                return;

            Debug.Log("git add .");
            tmp.Clear();
            tmp.WriteLine("cd " + gitServerEditor.gitRoot);
            tmp.WriteLine(string.Format("git add . 2>&1 | tee {0}", Shell.txt_vertmp));
            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);

            string txt = File.ReadAllText(Shell.txt_vertmp);
            if (txt.IndexOf("error:") != -1)
            {
                EditorUtility.DisplayDialog("git add . ", "错误： 建议先用git工具处理好再执行!\n" + txt, "终止执行");

                stepIsContinue = false;
                return;
            }


            Debug.Log("git commit -am");
            tmp.Clear();
            tmp.WriteLine("cd " + gitServerEditor.gitRoot);
            tmp.WriteLine(string.Format("git commit -am \"提交版本信息修改\" 2>&1 | tee {0}", Shell.txt_vertmp));
            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);
            txt = File.ReadAllText(Shell.txt_vertmp);
            if (txt.IndexOf("error:") != -1 || txt.IndexOf("fatal:") != -1)
            {
                EditorUtility.DisplayDialog(string.Format("git commit -am \"提交版本信息修改\""), "错误： 建议先用git工具处理好再执行!\n" + txt, "终止执行");

                stepIsContinue = false;
                return;
            }

          
        }


        /** 推送分支 And Tag */
        void Step_PushBranchAndTag()
        {
            // 切换分支
            if (!CheckoutBranch())
                return;


            tmp.Clear(gitServerEditor.gitServer.needPassword);
            tmp.WriteLine("cd " + gitServerEditor.gitRoot);
            tmp.WriteLine(string.Format("git push origin {0}",  branch)    , gitServerEditor.gitServer.needPassword, gitServerEditor.gitServer.password);

            tmp.Save();
            Shell.RunFile(Shell.sh_tmp);
        }


        /** verinfo 将test转正 */
        void Step_Verinfo_CopyFromTest()
        {
            ShFile.tmp.Clear();
            ShFile.tmp.WriteLine("cd " + gitServerEditor.gitRoot);


            for (int i = 0; i < centerList.Count; i++)
            {
                CenterSwitcher.CenterItem item = centerList[i];

                if (item.gitToggle)
                {
                    string verinfoPath_Test = gitServerEditor.GetGitVerinfoPath(item.name, true);
                    string verinfoPath =gitServerEditor.GetGitVerinfoPath(item.name, false);

                    ShFile.tmp.WriteLine(string.Format("cp {0} {1}", verinfoPath_Test, verinfoPath));

                }
            }


            ShFile.tmp.Save();
            Shell.RunTmp(ShFile.tmp.path);
        }





      

    }


}
