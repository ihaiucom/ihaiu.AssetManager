using UnityEngine;
using System.Collections;
using System.IO;

namespace Ihaiu.Debugs
{
    public class AppInfo 
    {
    	public static string GetAppInfo()
    	{
    		string info = "";
    		
    		info += "\nApplication.unityVersion=" + Application.unityVersion ;
    		info += "\nApplication.version=" + Application.version ;
    		info += "\nApplication.productName=" + Application.productName ;
    		info += "\nApplication.isEditor=" + Application.isEditor ;
    		info += "\nApplication.isConsolePlatform=" + Application.isConsolePlatform ;
    		info += "\nApplication.isMobilePlatform=" + Application.isMobilePlatform ;
    		info += "\nApplication.platform=" + Application.platform ;

    		info += "\nApplication.persistentDataPath=" + Application.persistentDataPath ;
    		info += "\nApplication.dataPath=" + Application.dataPath ;
    		info += "\nApplication.streamingAssetsPath=" + Application.streamingAssetsPath ;
    		info += "\nApplication.temporaryCachePath=" + Application.temporaryCachePath ;

    		info += "\n(WebPlay)Application.absoluteURL=" + Application.absoluteURL ;
    		info += "\n(WebPlay)Application.srcValue=" + Application.srcValue ;


    		
    		info += "\nApplication.internetReachability=" + Application.internetReachability ;

    		
            info += "\nApplication.levelCount=" + Application.levelCount ;
    		info += "\nApplication.loadedLevel=" + Application.loadedLevel ;

    		info += "\nApplication.loadedLevelName=" + Application.loadedLevelName ;
    		info += "\nApplication.isLoadingLevel=" + Application.isLoadingLevel ;
    		info += "\nApplication.isPlaying=" + Application.isPlaying ;
    		info += "\nApplication.isWebPlayer=" + Application.isWebPlayer ;
    		info += "\nApplication.runInBackground=" + Application.runInBackground ;



    		info += "\nApplication.backgroundLoadingPriority=" + Application.backgroundLoadingPriority ;
    		info += "\nApplication.bundleIdentifier=" + Application.bundleIdentifier ;
    		info += "\nApplication.cloudProjectId=" + Application.cloudProjectId ;
    		info += "\nApplication.companyName=" + Application.companyName ;
    		info += "\nApplication.genuine=" + Application.genuine ;
    		info += "\nApplication.genuineCheckAvailable=" + Application.genuineCheckAvailable ;
    		info += "\nApplication.installMode=" + Application.installMode ;
    		info += "\nApplication.genuineCheckAvailable=" + Application.genuineCheckAvailable ;
    		info += "\nApplication.sandboxType=" + Application.sandboxType ;
    		info += "\nApplication.streamedBytes=" + Application.streamedBytes ;
    		info += "\nApplication.systemLanguage=" + Application.systemLanguage ;
    		info += "\nApplication.targetFrameRate=" + Application.targetFrameRate ;
    		info += "\nApplication.webSecurityEnabled=" + Application.webSecurityEnabled ;

    		
    		info += "\n-------" ;

    		
    		info += "\nSystemInfo.DeviceModel : " + SystemInfo.deviceModel;
    		info += "\nSystemInfo.deviceName : " + SystemInfo.deviceName;
    		info += "\nSystemInfo.deviceType : " + SystemInfo.deviceType;
    		info += "\nSystemInfo.deviceUniqueIdentifier : " + SystemInfo.deviceUniqueIdentifier;
    		info += "\nSystemInfo.graphicsDeviceName : " + SystemInfo.graphicsDeviceName;
    		info += "\nSystemInfo.graphicsMemorySize : " + SystemInfo.graphicsMemorySize+ "MB";
    		info += "\nSystemInfo.graphicsPixelFillrate : " + SystemInfo.graphicsPixelFillrate;
    		info += "\nSystemInfo.graphicsShaderLevel : " + SystemInfo.graphicsShaderLevel;
    		info += "\nSystemInfo.maxTextureSize : " + SystemInfo.maxTextureSize;
    		info += "\nSystemInfo.npotSupport : " + SystemInfo.npotSupport;
    		info += "\nSystemInfo.operatingSystem : " + SystemInfo.operatingSystem;
    		info += "\nSystemInfo.processorCount : " + SystemInfo.processorCount;
    		info += "\nSystemInfo.processorType : " + SystemInfo.processorType;
    		info += "\nSystemInfo.systemMemorySize : " + SystemInfo.systemMemorySize+ "MB";
    		info += "\nScreen : " + Screen.currentResolution.width +" x "+ Screen.currentResolution.height;
    		info += "\nScreen.currentResolution.refreshRate : " + Screen.currentResolution.refreshRate;
    		info += "\nScreen.sleepTimeout : " + Screen.sleepTimeout;

            info += "\nDeviceResolution.width=" + Screen.width;
            info += "\nDeviceResolution.height=" + Screen.height;
    		info += "\n[Screen.resolutions] ";
    		for(int i = 0; i < Screen.resolutions.Length; i ++)
    		{
    			info += "\n    i=" + i + ",  width=" +  Screen.resolutions[i].width + ",  height=" + Screen.resolutions[i].height + ",  refreshRate=" + Screen.resolutions[i].refreshRate ;
    		}


            info += "\n";
            info += Games.GameConst.GetInfo();
            info += "\n";
            info += Ihaiu.Assets.AssetManagerSetting.GetInfo();
    	
    		info += "\n";
    		#if LUA_ZIP
    		info += "\n LUA_ZIP define";
    		#else
    		info += "\n LUA_ZIP undefine";
    		#endif



    		return info;
    	}
    }
}