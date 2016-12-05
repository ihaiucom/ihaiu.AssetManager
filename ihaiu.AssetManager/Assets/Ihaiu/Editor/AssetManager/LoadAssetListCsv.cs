using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace com.ihaiu
{
    public class LoadAssetListCsv 
    {
        public static void Generator()
        {
            string filesPath = AssetManagerSetting.EditorLoadAssetListPath;

            if (File.Exists(filesPath))
            {
                File.Delete(filesPath);
            }

            PathUtil.CheckPath(filesPath, true);


            FileStream fs = new FileStream(filesPath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);


            // Resources
            Resources(sw);

            // StreamingAssets
            StreamingAssets(sw);


            sw.Close(); fs.Close();
            AssetDatabase.Refresh();
            Debug.Log("[AssetListCsv]" + filesPath);
            EditorUtility.ClearProgressBar();



            DontUnloadAssetListCsv.Generator();
        }

        static void Resources(StreamWriter sw)
        {
            if (!Directory.Exists(AssetManagerSetting.EditorRootResources))
                return;

            List<string> list = new List<string>();
            //            FindFolder(list, Application.dataPath);
            list.Add(AssetManagerSetting.EditorRootResources);
            if(list.Count == 0) return;


            string fileinfo = "";
            string path, objType;

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
                    if (path.IndexOf("map/terrain") != -1)
                    {
                        objType = AssetManagerSetting.ObjType_Texture;
                    }
                    else
                    {
                        TextureImporter textureImporter = TextureImporter.GetAtPath(file) as TextureImporter;
                        if (textureImporter != null && textureImporter.textureType == TextureImporterType.Sprite)
                        {
                            objType = AssetManagerSetting.ObjType_Sprite;
                        }
                    }

                    fileinfo = SerializeFile(AssetLoadType.Resources, path, objType);

                    sw.WriteLine(fileinfo);
                }

            }

        }




        static void StreamingAssets(StreamWriter sw)
        {
            string resourceRoot = AssetBundleEditor.resourceRoot;

            List<string> list = new List<string>();
            PathUtil.RecursiveFile(AssetBundleEditor.resourceRoot, list, AssetBundleEditor.exts);

            string fileinfo, filepath, path, objType, assetBundleName, assetName;
            for(int i = 0; i < list.Count; i ++)
            {
                filepath = list[i];
                AssetImporter importer = AssetImporter.GetAtPath(filepath);

                if (string.IsNullOrEmpty(importer.assetBundleName))
                {
                    Debug.LogWarningFormat("MResource资源没有设置AssetBundleName  path={0}", filepath);
                    //                    continue;
                }

                path = filepath.Replace(resourceRoot, "{0}");
                path = PathUtil.ChangeExtension(path, string.Empty);

                assetBundleName     = importer.assetBundleName;
                assetName           = PathUtil.ChangeExtension(Path.GetFileName(filepath), string.Empty);



                objType = string.Empty;
                string ext = Path.GetExtension(filepath).ToLower();
                if (ext == ".prefab")
                {
                    objType = AssetManagerSetting.ObjType_GameObject;
                }
                else if (filepath.IndexOf("map/terrain") != -1)
                {
                    objType = AssetManagerSetting.ObjType_Texture;
                }
                else
                {
                    TextureImporter textureImporter = TextureImporter.GetAtPath(filepath) as TextureImporter;
                    if (textureImporter != null && textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        objType = AssetManagerSetting.ObjType_Sprite;
                    }
                }


                fileinfo = SerializeFile(AssetLoadType.AssetBundle, path, objType, assetBundleName, assetName, ext);

                sw.WriteLine(fileinfo);
            }

        }


        public static string SerializeFile(AssetLoadType loadType, string path, string objType, string assetBundleName, string assetName, string ext)
        {
            return string.Format("{0};{1};{2};{3};{4};{5}", (int)loadType, path, objType, assetBundleName, assetName, ext);
        }

        // loadType, path;objType
        public static string SerializeFile(AssetLoadType loadType, string path, string objType)
        {
            return string.Format("{0};{1};{2}", (int)loadType, path, objType);
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