
namespace com.ihaiu
{

    public class VersionData 
    {
        #region app version
        /** StreamingAssets 的App版本号 */
        private Version streamingAppVersion    = new Version(0, 0, 0);
        /** PersistentAssets 的App版本号 */
        private Version persistentAppVersion   = new Version(0, 0, 0);
        /** 服务器 的App版本号 */
        private Version serverAppVersion       = new Version(0, 0, 0);

        /** 是否需要初始化数据 */
        public bool AppNeedInit
        {
            get
            {
                if (persistentAppVersion.IsZero)
                    return true;


                Version sub = persistentAppVersion.Sub(streamingAppVersion);
                if (sub.master < 0)
                {
                    return true;
                }
                else if (sub.master == 0 && sub.minor < 0)
                {
                    return true;
                }
                else if (sub.master == 0 && sub.minor == 0 && sub.revised < 0)
                {
                    return true;
                }

                return false;
            }
        }

        /** 对比服务器版本号状态 */
        public bool AppNeedDown
        {
            get
            {
                if (serverAppVersion.IsZero)
                    return false;

                Version sub = persistentAppVersion.Sub(serverAppVersion);
                if (sub.master < 0)
                {
                    return true;
                }
                else if (sub.master == 0 && sub.minor < 0)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion



        #region res version
        /** StreamingAssets 的App版本号 */
        private Version streamingResVersion    = new Version(0, 0, 0);
        /** PersistentAssets 的Zip版本号 */
        private Version persistentResVersion   = new Version(0, 0, 0);
        /** 服务器 的Zip版本号 */
        private Version serverResVersion       = new Version(0, 0, 0);


        /** 是否需要初始化数据 */
        public bool ResNeedSetLoadFormApp
        {
            get
            {
                if (persistentResVersion.IsZero)
                {
                    return true;
                }

                Version sub = persistentResVersion.Sub(streamingResVersion);
                if (sub.master < 0)
                {
                    return true;
                }
                else if (sub.master == 0 && sub.minor < 0)
                {
                    return true;
                }
                else if (sub.master == 0 && sub.minor == 0 && sub.revised < 0)
                {
                    return true;
                }

                return false;
            }
        }

        /** 是否需要下载ZIP资源包 */
        public bool ResNeedDownZip
        {
            get
            {
                if(serverResVersion.IsZero) return false;
                Version sub = persistentResVersion.Sub(serverResVersion);

                if (sub.master < 0)
                {
                    return true;
                }
                else if(sub.master == 0 && sub.minor < 0)
                {
                    return true;
                }

                return false;
            }
        }

        /** 是否需要更新资源 */
        public bool ResNeedUpdate
        {
            get
            {
                if(serverResVersion.IsZero) return false;

                Version sub = persistentResVersion.Sub(serverResVersion);
                if (sub.master == 0 && sub.minor == 0 && sub.revised < 0)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion


    }
}