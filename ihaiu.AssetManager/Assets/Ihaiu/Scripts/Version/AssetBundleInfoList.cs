using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace com.ihaiu
{
    public class AssetBundleInfoList 
    {
        public List<AssetBundleInfo> list = new List<AssetBundleInfo>();

        private Dictionary<string, AssetBundleInfo> _dict;
        private Dictionary<string, AssetBundleInfo> dict
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<string, AssetBundleInfo>();
                    int count = list.Count;
                    AssetBundleInfo asset;
                    for(int i = 0; i < count; i ++)
                    {
                        asset = list[i];
                        _dict.Add(asset.assetBundleName, asset);
                    }
                }
                return _dict;
            }
        }


        public bool Has(string assetBundleName)
        {
            return dict.ContainsKey(assetBundleName);
        }


        public AssetBundleInfo Get(string assetBundleName)
        {
            if(dict.ContainsKey(assetBundleName))
                return dict[assetBundleName];

            return null;
        }

        public void Add(AssetBundleInfo item)
        {
            if (dict.ContainsKey(item.assetBundleName))
            {
                AssetBundleInfo asset = dict[item.assetBundleName];
                asset.assetBundleName = item.assetBundleName;
                asset.assetName = item.assetName;
                asset.objType = item.objType;
            }
            else
            {
                dict.Add(item.assetBundleName, item);
                list.Add(item);
            }
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



        public static AssetBundleInfoList Read(string path)
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

        public static AssetBundleInfoList Deserialize(string txt)
        {
            AssetBundleInfoList list = new AssetBundleInfoList();

            using(StringReader stringReader = new StringReader(txt))
            {
                while(stringReader.Peek() >= 0)
                {
                    string line = stringReader.ReadLine();
                    if(!string.IsNullOrEmpty(line))
                    {
                        string[] seg = line.Split(';');
                        AssetBundleInfo item = new AssetBundleInfo();
                        item.path               = seg[0];
                        item.assetBundleName    = seg[1];
                        item.assetName          = seg[2];
                        item.objType            = seg[3];
                        list.Add(item);
                    }
                }
            }

            return list;
        }
    }
}
