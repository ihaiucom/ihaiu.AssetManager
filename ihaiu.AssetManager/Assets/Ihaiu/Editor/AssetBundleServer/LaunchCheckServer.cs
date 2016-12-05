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
    internal class LaunchCheckServer : ScriptableSingleton<LaunchCheckServer>
	{
        const string menuTxt = "资源管理/Local Check Server";

       
		[SerializeField]
		int 	m_ServerPID = 0;



		[MenuItem (menuTxt)]
		public static void ToggleLocalServer ()
		{
			bool isRunning = IsRunning();
			if (!isRunning)
			{
				Run ();
			}
			else
			{
				KillRunningServer ();
			}
		}

		[MenuItem (menuTxt, true)]
		public static bool ToggleLocalServerValidate ()
		{
			bool isRunnning = IsRunning ();
			Menu.SetChecked(menuTxt, isRunnning);
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

		public static void KillRunningServer ()
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

            var editorAppPath = EditorApplication.applicationPath;
            editorAppPath = editorAppPath.Replace("Unity.app", "MonoDevelop.app");
            editorAppPath = Path.Combine(editorAppPath, "Contents");
            string monoPath = Path.Combine(editorAppPath, "Frameworks/Mono.framework/Versions/Current/bin/mono");
            
            string exePath = Path.Combine(Application.dataPath, AssetManagerSetting.EditorCheckServerExe);
	
			KillRunningServer();

            UnityEngine.Debug.Log(monoPath);
            UnityEngine.Debug.Log(exePath);
			
            string args = "";
            args = string.Format("{0}", args, Process.GetCurrentProcess().Id);
            UnityEngine.Debug.Log("args=" + args);
            ProcessStartInfo startInfo = new ProcessStartInfo(monoPath, exePath);
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



	}
}