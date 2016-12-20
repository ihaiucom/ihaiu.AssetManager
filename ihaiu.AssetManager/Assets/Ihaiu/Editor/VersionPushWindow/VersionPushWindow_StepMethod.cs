using UnityEngine;
using System.Collections;
using UnityEditor;
using Games;
using System.IO;
using System.Collections.Generic;
using System;


namespace com.ihaiu
{
    public partial class VersionPushWindow 
    {
        bool stepIsContinue = true;
        bool hasAlreadyExistTag = false;

        ShFile tmp ;

        string branch
        {
            get
            {
                return  Platform.PlatformDirectoryName.ToLower();
            }
        }

        string  branchTag
        {
            get
            {
                return GetBranchTag() + "_date" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        void Init()
        {
            if (tmp != null)
                return;


            tmp                     = new ShFile(Shell.sh_vertmp);
        }



        /** Begin */
        void Step_Begin()
        {
            stepIsContinue = true;
            hasAlreadyExistTag = false;
        }

        /** 检测分支 */
        void Step_CheckBranch()
        {
            string branch       = this.branch;
            string branchOrigin = "remotes/origin/" + branch;
            tmp.Clear();
            tmp.WriteLine("cd " + gitRoot);
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
            tmp.WriteLine("cd " + gitRoot);
            tmp.WriteLine(string.Format("git status -s > {0}",  Shell.txt_vertmp));
            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);
            txt =File.ReadAllText(Shell.txt_vertmp);
            if (!string.IsNullOrEmpty(txt))
            {
                LogWarning(txt);
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


				if(gitNeedPassword)
				{
					tmp.Clear(true);
					tmp.WriteLine("cd " + gitRoot);
					tmp.WriteLine(string.Format("git pull origin {0}",  branch),   gitNeedPassword, gitPassword);
					tmp.Save();

                    if (gitPullVisiableWindow)
                        Shell.RunFile(Shell.sh_tmp, true);
                    else
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
					tmp.WriteLine("cd " + gitRoot);
					tmp.WriteLine(string.Format("git pull origin {1} 2>&1 | tee {0}",  Shell.txt_vertmp, branch));
					tmp.Save();

                    if (gitPullVisiableWindow)
                        Shell.RunFile(Shell.sh_tmp, true);
                    else
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
                tmp.WriteLine("cd " + gitRoot);
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
                tmp.WriteLine("cd " + gitRoot);
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
            tmp.WriteLine("cd " + gitRoot);
            tmp.WriteLine(string.Format("git checkout {1} 2>&1 | tee {0}", Shell.txt_vertmp, branch));
            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);

            string txt = File.ReadAllText(Shell.txt_vertmp);
            if (txt.IndexOf("error:") != -1 || txt.IndexOf("fatal:") != -1)
            {
                EditorUtility.DisplayDialog("git checkout " + branch, "错误：切换分支出错, 建议先用git工具处理好再执行!\n" + txt, "终止执行");

                stepIsContinue = false;
                return false;
            }

            return true;
        }


        /** 检测tag */
        void Step_CheckTag()
        {
            tmp.Clear();
            tmp.WriteLine("cd " + gitRoot);
            tmp.WriteLine("git tag > "+Shell.txt_vertmp);


            tmp.Save();

            Shell.RunTmp(Shell.sh_tmp);




            string txt =File.ReadAllText(Shell.txt_vertmp);
            string[] tags = txt.Split('\n');
            Dictionary<string, bool> tagDict = new Dictionary<string, bool>();
            Dictionary<string, int> tagIndexDict = new Dictionary<string, int>();
            for(int i = 0; i < tags.Length; i ++)
            {
                tagDict.Add(tags[i], true);

                var arr = tags[i].Split('-');

                int index = tagIndexDict.ContainsKey(arr[0]) ? tagIndexDict[arr[0]] : 0;
                if (arr.Length == 2)
                {
                    if (index < Convert.ToInt32(arr[1]) )
                    {
                        index = Convert.ToInt32(arr[1]);
                    }
                }

                if (tagIndexDict.ContainsKey(arr[0]))
                {
                    tagIndexDict[arr[0]] = index;
                }
                else
                {
                    tagIndexDict.Add(arr[0], index);
                }
            }

            hasAlreadyExistTag = false;
            for(int i = 0; i < centerList.Count; i ++)
            {
                CenterSwitcher.CenterItem item = centerList[i];

                if (item.gitToggle)
                {
                    item.gitTag = GetTag(item.name);
                    item.gitTagMaxIndex = tagIndexDict.ContainsKey(item.gitTag) ? tagIndexDict[item.gitTag] + 1 : 0;

                    if (item.gitTagMaxIndex > 0)
                    {
                        hasAlreadyExistTag = true;
                        LogWarning(string.Format("已经存在tag={0}", item.gitTag));
                    }
                }
            }



            if (hasAlreadyExistTag)
            {
                switch(alreadyExistTagPlan)
                {
                    case AlreadyExistTagPlan.Kill:
                        stepIsContinue = false;
                        break;
                }
            }
        }

        /** 拷贝内容 */
        void Step_Copy()
        {
            switch(copyType)
            {
                case CopyType.Update:
                    AssetBundleServerData.CopyUpdateAsset(gitServerEditor.gitVerresRoot);
                    break;
                case CopyType.All:
                    AssetBundleServerData.CopyAlleAsset(gitServerEditor.gitVerresRoot);
                    break;
            }
        }



        /** 提交分支 */
        void Step_CommitBranch(bool isAddTag = false)
        {
            Debug.Log("Step_CommitBranch");
            // 切换分支
            if (!CheckoutBranch())
                return;

            Debug.Log("git add .");
            tmp.Clear();
            tmp.WriteLine("cd " + gitRoot);
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
            tmp.WriteLine("cd " + gitRoot);
            tmp.WriteLine(string.Format("git commit -am \"commit {1}\" 2>&1 | tee {0}", Shell.txt_vertmp, branchTag));
            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);
            txt = File.ReadAllText(Shell.txt_vertmp);
            if (txt.IndexOf("error:") != -1 || txt.IndexOf("fatal:") != -1)
            {
                EditorUtility.DisplayDialog(string.Format("git commit -am \"commit {0}\"", branchTag), "错误： 建议先用git工具处理好再执行!\n" + txt, "终止执行");

                stepIsContinue = false;
                return;
            }

            if (isAddTag)
            {
                Debug.Log("git tag -a");
                tmp.Clear();
                tmp.WriteLine("cd " + gitRoot);
                tmp.WriteLine(string.Format("git tag -a {1} -m\"add tag {1}\" 2>&1 | tee {0}", Shell.txt_vertmp, branchTag));
                tmp.Save();
                Shell.RunTmp(Shell.sh_tmp);
                txt = File.ReadAllText(Shell.txt_vertmp);
                if (txt.IndexOf("error:") != -1 || txt.IndexOf("fatal:") != -1)
                {
                    EditorUtility.DisplayDialog(string.Format("git tag -a {0} -m \"add tag {0}\"", branchTag), "错误： 建议先用git工具处理好再执行!\n" + txt, "终止执行");

                    stepIsContinue = false;
                    return;
                }
            }

        }


		void Step_AddTag()
		{
            GameConstConfig gameConstConfig = GameConstConfig.Load(gitServerEditor.gitGameConstPath);

            for(int i = 0; i < centerList.Count; i ++)
			{
                CenterSwitcher.CenterItem item = centerList[i];

				if (item.gitToggle)
				{



                    item.gitTagUse = alreadyExistTagPlan == AlreadyExistTagPlan.Suffix ? item.gitTagLast : item.gitTag;




					gameConstConfig.CenterName = item.name;
                    gameConstConfig.Save(gitServerEditor.gitGameConstPath);

                    string verinfoPath = gitServerEditor.GetGitVerinfoPath(item.name, true);
                    VersionInfo versionInfo = VersionInfo.Load(verinfoPath);
                    versionInfo.version = gameConstConfig.Version;
                    versionInfo.updateLoadUrl = gitServerEditor.GetGitHostUpdateUrlRoot(item.gitTagUse);
                    versionInfo.Save(verinfoPath);


					tmp.Clear();
                    tmp.WriteLine("cd " + gitRoot);

                    if(item.gitTagMaxIndex > 0)
                    {
                        switch(alreadyExistTagPlan)
                        {
                            case AlreadyExistTagPlan.Replace:
                                tmp.WriteLine(string.Format("git tag -d {1} 2>&1 | tee -a {0}", Shell.txt_vertmp, item.gitTag));
                                break;
                        }
                    }

                    tmp.WriteLine(string.Format("git add . 2>&1 | tee -a {0}", Shell.txt_vertmp));
					tmp.WriteLine(string.Format("git commit -am \" 修改GameConstConfig.centerName= {1}\" 2>&1 | tee -a {0}", Shell.txt_vertmp, item.name));

					string cmd = string.Format("git tag -a {1} -m \"add tag {1}\"  2>&1 | tee -a {0}", Shell.txt_vertmp, item.gitTagUse); 
					tmp.WriteLine(cmd);


					tmp.Save();
					Shell.RunTmp(Shell.sh_tmp);

					string txt = File.ReadAllText(Shell.txt_vertmp);
					if (txt.IndexOf("error:") != -1 || txt.IndexOf("fatal:") != -1)
					{
						EditorUtility.DisplayDialog(cmd, "错误： 建议先用git工具处理好再执行!\n" + txt, "终止执行");

						stepIsContinue = false;
						return;
					}
				}
			}
		}


        /** 推送分支 And Tag */
        void Step_PushBranchAndTag()
        {
            // 切换分支
            if (!CheckoutBranch())
                return;


            tmp.Clear(gitNeedPassword);
			tmp.WriteLine("cd " + gitRoot);
			tmp.WriteLine(string.Format("git push origin {0}",  branch)    , gitNeedPassword, gitPassword);

            for(int i = 0; i < centerList.Count; i ++)
			{
                CenterSwitcher.CenterItem item = centerList[i];

				if (item.gitToggle)
				{
					tmp.WriteLine(string.Format("git push origin  :refs/tags/{0}", item.gitTagUse)    , gitNeedPassword, gitPassword);
				}
			}


			tmp.WriteLine(string.Format("git push origin --tags")    , gitNeedPassword, gitPassword);


            tmp.Save();
            Shell.RunFile(Shell.sh_tmp);
        }


        /** 推送分支 */
        void Step_PushBranch()
        {
            // 切换分支
            if (!CheckoutBranch())
                return;


            tmp.Clear(gitNeedPassword);
            tmp.WriteLine("cd " + gitRoot);
            tmp.WriteLine(string.Format("git push origin {0}",  branch)    , gitNeedPassword, gitPassword);
            tmp.WriteLine(string.Format("git push origin --tags")    , gitNeedPassword, gitPassword);
            tmp.Save();
            Shell.RunFile(Shell.sh_tmp);
        }


        /** verinfo 将test转正 */
        void Step_Verinfo_CopyFromTest()
        {

            tmp.Clear(gitNeedPassword);
            tmp.WriteLine("cd " + gitRoot);


            for (int i = 0; i < centerList.Count; i++)
            {
                CenterSwitcher.CenterItem item = centerList[i];

                if (item.gitToggle)
                {


                    string verinfoPath_Test = gitServerEditor.GetGitVerinfoPath(item.name, true);
                    string verinfoPath =gitServerEditor.GetGitVerinfoPath(item.name, false);

                    tmp.WriteLine(string.Format("cp {0} {1}", verinfoPath_Test, verinfoPath));


                }
            }


            tmp.Save();
            Shell.RunTmp(Shell.sh_tmp);
        }



		void ClearAllTagLoacl()
		{

			tmp.Clear();
			tmp.WriteLine("cd " + gitRoot);
			tmp.WriteLine(string.Format("git tag > {0}", Shell.txt_vertmp));
			tmp.Save();
			Shell.RunTmp(Shell.sh_tmp);


			string txt =File.ReadAllText(Shell.txt_vertmp);
			string[] tags = txt.Split('\n');

			tmp.Clear();
			tmp.WriteLine("cd " + gitRoot);
			for(int i = 0; i < tags.Length; i ++)
			{
				if(string.IsNullOrEmpty(tags[i])) continue;

				tmp.WriteLine(string.Format("git tag -d {0} ", tags[i]));
			}


			tmp.Save();
			Shell.RunFile(Shell.sh_tmp);
		}

		void ClearAllTagOrigin()
		{

			tmp.Clear();
			tmp.WriteLine("cd " + gitRoot);

			tmp.WriteLine(string.Format("git pull origin --tag"), gitNeedPassword, gitPassword);
			tmp.WriteLine(string.Format("git tag > {0}", Shell.txt_vertmp));
			tmp.Save();
			Shell.RunTmp(Shell.sh_tmp);


			string txt =File.ReadAllText(Shell.txt_vertmp);
			string[] tags = txt.Split('\n');

			if(!string.IsNullOrEmpty(txt))
			{
                tmp.Clear(gitNeedPassword);
				tmp.WriteLine("cd " + gitRoot);
				for(int i = 0; i < tags.Length; i ++)
				{
					if(string.IsNullOrEmpty(tags[i])) continue;

					tmp.WriteLine(string.Format("git push origin  :refs/tags/{0} ", tags[i]), gitNeedPassword, gitPassword);
				}


				tmp.Save();
				Shell.RunFile(Shell.sh_tmp);
			}

		}




        void LogWarning(string msg)
        {
            Debug.LogFormat("<color=yellow>警告：{0}</color>", msg);
        }


        void Stop()
        {
            EditorApplication.update = null;
        }


    }


}
