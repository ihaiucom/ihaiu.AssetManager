using UnityEngine;
using System.Collections;

namespace com.ihaiu
{
    public class VersionCheck
    {

        public Version curr;
        public Version target;

        public Version sub = new Version();

        public VersionCheck(Version curr, Version target)
        {
            this.curr = curr;
            this.target = target;

            Sub();
        }

        public VersionCheckState Sub()
        {
            sub.master = curr.master - target.master;
            sub.minor = curr.minor - target.minor;
            sub.revised = curr.revised - target.revised;

            return State;
        }

        public VersionCheckState State
        {
            get
            {
                if (sub.master < 0)
                {
                    return VersionCheckState.DownApp;
                }
                else if(sub.master == 0 && sub.minor < 0)
                {
                    return VersionCheckState.DownApp;
                }

                if (sub.master == 0 && sub.minor == 0 && sub.revised < 0)
                {
                    return VersionCheckState.HotUpdate;
                }

                return VersionCheckState.Normal;
            }
        }

        public bool NeedCopy
        {
            get
            {
                return State == VersionCheckState.DownApp || State == VersionCheckState.HotUpdate;
            }
        }

        //----
        public static VersionCheckState CheckState(Version curr, Version newver)
        {
            return new VersionCheck(curr, newver).State;
        }

        public static VersionCheckState CheckState(string curr, string newver)
        {
            return CheckState(new Version().Parse(curr), new Version().Parse(newver));
        }

        //----
        public static bool CheckNeedCopy(Version curr, Version newver)
        {
            return new VersionCheck(curr, newver).NeedCopy;
        }


        public static bool CheckNeedCopy(string curr, string newver)
        {
            return CheckNeedCopy(new Version().Parse(curr), new Version().Parse(newver));
        }
    }
}