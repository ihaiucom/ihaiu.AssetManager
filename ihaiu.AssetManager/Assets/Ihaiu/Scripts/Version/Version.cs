using System;

namespace com.ihaiu
{
    /** 版本号 (git version) */
    public class Version
    {

        /** 第一部分为主版本号 */
        [HelpAttribute("主版本号")]
        public int master = 1;


        /** 第二部分为次版本号 */
        [HelpAttribute("次版本号")]
        public int minor = 0;


        /** 第三部分为修订版 */
        [HelpAttribute("修订版")]
        public int revised = 0;


        [HelpAttribute("软件版本阶段说明")]
        public VersionStages stages = VersionStages.Beta;


        [HelpAttribute("版本类型")]
        public VersionType verType = 0;


        [HelpAttribute("时间")]
        public Int64 datetime;

        public Version()
        {
//            SetNowDatetime();
        }

        public Version(int master, int minor, int revised)
        {
            this.master = master;
            this.minor = minor;
            this.revised = revised;
        }

        public bool IsZero
        {
            get
            {
                return master == 0 && minor == 0 && revised == 0;
            }
        }


        public void SetNowDatetime()
        {
            datetime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }


        public bool Equals(Version b)
        {
            return  master == b.master &&
                minor == b.minor &&
                revised == b.revised &&
                stages == b.stages &&
                verType == b.verType
                ;
        }

        public int Comparer(Version b)
        {
            if (master != b.master)
            {
                return master - b.master;
            }

            if (minor != b.minor)
            {
                return minor - b.minor;
            }

            if (revised != b.revised)
            {
                return revised - b.revised;
            }

            if (stages != b.stages)
            {
                return (int)stages - (int)b.stages;
            }

            return datetime > b.datetime ? 1 : -1;
        }

        public Version Sub(Version target)
        {
            Version sub = new Version();
            sub.master  = master - target.master;
            sub.minor   = minor - target.minor;
            sub.revised = revised - target.revised;
            return sub;
        }

        public void Copy(Version b)
        {
            master = b.master;
            minor = b.minor;
            revised = b.revised;
            stages = b.stages;
            verType = b.verType;
            datetime = b.datetime;
        }


        public string ToDateString()
        {
            string str = datetime.ToString();
            string dstr = string.Format("{0}年{1}月{2}日{3}:{4}", 
                str.Substring(0, 4), 
                str.Substring(4, 2), 
                str.Substring(6, 2), 

                str.Substring(8, 2),
                str.Substring(10, 2)
            );

            return string.Format("{4}-ver{0:D2}.{1:D2}.{2:D2}_{3}-{5}", master, minor, revised, GetStagesTxt(stages), GetVerTypeTxt(verType), dstr);
        }

        public override string ToString()
        {
            return string.Format("{4}-ver{0:D2}.{1:D2}.{2:D2}_{3}-{5}", master, minor, revised, GetStagesTxt(stages), GetVerTypeTxt(verType), datetime);
        }

        public string ToStringNoDate()
        {
            return string.Format("{4}-ver{0:D2}.{1:D2}.{2:D2}_{3}", master, minor, revised, GetStagesTxt(stages), GetVerTypeTxt(verType));
        }


        public string ToConfig()
        {
            return string.Format("{0:D1}.{1:D1}.{2:D1}", master, minor, revised);
        }

        public static Version ParseVersion(string str)
        {
            return new Version().Parse(str);
        }

        public Version Parse(string str)
        {
            str = str.ToLower().Replace("version:", "").Replace("version", "").Replace("ver", "").Replace("v", "");
            string[] arr = str.Split('-');
            if (arr.Length > 1)
            {
                string typeText = arr[0];
                verType = GetVerType(typeText);
                str = arr[1];

                if (arr.Length > 2)
                {
                    datetime = Convert.ToInt64(arr[2]);
                }
            }
            else
            {
                str = arr[0];
            }

            arr = str.Split('_');
            if (arr.Length > 1)
            {
                string stageTxt = arr[1];
                stages = GetStages(stageTxt);
            }

            arr = arr[0].Split('.');
            master  = Convert.ToInt32(arr[0]);
            minor   = Convert.ToInt32(arr[1]);
            revised = Convert.ToInt32(arr[2]);
            return this;
        }




        public static string GetStagesTxt(VersionStages stages)
        {
            switch(stages)
            {
                case VersionStages.Base:
                    return "base";

                case VersionStages.Alpha:
                    return "alpha";

                case VersionStages.Beta:
                    return "beta";

                case VersionStages.RC:
                    return "rc";

                case VersionStages.Release:
                    return "release";

                default:
                    return "beta";
            }
        }

        public static VersionStages GetStages(string stagesTxt)
        {
            stagesTxt = stagesTxt.ToLower();

            switch(stagesTxt)
            {
                case "base":
                    return VersionStages.Base;

                case "alpha":
                    return VersionStages.Alpha;

                case "beta":
                    return VersionStages.Beta;

                case "rc":
                    return VersionStages.RC;

                case "release":
                    return VersionStages.Release;

                default:
                    return VersionStages.Beta;
            }
        }



        public static string GetVerTypeTxt(VersionType type)
        {
            switch(type)
            {
                case VersionType.Patch:
                    return "patch";
                case VersionType.App:
                default:
                    return "app";
            }
        }

        public static VersionType GetVerType(string txt)
        {
            txt = txt.ToLower();

            switch(txt)
            {
                case "patch":
                    return VersionType.Patch;

                case "app":
                default:
                    return VersionType.App;

            }
        }


        public static int SortComparer(Version a, Version b)
        {
            return a.Comparer(b);
        }


    }
}