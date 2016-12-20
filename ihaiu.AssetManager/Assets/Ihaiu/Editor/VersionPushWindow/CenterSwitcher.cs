using UnityEngine;
using System.Collections;
using com.ihaiu;

public class CenterSwitcher 
{
	public class CenterItem
	{
		public string 	name;
		public string 	cnName;
		public bool 	gitToggle = true;
		public string 	gitTag;
		public int 		gitTagMaxIndex;
		public string	gitTagUse;
        public VersionInfo versionInfo;

		public string gitTagLast
		{
			get
			{
				return gitTagMaxIndex > 0 ? gitTag + "-" + gitTagMaxIndex : gitTag;
			}
		}

		public CenterItem(string name, string cnName)
		{
			this.name 		= name;
			this.cnName 	= cnName;
		}
	}

	public static CenterItem[] centerItemList = new CenterItem[]{
		new CenterItem("Official", "官方"),
		new CenterItem("XiaoMi", "小米"),
		new CenterItem("WeiXin", "微信"),
		new CenterItem("UC", "UC"),
	};
}
