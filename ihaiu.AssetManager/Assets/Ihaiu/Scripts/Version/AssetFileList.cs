using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ihaiu.Assets
{
    public class AssetFileList
    {
        public List<AssetFile> list = new List<AssetFile>();

        private Dictionary<string, AssetFile> _dict;
        private Dictionary<string, AssetFile> dict
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<string, AssetFile>();
                    int count = list.Count;
                    AssetFile asset;
                    for(int i = 0; i < count; i ++)
                    {
                        asset = list[i];
                        _dict.Add(asset.path, asset);
                    }
                }
                return _dict;
            }
        }

        public bool Has(string path)
        {
            return dict.ContainsKey(path);
        }


        public AssetFile Get(string path)
        {
            if(dict.ContainsKey(path))
                return dict[path];

            return null;
        }

        public void Add(AssetFile item)
        {
            if (dict.ContainsKey(item.path))
            {
                dict[item.path].md5 = item.md5;
            }
            else
            {
                dict.Add(item.path, item);
                list.Add(item);
            }
        }


        public void Add(string path, string md5)
        {
            if (dict.ContainsKey(path))
            {
                dict[path].md5 = md5;
            }
            else
            {
                AssetFile asset = new AssetFile();
                asset.path = path;
                asset.md5 = md5;
                dict.Add(path, asset);
                list.Add(asset);
            }
        }


        public void Save(string path)
        {
            PathUtil.CheckPath(path, true);

            string txt = Serialize();

            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(txt);
            sw.Close(); fs.Close();
        }



        public static AssetFileList Read(string path)
        {
            using (FileStream fsRead = new FileStream(path, FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                fsRead.Read(heByte, 0, heByte.Length);
                string txt = System.Text.Encoding.UTF8.GetString(heByte);

                return Deserialize(txt);
            } 
        }



        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            int count = list.Count;

            for(int i = 0; i < count; i ++)
            {
                sb.AppendLine(list[i].ToString());
            }

            return sb.ToString();
        }

        public static AssetFileList Deserialize(string txt)
        {
            AssetFileList list = new AssetFileList();

            using(StringReader stringReader = new StringReader(txt))
            {
                while(stringReader.Peek() >= 0)
                {
                    string line = stringReader.ReadLine();
                    if(!string.IsNullOrEmpty(line))
                    {
                        string[] seg = line.Split(';');
                        list.Add(seg[0], seg[1]);
                    }
                }
            }
            
            return list;
        }

    }
}