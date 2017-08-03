using UnityEngine;
using System.Collections;
using System.IO;


namespace com.ihaiu
{
    public class VersionLocalInfo  
    {
        public string localVer  = "0.0.0";
        public Version localVersion     = new Version(0, 0, 0);
        public Version streamVersion    = new Version(0, 0, 0);

        public bool IsNewApp
        {
            get
            {
                return !localVersion.Equals(streamVersion);
            }
        }

        public bool IsResetAppRes
        {
            get
            {
                Version sub = localVersion.Sub(streamVersion);
                if (sub.master != 0)
                {
                    return true;
                }
                else if(sub.master == 0 && sub.minor < 0)
                {
                    return true;
                }
                else if(sub.master == 0 && sub.minor == 0 && sub.revised < 0)
                {
                    return true;
                }
                return false;
            }
        }




        /** 当前Zip版本 */
        public string currentZipVer = "0.0.0";
        /** 进行中的Zip版本 */
        public string handZipVer    = "0.0.0";
        /** 进行中的Zip MD5 */
        public string handZipMD5    = "";

        public Version currentZipVersion    = new Version(0, 0, 0);
        public Version handZipVersion       = new Version(0, 0, 0);
        public Version serverZipVersion     = new Version(0, 0, 0);
        public VersionInfo serverInfo;


        public VersionLocalInfo SetServerInfo(VersionInfo serverInfo)
        {
            this.serverInfo = serverInfo;
            if (serverInfo != null)
            {
                serverZipVersion.Parse(serverInfo.zipVersion);
            }
            return this;
        }


        /** 是否需要更新Zip */
        public bool IsNeedUpdateZip
        {
            get
            {
                //TODO
//                return false;

                if (serverZipVersion.IsZero)
                    return false;

                Version sub = currentZipVersion.Sub(serverZipVersion);
                if (sub.master < 0)
                {
                    return true;
                }
                else if(sub.master == 0 && sub.minor < 0)
                {
                    return true;
                }
                else if(serverInfo != null && serverInfo.zipCheckDigit > 2 && sub.master == 0 && sub.minor == 0 && sub.revised < 0)
                {
                    return true;
                }
                return false;
            }
        }

        /** 是否需要删除正在处理Zip文件和临时文件 */
        public bool IsNeedDeleteHandZip
        {
            get
            {
                if (serverInfo == null)
                    return false;

                if (IsNeedUpdateZip)
                {
                    if (!string.IsNullOrEmpty(serverInfo.zipMD5) && handZipMD5 != serverInfo.zipMD5)
                    {
                        return true;
                    }
                    else if (!handZipVersion.Equals(serverZipVersion))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void SetBeginHandZip()
        {
            if (serverInfo == null)
                return;
            
            handZipMD5 = serverInfo.zipMD5;
            handZipVersion.Parse(serverInfo.zipVersion);
            Save();
        }

        public void SetEndHandZip()
        {
            currentZipVersion.Parse(serverInfo.zipVersion);
            Save();
        }

        public bool CheckZipExe()
        {
//            return Coo.downloadZip.CheckExe();
			return false;
        }


        public void Save()
        {
            string path = AssetManagerSetting.PersistentFilePath.VersionLocalInfo;
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

			localVer = localVersion.ToConfig();
            currentZipVer = currentZipVersion.ToConfig();
            handZipVer = handZipVersion.ToConfig();

            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(path, json);
        }


        public static VersionLocalInfo Load()
        {
            string path = AssetManagerSetting.PersistentFilePath.VersionLocalInfo;
            if (!File.Exists(path))
            {
                return new VersionLocalInfo();
            }

            string json = File.ReadAllText(path);
            VersionLocalInfo info = JsonUtility.FromJson<VersionLocalInfo>(json);
            info.localVersion.Parse(info.localVer);
            info.currentZipVersion.Parse(info.currentZipVer);
            info.handZipVersion.Parse(info.handZipVer);
            return info;
        }

        private static VersionLocalInfo _install;
        public static VersionLocalInfo Install
        {
            get
            {
                if (_install == null)
                {
                    _install = Load();
                }
                return _install;
            }
        }
    }
}
