using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

/// <summary> 
/// 适用与ZIP压缩 
/// </summary> 
public class ZipHelper
{

    private static void SetCode()
    {
        // 需要注意的是 默认编码如果有中文的文件名或目录将会乱码，甚至解压的数据出错。所以设置一下支持中文的编码
        ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;
    }


    private static List<string> ignoreExts = new List<string>(new string[]{ ".meta", ".manifest"});
    private static List<string> ignoreFiles = new List<string>(new string[]{ ".ds_store"});

    #region 压缩

    /// <summary> 
    /// 递归压缩文件夹的内部方法 
    /// </summary> 
    /// <param name="folderToZip">要压缩的文件夹路径</param> 
    /// <param name="zipStream">压缩输出流</param> 
    /// <param name="parentFolderName">此文件夹的上级文件夹</param> 
    /// <returns></returns> 
    private static bool ZipDirectory(string folderToZip, ZipOutputStream zipStream, string parentFolderName)
    {
        SetCode();

        bool result = true;
        string[] folders, files;
        ZipEntry ent = null;
        FileStream fs = null;
        FileInfo fileInfo = null;
        Crc32 crc = new Crc32();

        try
        {
            folders = Directory.GetDirectories(folderToZip, "*", SearchOption.AllDirectories);
            files = Directory.GetFiles(folderToZip, "*.*", SearchOption.AllDirectories)
                .Where(s => !ignoreExts.Contains(Path.GetExtension(s).ToLower()) && !ignoreFiles.Contains(Path.GetFileName(s).ToLower()) ).ToArray();


			parentFolderName = folderToZip;
			parentFolderName = parentFolderName.Replace("\\", "/");
            if (!parentFolderName.EndsWith("/"))
                parentFolderName += "/";

            // 可以不需要压入文件夹。压人后在Mac下用默认的zip工具解压不了
//                foreach(string folder in folders)
//                {
//                    ent = new ZipEntry(folder.Replace(parentFolderName, "") + "/");
//                    zipStream.PutNextEntry(ent);
//                    zipStream.Flush();
//                }


            foreach(string file in files)
			{
                fileInfo = new FileInfo(file);
                fs = File.OpenRead(file);

                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
				ent = new ZipEntry(file.Replace("\\", "/").Replace(parentFolderName, ""));
                ent.DateTime = fileInfo.LastWriteTime;
                ent.Size = fs.Length;

                fs.Close();
                
                crc.Reset();
                crc.Update(buffer);

                ent.Crc = crc.Value;
                zipStream.PutNextEntry(ent);
                zipStream.Write(buffer, 0, buffer.Length);
            }
            
             
        }
        catch
        {
            result = false;
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
            if (ent != null)
            {
                ent = null;
            }
            GC.Collect();
            GC.Collect(1);
        }
        return result;
    }

    /// <summary> 
    /// 压缩文件夹  
    /// </summary> 
    /// <param name="folderToZip">要压缩的文件夹路径</param> 
    /// <param name="zipedFile">压缩文件完整路径</param> 
    /// <param name="password">密码</param> 
    /// <returns>是否压缩成功</returns> 
    public static bool ZipDirectory(string folderToZip, string zipedFile, string password)
    {
        bool result = false;
        if (!Directory.Exists(folderToZip))
            return result;

        ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile));
        zipStream.SetLevel(9);
        if (!string.IsNullOrEmpty(password)) zipStream.Password = password;


        string fullPath = Path.GetFullPath(folderToZip);
        string parentPath = Path.GetFullPath(fullPath + "/../");

        result = ZipDirectory(fullPath, zipStream, parentPath);

        zipStream.Finish();
        zipStream.Close();

        return result;
    }

    /// <summary> 
    /// 压缩文件夹 
    /// </summary> 
    /// <param name="folderToZip">要压缩的文件夹路径</param> 
    /// <param name="zipedFile">压缩文件完整路径</param> 
    /// <returns>是否压缩成功</returns> 
    public static bool ZipDirectory(string folderToZip, string zipedFile)
    {
        bool result = ZipDirectory(folderToZip, zipedFile, null);
        return result;
    }

    /// <summary> 
    /// 压缩文件 
    /// </summary> 
    /// <param name="fileToZip">要压缩的文件全名</param> 
    /// <param name="zipedFile">压缩后的文件名</param> 
    /// <param name="password">密码</param> 
    /// <returns>压缩结果</returns> 
    public static bool ZipFile(string fileToZip, string zipedFile, string password)
    {
        SetCode();

        bool result = true;
        ZipOutputStream zipStream = null;
        FileStream fs = null;
        ZipEntry ent = null;

        if (!File.Exists(fileToZip))
            return false;

        try
        {
            fs = File.OpenRead(fileToZip);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            fs = File.Create(zipedFile);
            zipStream = new ZipOutputStream(fs);
            if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
            ent = new ZipEntry(Path.GetFileName(fileToZip));
            zipStream.PutNextEntry(ent);
            zipStream.SetLevel(6);

            zipStream.Write(buffer, 0, buffer.Length);

        }
        catch
        {
            result = false;
        }
        finally
        {
            if (zipStream != null)
            {
                zipStream.Finish();
                zipStream.Close();
            }
            if (ent != null)
            {
                ent = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
        }
        GC.Collect();
        GC.Collect(1);

        return result;
    }

    /// <summary> 
    /// 压缩文件 
    /// </summary> 
    /// <param name="fileToZip">要压缩的文件全名</param> 
    /// <param name="zipedFile">压缩后的文件名</param> 
    /// <returns>压缩结果</returns> 
    public static bool ZipFile(string fileToZip, string zipedFile)
    {
        bool result = ZipFile(fileToZip, zipedFile, null);
        return result;
    }

    /// <summary> 
    /// 压缩文件或文件夹 
    /// </summary> 
    /// <param name="fileToZip">要压缩的路径</param> 
    /// <param name="zipedFile">压缩后的文件名</param> 
    /// <param name="password">密码</param> 
    /// <returns>压缩结果</returns> 
    public static bool Zip(string fileToZip, string zipedFile, string password)
    {
        bool result = false;
        if (Directory.Exists(fileToZip))
            result = ZipDirectory(fileToZip, zipedFile, password);
        else if (File.Exists(fileToZip))
            result = ZipFile(fileToZip, zipedFile, password);

        return result;
    }

    /// <summary> 
    /// 压缩文件或文件夹 
    /// </summary> 
    /// <param name="fileToZip">要压缩的路径</param> 
    /// <param name="zipedFile">压缩后的文件名</param> 
    /// <returns>压缩结果</returns> 
    public static bool Zip(string fileToZip, string zipedFile)
    {
        bool result = Zip(fileToZip, zipedFile, null);
        return result;

    }

    #endregion

    #region 解压

    /// <summary> 
    /// 解压功能(解压压缩文件到指定目录) 
    /// </summary> 
    /// <param name="fileToUnZip">待解压的文件</param> 
    /// <param name="zipedFolder">指定解压目标目录</param> 
    /// <param name="password">密码</param> 
    /// <returns>解压结果</returns> 
    public static bool UnZip(string zipFilePath, string unZipDir, string password)
    {

        try
        {
            SetCode();

            if (zipFilePath == string.Empty)
            {
                throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("压缩文件不存在！");
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            
            if (!unZipDir.EndsWith("/"))
                unZipDir += "/";

            UnZip(File.OpenRead(zipFilePath), unZipDir, password);
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e);
            return false;
        }

        return true;
    }

    public static bool UnZip(Stream baseInputStream, string unZipDir, string password)
    {
        try
        {
            SetCode();

            if (!unZipDir.EndsWith("/"))
                unZipDir += "/";

            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            using (var s = new ZipInputStream(baseInputStream))
            {
                if(!string.IsNullOrEmpty(password)) s.Password = password;

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    if (directoryName != null && !directoryName.EndsWith("/"))
                    {
                    }
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                        {

                            int size;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e);
            return false;
        }

        return true;
    }

    /// <summary> 
    /// 解压功能(解压压缩文件到指定目录) 
    /// </summary> 
    /// <param name="fileToUnZip">待解压的文件</param> 
    /// <param name="zipedFolder">指定解压目标目录</param> 
    /// <returns>解压结果</returns> 
    public static bool UnZip(string fileToUnZip, string zipedFolder)
    {
        bool result = UnZip(fileToUnZip, zipedFolder, null);
        return result;
    }


    public static bool UnZip(Stream baseInputStream, string unZipDir)
    {
        return UnZip(baseInputStream, unZipDir, null);
    }


    public static bool UnZip(byte[] data, string unZipDir)
    {
        MemoryStream stream = new MemoryStream(data);
        stream.Position = 0;
        bool result =  UnZip(stream, unZipDir, null);
        stream.Dispose();
        stream.Close();
        return result;
    }
    #endregion
}