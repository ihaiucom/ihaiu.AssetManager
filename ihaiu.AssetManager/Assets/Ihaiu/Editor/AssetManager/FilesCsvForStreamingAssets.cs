using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace Ihaiu.Assets
{
	public class FilesCsvForStreamingAssets 
	{
        
        public static string SerializeFile(string path, string md5, string objType, string assetBundleName, string assetName)
        {
            return string.Format("{0};{1};{2};{3};{4}", path, md5, objType, assetBundleName, assetName);
        }

        public static string SerializeFile(string path, string md5)
        {
            return string.Format("{0};{1}", path, md5);
        }

        //path;md5;objType;assetName;assetBundleName;manifestName
        public static void Generator(bool developMode = false)
        {
            List<string> fileList = new List<string>();

            string platformRoot = AssetManagerSetting.EditorRootPlatform;

            string filecsv = AssetManagerSetting.EditorFileCsvForStreaming;
            if (File.Exists(filecsv)) File.Delete(filecsv);
            PathUtil.CheckPath(filecsv);


            FileStream fs = new FileStream(filecsv, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);

            string fileinfo;
            string path, md5, objType, assetName, assetBundleName;
            string file;


            AssetBundleInfoList infoList = AssetBundleEditor.GeneratorAssetBundleInfo();
            if (developMode)
            {
                for(int i = 0; i < infoList.list.Count; i ++)
                {
                    AssetBundleInfo item = infoList.list[i];
                    fileinfo = SerializeFile("{0}/" + item.assetBundleName, "", item.objType, item.assetBundleName, item.assetName);
                    sw.WriteLine(fileinfo);
                }
            }
            else
            {

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

                // config.assetbunld, luacode.assetbunld
                List<string> cl = new List<string>();

                string[] byteFiles = new string[]{ "config", "luacode" };
                for (int i = 0; i < byteFiles.Length; i++)
                {
                    file = platformRoot + "/" + byteFiles[i] + AssetManagerSetting.AssetbundleExt;
                    if (!File.Exists(file))
                        continue;
                
                    cl.Add(file);
                    path = file.Replace(platformRoot, "{0}");
                    md5 = PathUtil.md5file(file);
                    fileinfo = SerializeFile(path, md5);
                    sw.WriteLine(fileinfo);
                }


                // streamingAssetsPath下platform
                fileList.Clear();
                Recursive(platformRoot, fileList);

          

                objType = "";
                int count = fileList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i % 20 == 0)
                        EditorUtility.DisplayProgressBar("生成文件MD5", i + "/" + count, 1f * i / count);
                    file = fileList[i];
                    path = file.Replace(platformRoot, "{0}");
                    assetBundleName = file.Replace(platformRoot + "/", string.Empty);
                    md5 = PathUtil.md5file(file);

                    if (infoList.Has(assetBundleName))
                    {
                        AssetBundleInfo info = infoList.Get(assetBundleName);
                        assetName = info.assetName;
                        objType = info.objType;
                    }
                    else
                    {

                        assetName = Path.GetFileName(file);
                        if (path.IndexOf("{0}/assets") == 0)
                        {
                            assetName = "";
                        }
                        else if (path == "{0}/" + Platform.PlatformDirectoryName)
                        {
                            assetName = "assetbundlemanifest";
                        }
                        else
                        {
                            assetName = assetName.Replace(AssetManagerSetting.AssetbundleExt, string.Empty);
                            assetName = PathUtil.ChangeExtension(assetName, string.Empty);
                        }
                    }


                    fileinfo = SerializeFile(path, md5, objType, assetBundleName, assetName);
                    sw.WriteLine(fileinfo);
                }
            }
           


            fileinfo = SerializeFile("{0}/" + AssetManagerSetting.FilesName, DateTime.Now.ToString("yyyyMMddHHmmss"));
            sw.WriteLine(fileinfo);


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
                if(fn.Equals("config" + AssetManagerSetting.AssetbundleExt)) continue;
                if(fn.Equals("luacode" + AssetManagerSetting.AssetbundleExt)) continue;
                if(fn.Equals("files.csv")) continue;
                if(fn.Equals("UpdateAssetList.csv")) continue;
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