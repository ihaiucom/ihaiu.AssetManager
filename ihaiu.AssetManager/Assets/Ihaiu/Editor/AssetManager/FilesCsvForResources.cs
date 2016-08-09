using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

namespace Ihaiu.Assets
{
	public class FilesCsvForResources 
	{

        public static string SerializeFile(string path, string objType)
        {
            return string.Format("{0};{1}", path, objType);
        }

        // path;objType
		public static void Generator()
		{

			List<string> list = new List<string>();
			FindFolder(list, Application.dataPath);
			if(list.Count == 0) return;

            string filesPath = AssetManagerSetting.EditorFileCsvForResource;

			if (File.Exists(filesPath))
			{
				File.Delete(filesPath);
			}

            PathUtil.CheckPath(filesPath, true);

			
            string fileinfo = "";
            string path, objType;

			FileStream fs = new FileStream(filesPath, FileMode.CreateNew);
			StreamWriter sw = new StreamWriter(fs);
            foreach(string itemPath in list)
			{
                List<string> fileList = new List<string>();
                Recursive(fileList, itemPath, false, itemPath + "/", "");
                string resourcePath = itemPath.Replace("\\", "/") + "/";

                int count = fileList.Count;
                for (int i = 0; i < count; i++) 
				{
                    if(i % 20 == 0) EditorUtility.DisplayProgressBar("生成文件信息", resourcePath, 1f * i / count);

					string file = fileList[i];
					string ext = Path.GetExtension(file);
					if (ext.Equals(".meta")) continue;
					string filename = Path.GetFileName(file);
					if(filename.Equals(".DS_Store")) continue;
					if(filename.Equals("files.csv")) continue;

                    path = file.Replace(resourcePath , string.Empty);
                    if(!string.IsNullOrEmpty(ext)) path = path.Replace(ext, string.Empty);


                    objType = "";
                    if (Resources.Load<Sprite>(path) != null)
                    {
                        objType = "Sprite";
                    }

                    fileinfo = SerializeFile(path, objType);
					
                    sw.WriteLine(fileinfo);
				}

			}

			sw.Close(); fs.Close();
			AssetDatabase.Refresh();
            Debug.Log("[FilesCsvForResources]" + filesPath);
            EditorUtility.ClearProgressBar();
		}


		public static void FindFolder(List<string> list, string path, string folderName="Resources", bool stopChild = true)
		{

			DirectoryInfo dir = new DirectoryInfo(path);
			if(dir.Name.Equals(folderName))
			{
				list.Add(dir.FullName);
				if(stopChild) return;
			}

			string[] dirs = Directory.GetDirectories(path);
            int index = 0;
            int count = dirs.Length;
			foreach(string item in dirs)
			{
                if(index % 10 == 0) EditorUtility.DisplayProgressBar("遍历目录", path, 1f *(index ++) / count);
				FindFolder(list, item, folderName, stopChild);
            }
            EditorUtility.ClearProgressBar();
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