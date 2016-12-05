using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace com.ihaiu
{
    public class FilesCsvForStreamingAssets 
    {


        public static string SerializeFile(string path, string md5)
        {
            return string.Format("{0};{1}", path, md5);
        }

        public static void Generator()
        {
            List<string> fileList = new List<string>();

            string platformRoot = AssetManagerSetting.EditorRootPlatform;

            string filecsv = AssetManagerSetting.EditorFileCsvForStreaming;
            if (File.Exists(filecsv)) File.Delete(filecsv);
            PathUtil.CheckPath(filecsv);


            FileStream fs = new FileStream(filecsv, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);

            string fileinfo;
            string path, md5;
            string file;




            // streamingAssetsPath下非platform
            fileList.Clear();
            if (Directory.Exists(Application.streamingAssetsPath))
            {
                PathUtil.RecursiveFileFilter(Application.streamingAssetsPath, fileList, new List<string>(new string[]{ ".meta" }), new List<string>(new string[]{ "platform", "users" }));

                for (int i = 0; i < fileList.Count; i++)
                {
                    file = fileList[i];
                    if (file.IndexOf("test_") != -1 
                        || file.IndexOf("crash_report") != -1 
                        || file.IndexOf("AssetBundleList.csv") != -1 
                        || file.IndexOf("AssetList.csv") != -1
                        || file.IndexOf("version_") != -1
                    )
                        continue;



                    path = file.Replace(Application.streamingAssetsPath + "/", string.Empty);
                    md5 = PathUtil.md5file(file);
                    fileinfo = SerializeFile(path, md5);
                    sw.WriteLine(fileinfo);
                }
            }



            // streamingAssetsPath下platform
            fileList.Clear();
            Recursive(platformRoot, fileList);

            int count = fileList.Count;
            for (int i = 0; i < count; i++)
            {
                if (i % 20 == 0)
                    EditorUtility.DisplayProgressBar("生成文件MD5", i + "/" + count, 1f * i / count);
                file = fileList[i];
                path = file.Replace(platformRoot, "{0}");
                md5 = PathUtil.md5file(file);


                fileinfo = SerializeFile(path, md5);
                sw.WriteLine(fileinfo);
            }

            List<string> others = new List<string>();
            others.Add(AssetManagerSetting.EditorLoadAssetListPath);
            others.Add(AssetManagerSetting.EditorDontUnloadAssetListPath);

            for(int i = 0; i < others.Count; i ++)
            {
                file = others[i];
                path = file.Replace(platformRoot, "{0}");
                md5 = PathUtil.md5file(file);
                fileinfo = SerializeFile(path, md5);
                sw.WriteLine(fileinfo);
            }




            sw.Close(); fs.Close();
            AssetDatabase.Refresh();
            Debug.Log("[FilesGenerator]" + filecsv);

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        static void Recursive(string path, List<string> fileList) {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names) {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta")) continue;
                if (ext.Equals(".manifest")) continue;



                string fn = Path.GetFileName(filename);
                if(fn.Equals("files.csv")) continue;
                if(fn.Equals("UpdateAssetList.csv")) continue;
                if(fn.Equals("LoadAssetList.csv")) continue;
                if(fn.Equals(".DS_Store")) continue;
                if(fn.IndexOf(".") == 0) continue;
                if(fn.IndexOf("test_") == 0) continue;

                fileList.Add(filename.Replace('\\', '/'));
            }
            foreach (string dir in dirs) {
                string dirName = Path.GetFileName(dir);
                if(dirName.IndexOf(".") == 0) continue;
                if(dirName.IndexOf("users") == 0) continue;
                Recursive(dir, fileList);
            }
        }





        public static void CopyStreamFilesCsvToVersion(Version version)
        {
            string path = AssetManagerSetting.EditorGetVersionFileListPath(version.ToString());
            PathUtil.CheckPath(path);
            File.Copy(AssetManagerSetting.EditorFileCsvForStreaming, path);
        }


        public static void GeneratorUpdateList(Version appVer)
        {

            string path = AssetManagerSetting.EditorUpdateAssetListPath;
            Debug.Log(path);
            PathUtil.CheckPath(path);


            if (appVer == null)
            {
                if (File.Exists(path))
                    File.Delete(path);
                File.Copy(AssetManagerSetting.EditorFileCsvForStreaming, path);
                return;
            }
            Debug.Log(appVer);
            AssetFileList app = AssetFileList.Read(AssetManagerSetting.EditorGetVersionFileListPath(appVer.ToString()));
            AssetFileList curr = AssetFileList.Read(AssetManagerSetting.EditorFileCsvForStreaming);

            AssetFileList diff = AssetFileList.DiffAssetFileList(app, curr);
            diff.Save(path);
        }






    }
}