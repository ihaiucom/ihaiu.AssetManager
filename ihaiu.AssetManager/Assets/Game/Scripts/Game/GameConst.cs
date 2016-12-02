using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Games
{
    public class GameConst 
	{
        public static string UserId
        {
            get
            {
                return "00001";
            }
        }


		public static bool      DevelopMode = true;					    //开发模式


		public static string    AppName = "ihaiu";           			//应用程序名称
		public static string    AppPrefix = AppName + "_";              //应用程序前缀

	

        public static string    Version = "0.0.0";                      //游戏版本号
        public static string    CenterName = "Official";                //运营商


        public static string   WebUrl_Release  = "http://www.ihaiu.com/";          //更新地址--发布版本
        public static string   WebUrl_Develop  = "http://localhost/";              //更新地址--调试版本

        public static bool WebUrlIsDevelop
        {
            get
            {
                if(PlayerPrefsUtil.HasKey(PlayerPrefsKey.Test_WebUrl_IsDevelop, false))
                {
                    return PlayerPrefsUtil.GetBool(PlayerPrefsKey.Test_WebUrl_IsDevelop, false);
                }
                return false;
            }

            set
            {
                PlayerPrefsUtil.SetBool(PlayerPrefsKey.Test_WebUrl_IsDevelop, value, false);
            }
        }

        public static string    WebUrl
        {
            get
            {
                if (WebUrlIsDevelop)
                {
                    return WebUrl_Develop;
                }
                return WebUrl_Release;
            }
        }

        public static string GetInfo()
        {
            string info = "";
            info += "\nGameConst.UserId : " + UserId;
            info += "\nGameConst.Version : " + Version;
            info += "\nGameConst.DevelopMode : " + DevelopMode;
            info += "\nGameConst.AppName : " + AppName;
            info += "\nGameConst.AppPrefix : " + AppPrefix;
            info += "\nGameConst.WebUrl_Release : " + WebUrl_Release;
            info += "\nGameConst.WebUrl_Develop : " + WebUrl_Develop;
            info += "\nGameConst.WebUrlIsDevelop : " + WebUrlIsDevelop;
            info += "\nGameConst.WebUrl : " + WebUrl;


            return info;
        }
	

    }
}