using UnityEngine;
using System.Collections;
using System;

namespace com.ihaiu
{
    public class AssetFile 
    {
        public string path;
        public string md5;
        public int verMaster    = 0;
        public int verMinor     = 0;
        public int verRevised   = 0;


        public AssetFile()
        {
        }

        public AssetFile(string path, string md5)
        {
            this.path = path;
            this.md5 = md5;
        }

        public AssetFile SetVer(Version ver)
        {
            verMaster   = ver.master;
            verMinor    = ver.minor;
            verRevised  = ver.revised;
            return this;
        }

        public AssetFile SetVer(string ver)
        {
            if (!string.IsNullOrEmpty(ver))
            {
                string[] arr = ver.Split('.');
                if (arr.Length >= 3)
                {
                    verMaster = Convert.ToInt32(arr[0]);
                    verMinor = Convert.ToInt32(arr[1]);
                    verRevised = Convert.ToInt32(arr[2]);
                }
            }
            return this;
        }

        public bool IsEnableCover(Version serVer)
        {
            if (verMaster < serVer.master)
            {
                return true;
            }
            else if(verMaster == serVer.master && verMinor < serVer.minor)
            {
                return true;
            }
            else if(verMaster == serVer.master && verMinor == serVer.minor && verRevised < serVer.revised)
            {
                return true;
            }
            return false;
        }

        public bool IsEnableCover(AssetFile target)
        {
            if (verMaster < target.verMaster)
            {
                return true;
            }
            else if(verMaster == target.verMaster && verMinor < target.verMinor)
            {
                return true;
            }
            else if(verMaster == target.verMaster && verMinor == target.verMinor && verRevised < target.verRevised)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2}.{3}.{4}", path, md5, verMaster, verMinor, verRevised);
        }
    }
}