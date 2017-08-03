using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.ihaiu
{
    public class AssetFileList
    {
        public string savePath ;
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

        public AssetFile Add(AssetFile item)
        {
            if (dict.ContainsKey(item.path))
            {
                dict[item.path].md5 = item.md5;

                dict[item.path].verMaster = item.verMaster;
                dict[item.path].verMinor = item.verMinor;
                dict[item.path].verRevised = item.verRevised;
                return dict[item.path];
            }
            else
            {
                dict.Add(item.path, item);
                list.Add(item);
                return item;
            }
        }


        public AssetFile Add(string path, string md5)
        {
            return Add(path, md5, null);
        }

        public AssetFile Add(string path, string md5, string ver)
        {
            if (dict.ContainsKey(path))
            {
                dict[path].md5 = md5;
                dict[path].SetVer(ver);

                return dict[path];
            }
            else
            {
                AssetFile asset = new AssetFile();
                asset.path = path;
                asset.md5 = md5;
                asset.SetVer(ver);
                dict.Add(path, asset);
                list.Add(asset);
                return asset;
            }
        }

        public void Remove(string path)
        {
            if (dict.ContainsKey(path))
            {
                AssetFile asset = dict[path];
                list.Remove(asset);
                dict.Remove(path);
            }
        }


        public AssetFileList SetSavePath(string path)
        {
            this.savePath = path;
            return this;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(savePath))
            {
                Debug.Log("AssetFileList savePath=" + savePath);
                return;
            }

            Save(savePath);
        }

        public void Save(string path)
        {
            PathUtil.CheckPath(path, true);

            if (File.Exists(path))
                File.Delete(path);
            
            string txt = Serialize();

            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(txt);
            sw.Close(); fs.Close();
        }



        public static AssetFileList Read(string path)
        {
            if (!File.Exists(path))
                return new AssetFileList().SetSavePath(path);
            
            using (FileStream fsRead = new FileStream(path, FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                fsRead.Read(heByte, 0, heByte.Length);
                string txt = System.Text.Encoding.UTF8.GetString(heByte);

                return  Deserialize(txt).SetSavePath(path);
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
                        list.Add(seg[0], seg[1], seg.Length > 2 ? seg[2] : null);
                    }
                }
            }
            
            return list;
        }


        public static List<AssetFile> Diff(AssetFileList current, AssetFileList update)
        {
            List<AssetFile> diffs = new List<AssetFile>();

            AssetFile gameConstItem = null;

            int count = update.list.Count;
            for(int i = 0; i < count; i ++)
            {
                AssetFile item = update.list[i];

                if (item.path == AssetManagerSetting.FileName.GameConst)
                {
                    gameConstItem = item;
                    continue;
                }

                if(!current.Has(item.path))
                {
                    diffs.Add(item);
                }
                else if(item.md5 != current.Get(item.path).md5)
                {
                    diffs.Add(item);
                }
            }

            if (gameConstItem != null)
            {
                diffs.Add(gameConstItem);
            }
            else
            {
                diffs.Add(new AssetFile(AssetManagerSetting.FileName.GameConst, "md5"));
            }

            return diffs;
        }


        public static AssetFileList DiffAssetFileList(AssetFileList current, AssetFileList update)
        {
            AssetFileList diffs = new AssetFileList();

            AssetFile gameConstItem = null;

            int count = update.list.Count;
            for(int i = 0; i < count; i ++)
            {
                AssetFile item = update.list[i];

                if (item.path == AssetManagerSetting.FileName.GameConst)
                {
                    gameConstItem = item;
                    continue;
                }

                if(!current.Has(item.path))
                {
                    diffs.Add(item);
                }
                else if(item.md5 != current.Get(item.path).md5)
                {
                    diffs.Add(item);
                }
            }

            if (gameConstItem != null)
            {
                diffs.Add(gameConstItem);
            }
            else
            {
                diffs.Add(new AssetFile(AssetManagerSetting.FileName.GameConst, "md5"));
            }

            return diffs;
        }
    }
}