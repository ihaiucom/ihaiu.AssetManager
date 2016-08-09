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

namespace Ihaiu.Assets
{
	internal class LaunchAssetBundleServer : ScriptableSingleton<LaunchAssetBundleServer>
	{
        const string kLocalAssetbundleServerMenu = "AssetManager/Local AssetBundle Server";

		[SerializeField]
		int 	m_ServerPID = 0;

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

		static bool IsRunning ()
		{
			if (instance.m_ServerPID == 0)
				return false;

			var process = Process.GetProcessById (instance.m_ServerPID);
			if (process == null)
				return false;

			return !process.HasExited;
		}

		static void KillRunningAssetBundleServer ()
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
		
		static void Run ()
		{
            string pathToAssetServer = Path.Combine(Application.dataPath, AssetManagerSetting.EditorAssetBundleServerExe);
	
			KillRunningAssetBundleServer();
			
			WriteServerURL();
			
            string args = AssetManagerSetting.EditorAssetBundleServerRoot;
            UnityEngine.Debug.Log(pathToAssetServer);
            args = string.Format("\"{0}\" {1}", args, Process.GetCurrentProcess().Id);
            UnityEngine.Debug.Log(args);
			ProcessStartInfo startInfo = ExecuteInternalMono.GetProfileStartInfoForMono(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", pathToAssetServer, args, true);
            startInfo.WorkingDirectory = AssetManagerSetting.EditorAssetBundleServerRoot;
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

            Games.GameConstConfig gameConstConfig = Games.GameConstConfig.Load();
            gameConstConfig.WebUrl = downloadURL;
            gameConstConfig.Save();

            VersionInfo versionInfo = VersionInfo.Load();
            versionInfo.updateLoadUrl = downloadURL + "StreamingAssets/";
            versionInfo.Save();
        }

	}
}