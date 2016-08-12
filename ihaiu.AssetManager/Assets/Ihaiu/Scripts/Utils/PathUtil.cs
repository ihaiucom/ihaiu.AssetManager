using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


public class PathUtil
{
    

    public static void ClearAllPlatformDirctory()
    {
        foreach(RuntimePlatform p in Platform.runtimePlatformEnums)
        {
            ClearPlatformDirctory(p);
        }
    }


    public static void ClearOtherPlatformDirctory(RuntimePlatform p)
    {
        foreach(RuntimePlatform o in Platform.runtimePlatformEnums)
        {
            if (o == p)
                continue;
            
            ClearPlatformDirctory(o);
        }
    }


    public static void ClearTestData()
    {
        string root = Application.streamingAssetsPath;
        string path;
        path = root + "/users";
        DeleteDirectory(path);

        string[] names = new string[]{
            "crash_report.log", 
            "test_record.json", 
            "test_record.bin", 
            "test_rscdata.bin", 
            "test_WarEnterData.json", 
            "AssetBundleList.csv", 
            "version_osx.json", 
            "version_ios.json",
            "version_android.json",
        };
        foreach(string name in names)
        {
            path = root + "/" + name;
            DeleteFile(path);
        }
    }


    public static void ClearPlatformDirctory(RuntimePlatform p)
    {
        PathUtil.ClearDirectory( Application.streamingAssetsPath + "/" + Platform.GetPlatformDirectory(p, true));
    }

    public static void ClearDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            CheckPath(path, false);
        }
    }


	public static void DeleteDirectory(string path)
	{
		if (!Directory.Exists (path))
		{
			return;
		}

		string[] names = Directory.GetFiles(path);
		string[] dirs = Directory.GetDirectories(path);

		
		foreach (string dir in dirs) {
			DeleteDirectory(dir);
		}


		foreach (string filename in names) {
			File.Delete(filename);
		}

		
		Directory.Delete(path);
	}

    public static void DeleteFile(string path)
    {
        if(File.Exists(path)) File.Delete(path);
    }

	public static string GetDirectoryName(string path, int upCount = 0)
	{
		for(int i = 0; i < upCount; i ++)
		{
			path = Path.GetDirectoryName(path);
		}
		return Path.GetFileName(Path.GetDirectoryName(path));
	}



	public static void CheckPath(string path, bool isFile = true)
	{
		if(isFile) path = path.Substring(0, path.LastIndexOf('/'));
		string[] dirs = path.Split('/');
		string target = "";
		
		bool first = true;
		foreach(string dir in dirs)
		{
			if(first)
			{
				first = false;
				target += dir;
				continue;
			}
			
			if(string.IsNullOrEmpty(dir)) continue;
			target +="/"+ dir;
			if(!Directory.Exists(target))
			{
				Directory.CreateDirectory(target);
			}
		}
	}

	public static string ChangeExtension(string path, string ext)
	{
		string e = Path.GetExtension(path);
		if(string.IsNullOrEmpty(e))
		{
			return path + ext;
		}

		bool backDSC = path.IndexOf('\\') != -1;
		path = path.Replace('\\', '/');
		if(path.IndexOf('/') == -1)
		{
			return path.Substring(0, path.LastIndexOf('.')) + ext;
		}

		string dir = path.Substring(0, path.LastIndexOf('/'));
		string name = path.Substring(path.LastIndexOf('/'), path.Length - path.LastIndexOf('/'));
		name = name.Substring(0, name.LastIndexOf('.')) + ext;
		path = dir + name;

		if (backDSC)
		{
			path = path.Replace('/', '\\');
		}
		return path;
	}


	
	/// <summary>
	/// 计算字符串的MD5值
	/// </summary>
	public static string md5(string source) {
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
		byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
		md5.Clear();
		
		string destString = "";
		for (int i = 0; i < md5Data.Length; i++) {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
		}
		destString = destString.PadLeft(32, '0');
		return destString;
	}
	
	/// <summary>
	/// 计算文件的MD5值
	/// </summary>
	public static string md5file(string file) {
		try {
			FileStream fs = new FileStream(file, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(fs);
			fs.Close();
			
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
			}
            return sb.ToString();
		} catch (Exception ex) {
			throw new Exception("md5file() fail, error:" + ex.Message);
		}
	}




	public static string[] FindDirectory(string path)
	{
		return System.IO.Directory.GetFileSystemEntries(Application.dataPath + "/" + path);
	}

    /** 遍历目录下文件 */
    public static void RecursiveFile(string path, List<string> fileList, List<string> exts = null)
    {


        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        bool isCheckExt = exts != null && exts.Count > 0;
        foreach (string filename in names) 
        {
            if (isCheckExt)
            {
                string ext = Path.GetExtension(filename).ToLower();
                if (!exts.Contains(ext))
                    continue;
            }


            string fn = Path.GetFileName(filename);
            if(fn.Equals(".DS_Store")) continue;

            string file = filename.Replace('\\', '/');
            fileList.Add(file);
        }

        #if UNITY_EDITOR
        int count = dirs.Length;
        int index = 0;
        #endif


        foreach (string dir in dirs) 
        {

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar("遍历目录", path, 1f *(index ++) / count);
            #endif

            RecursiveFile(dir, fileList, exts);
        }


        #if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
        #endif
    }

    /** 遍历目录下文件 */
    public static void RecursiveFile(List<string> paths, List<string> fileList, List<string> exts = null)
    {
        RecursiveFile(paths.ToArray(), fileList, exts);
    }

    public static void RecursiveFile(string[] paths, List<string> fileList, List<string> exts = null)
    {
        for (int i = 0; i < paths.Length; i ++)
        {
            PathUtil.RecursiveFile(paths[i], fileList, exts);
        }
    }


    /** 遍历目录下文件,过滤文件夹名 */
    public static void RecursiveFileFilter(string path, List<string> fileList, List<string> filterexts = null, List<string> filterFolderNames = null) {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        bool isCheckExt = filterexts != null && filterexts.Count > 0;
        foreach (string filename in names) 
        {
            if (isCheckExt)
            {
                string ext = Path.GetExtension(filename).ToLower();
                if (filterexts.Contains(ext))
                    continue;
            }


            string fn = Path.GetFileName(filename);
            if(fn.Equals(".DS_Store")) continue;
            if(fn.IndexOf(".") == 0) continue;

            string file = filename.Replace('\\', '/');
            fileList.Add(file);
        }

        bool isCheckFilter = filterFolderNames != null && filterFolderNames.Count > 0;
        foreach (string dir in dirs) 
        {

            string dirName = Path.GetFileName(dir).ToLower();
            if(dirName.IndexOf(".") == 0) continue;
            if (isCheckFilter)
            {
                if (filterFolderNames.Contains(dirName))
                    continue;
            }
            
            RecursiveFileFilter(dir, fileList, filterexts, filterFolderNames);
        }
    }



    /** 查找path下文件夹名叫folderName的所有文件夹路径 */
    public static void FindFolder(List<string> list, string path, string folderName="Resources", bool stopChild = true)
    {

        DirectoryInfo dir = new DirectoryInfo(path);
        if(dir.Name.Equals(folderName))
        {
            list.Add(dir.FullName);
            if(stopChild) return;
        }

        string[] dirs = Directory.GetDirectories(path);
        foreach(string item in dirs)
        {
            FindFolder(list, item, folderName, stopChild);
        }
    }


    /** 查找rootPath下文件夹名叫folderName的所有指定后缀名的文件 */
    public static List<string> GetAllFolderFiels(string rootPath, string folderName, List<string> exts)
    {
        List<string> dirs = new List<string>();

        PathUtil.FindFolder(dirs, rootPath, folderName);

        List<string> fileList = new List<string>();

        foreach (string path in dirs)
        {
            PathUtil.RecursiveFile(path, fileList, exts);
        }

        return fileList;
    }

    /** 获取所有Resource下的资源 */
    public static List<string> GetAllResourcesFiels()
    {
        return GetAllFolderFiels(Application.dataPath, "Resources", new List<string>(new string[]{".prefab", ".png", ".jpg", ".jpeg", ".bmp", "gif", ".tga", ".tiff", ".psd", ".mat", ".mp3", ".wav"}));
    }


    /** 获取所有Resource下的资源 */
    public static List<string> GetAllGameResourcesFiels()
    {
        List<string> exts = new List<string>(new string[]{ ".prefab", ".png", ".jpg", ".jpeg", ".bmp", "gif", ".tga", ".tiff", ".psd", ".mat", ".mp3", ".wav" });

        List<string> dirs = new List<string>();
        dirs.Add("Assets/Game/Arts_Modules/Wars/Resources");
        dirs.Add("Assets/Game/Resources");

        List<string> fileList = new List<string>();

        foreach (string path in dirs)
        {
            PathUtil.RecursiveFile(path, fileList, exts);
        }

//
//        #if UNITY_EDITOR
//        UnityEditor.EditorUtility.ClearProgressBar();
//        #endif

        return fileList;
    }


    public static void CopyDirectory(string fromPath, string toPath, List<string> filterexts = null, List<string> filterFolderNames = null)
    {
        PathUtil.CheckPath(toPath, false);

        string[] names = Directory.GetFiles(fromPath);
        string[] dirs = Directory.GetDirectories(fromPath);
        bool isCheckExt = filterexts != null && filterexts.Count > 0;
        foreach (string filename in names) 
        {
            if (isCheckExt)
            {
                string ext = Path.GetExtension(filename).ToLower();
                if (filterexts.Contains(ext))
                    continue;
            }


            string fn = Path.GetFileName(filename);
            if(fn.Equals(".DS_Store")) continue;
            if(fn.IndexOf(".") == 0) continue;

            string file = filename.Replace('\\', '/');
            string dist = file.Replace(fromPath, toPath);


            File.Copy(file, dist, true);
        }

        #if UNITY_EDITOR
        int count = dirs.Length;
        int index = 0;
        #endif

        bool isCheckFilter = filterFolderNames != null && filterFolderNames.Count > 0;
        foreach (string dir in dirs) 
        {
            string dirName = Path.GetFileName(dir);
            string dirNameLower = dirName.ToLower();
            if(dirNameLower.IndexOf(".") == 0) continue;
            if (isCheckFilter)
            {
                if (filterFolderNames.Contains(dirNameLower))
                    continue;
            }


            #if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar("拷贝目录", dir, 1f *(index ++) / count);
            #endif

            CopyDirectory(dir, toPath + "/" + dirName, filterexts, filterFolderNames);
        }


        #if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
        #endif
    }


}
