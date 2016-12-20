using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Net;
using System.Threading;
using UnityEditor.Utils;
using System.Net.Sockets;

namespace com.ihaiu
{
	internal class LaunchAssetBundleServer : ScriptableSingleton<LaunchAssetBundleServer>
	{
        const string kLocalAssetbundleServerMenu = "资源管理/Local AssetBundle Server";

        private static string _serverRootPath;
        public static string ServerRootPath
        {
            get
            {
                if (string.IsNullOrEmpty(_serverRootPath))
                {
                    if (EditorPrefs.HasKey("AssetBundleServerRootPath"))
                    {
                        _serverRootPath = EditorPrefs.GetString("AssetBundleServerRootPath");
                    }
                    else
                    {
                        _serverRootPath = AssetManagerSetting.EditorAssetBundleServerRoot_WWW;
                    }
                }
                return _serverRootPath;
            }

            set
            {
                _serverRootPath = value;
                EditorPrefs.SetString("AssetBundleServerRootPath", value);
            }
        }

		[SerializeField]
		int 	m_ServerPID = 0;


        [SerializeField]
        string     m_host;

        public static string Host
        {
            get
            {
                return instance.m_host;
            }

            set
            {
                instance.m_host = value;
            }
        }

		[MenuItem (kLocalAssetbundleServerMenu)]
		public static void ToggleLocalAssetBundleServer ()
		{
			bool isRunning = IsRunning();
			if (!isRunning)
			{
				Run ();
			}
			else
			{
				KillRunningAssetBundleServer ();
			}
		}

		[MenuItem (kLocalAssetbundleServerMenu, true)]
		public static bool ToggleLocalAssetBundleServerValidate ()
		{
			bool isRunnning = IsRunning ();
			Menu.SetChecked(kLocalAssetbundleServerMenu, isRunnning);
			return true;
		}

		public static bool IsRunning ()
		{
			if (instance.m_ServerPID == 0)
				return false;

			var process = Process.GetProcessById (instance.m_ServerPID);
			if (process == null)
				return false;

			return !process.HasExited;
		}

		public static void KillRunningAssetBundleServer ()
		{
			// Kill the last time we ran
			try
			{
				if (instance.m_ServerPID == 0)
					return;

				var lastProcess = Process.GetProcessById (instance.m_ServerPID);
				lastProcess.Kill();
				instance.m_ServerPID = 0;
			}
			catch
			{
			}
		}
		
		public static void Run ()
		{
            PathUtil.CheckPath(ServerRootPath, false);
            
            string pathToAssetServer = Path.Combine(Application.dataPath, AssetManagerSetting.EditorAssetBundleServerExe);
	
			KillRunningAssetBundleServer();
			
			WriteServerURL();
			
            string args = ServerRootPath;
            UnityEngine.Debug.Log(pathToAssetServer);
            args = string.Format("\"{0}\" {1}", args, Process.GetCurrentProcess().Id);
            UnityEngine.Debug.Log("args=" + args);
            UnityEngine.Debug.Log("GetMonoInstallation=" + MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"));
			ProcessStartInfo startInfo = ExecuteInternalMono.GetProfileStartInfoForMono(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", pathToAssetServer, args, true);
            startInfo.WorkingDirectory = ServerRootPath;
			startInfo.UseShellExecute = false;
            Process launchProcess = Process.Start(startInfo);
            UnityEngine.Debug.Log(startInfo.WorkingDirectory);
			if (launchProcess == null || launchProcess.HasExited == true || launchProcess.Id == 0)
			{
				//Unable to start process
				UnityEngine.Debug.LogError ("Unable Start AssetBundleServer process");
			}
			else
			{
				//We seem to have launched, let's save the PID
				instance.m_ServerPID = launchProcess.Id;
			}
		}


        public static void WriteServerURL()
        {
            string downloadURL;
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            downloadURL = "http://"+localIP+":7888/";
            Host = downloadURL;

            Games.GameConstConfig gameConstConfig = Games.GameConstConfig.Load();
            gameConstConfig.WebUrl_Develop = downloadURL;
            gameConstConfig.Save();

            VersionInfo versionInfo = VersionInfo.Load(ServerRootPath + "/" + AssetManagerSetting.VersionInfoName);
            versionInfo.version = gameConstConfig.Version;
            if (ServerRootPath == AssetManagerSetting.EditorAssetBundleServerRoot_StreamingAssets)
            {
                versionInfo.updateLoadUrl = downloadURL;
            }
            else
            {
                versionInfo.updateLoadUrl = downloadURL + "StreamingAssets/";
            }



            string versionPath = AssetManagerSetting.GetServerVersionInfoURL(ServerRootPath, gameConstConfig.CenterName) ;
            versionInfo.Save(versionPath);
        }

	}
}