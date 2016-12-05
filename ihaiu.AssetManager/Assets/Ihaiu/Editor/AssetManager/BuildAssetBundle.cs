using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace com.ihaiu
{
    public partial class AB 
    {

        private static string SKey = "zengfeng";

        public static byte[] EncryptBytes(byte[] data, string Skey)  
        {  
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();  
            DES.Key = ASCIIEncoding.ASCII.GetBytes(Skey);  
            DES.IV = ASCIIEncoding.ASCII.GetBytes(Skey);  
            ICryptoTransform desEncrypt = DES.CreateEncryptor();  
            byte[] result = desEncrypt.TransformFinalBlock(data, 0, data.Length);  
            return result;
        }




        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        static void Recursive(string path, List<string> fileList) {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names) 
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta")) continue;


                string fn = Path.GetFileName(filename);
                if(fn.Equals(".DS_Store")) continue;

                string file = filename.Replace('\\', '/');
                fileList.Add(file);
            }

            foreach (string dir in dirs) 
            {
                Recursive(dir, fileList);
            }
        }

        static void FindPrefab(string path, List<string> fileList)
        {
            string[] names = Directory.GetFiles(path);
            foreach (string filename in names) 
            {
                string ext = Path.GetExtension(filename);
                if (!ext.Equals(".prefab")) continue;


                string fn = Path.GetFileName(filename);
                if(fn.Equals(".DS_Store")) continue;

                string file = filename.Replace('\\', '/');
                fileList.Add(file);
            }
        }

        static void RecursivePrefab(string path, List<string> fileList) {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names) 
            {
                string ext = Path.GetExtension(filename);
                if (!ext.Equals(".prefab")) continue;


                string fn = Path.GetFileName(filename);
                if(fn.Equals(".DS_Store")) continue;

                string file = filename.Replace('\\', '/');
                fileList.Add(file);
            }

            foreach (string dir in dirs) 
            {
                RecursivePrefab(dir, fileList);
            }
        }


        static void RecursiveLua(string path, List<string> fileList) {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names) 
            {
                string ext = Path.GetExtension(filename);
                if (!ext.Equals(".lua")) continue;

                string fn = Path.GetFileName(filename);
                if(fn.Equals(".DS_Store")) continue;

                string file = filename.Replace('\\', '/');
                fileList.Add(file);
            }

            foreach (string dir in dirs) 
            {
                RecursiveLua(dir, fileList);
            }
        }


        static void RecursiveLuaBytes(string path, List<string> fileList) {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names) 
            {
                string ext = Path.GetExtension(filename);
                if (!ext.Equals(AssetManagerSetting.BytesExt)) continue;

                string fn = Path.GetFileName(filename);
                if(fn.Equals(".DS_Store")) continue;

                string file = filename.Replace('\\', '/');
                fileList.Add(file);
            }

            foreach (string dir in dirs) 
            {
                RecursiveLuaBytes(dir, fileList);
            }
        }


    }

}