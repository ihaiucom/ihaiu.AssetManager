using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace com.ihaiu
{
    public class DontUnloadAssetListCsv 
    {
        public static void Generator()
        {
            string filesPath = AssetManagerSetting.EditorDontUnloadAssetListPath;

            if (File.Exists(filesPath))
            {
                File.Delete(filesPath);
            }

            PathUtil.CheckPath(filesPath, true);

            List<string> folders = new List<string>();
            folders.Add("module");
            folders.Add("shaders");
            folders.Add("sound");
            folders.Add("Materials");


            FileStream fs = new FileStream(filesPath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);


            // Resources
            Resources(sw, AssetManagerSetting.EditorRootResources, folders);

            // StreamingAssets
            Resources(sw, AssetManagerSetting.EditorRootMResources, folders);


            sw.Close(); fs.Close();
            AssetDatabase.Refresh();
            Debug.Log("[AssetListCsv]" + filesPath);
            EditorUtility.ClearProgressBar();
        }

        static void Resources(StreamWriter sw, string root, List<string> folders)
        {
            string path;


            List<string> list = new List<string>();
            for(int i = 0; i < folders.Count; i ++)
            {
                path = Path.Combine(root, folders[i]);
                if (Directory.Exists(path))
                {
                    list.Add(path);
                }
            }
            if(list.Count == 0) return;



            foreach(string itemPath in list)
            {
                List<string> fileList = new List<string>();
                Recursive(fileList, itemPath, false, root + "/", "");
                string resourcePath = root.Replace("\\", "/") + "/";

                int count = fileList.Count;
                for (int i = 0; i < count; i++) 
                {
                    if(i % 20 == 0) EditorUtility.DisplayProgressBar("生成文件信息", itemPath, 1f * i / count);

                    string file = fileList[i];
                    string ext = Path.GetExtension(file);
                    if (ext.Equals(".meta")) continue;
                    string filename = Path.GetFileName(file);
                    if(filename.Equals(".DS_Store")) continue;
                    if(filename.Equals("files.csv")) continue;

                    path = file.Replace(resourcePath , string.Empty);
                    if(!string.IsNullOrEmpty(ext)) path = path.Replace(ext, string.Empty);

                    path = path.ToLower() + ";";

                    sw.WriteLine(path);
                }

            }

        }








        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        static void Recursive(List<string> fileList, string path, bool replaceRoot=false, string root="", string replace="") {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names) {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta")) continue;


                string fn = Path.GetFileName(filename);
                if(fn.Equals(".DS_Store")) continue;
                if(fn.IndexOf(".") == 0) continue;

                string filepath = filename;
                if(replaceRoot)
                {
                    filepath = filepath.Replace(root, replace);
                }

                fileList.Add(filepath.Replace('\\', '/'));
            }
            foreach (string dir in dirs) {
                //paths.Add(dir.Replace('\\', '/'));

                string dirName = Path.GetFileName(dir);
                if(dirName.IndexOf(".") == 0) continue;
                Recursive(fileList, dir, replaceRoot, root, replace);
            }
        }







    }
}