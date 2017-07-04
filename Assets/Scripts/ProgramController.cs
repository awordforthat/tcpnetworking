using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Diagnostics;

namespace Citrus
{
	public class ProgramController : MonoBehaviour
	{
		#if UNITY_EDITOR
		public static readonly string FOLDER_PATH = Directory.GetCurrentDirectory() + "/TempFiles";
		#else
		public static readonly string FOLDER_PATH = Directory.GetCurrentDirectory();
		#endif
		public static readonly string SHUTDOWN_SCRIPT = FOLDER_PATH + "/shutdownscript.bat";

		private void Awake()
		{
			Screen.SetResolution(450, 450, false);

			// setup callbacks
			NetworkMessageCall shutdownCallback = new NetworkMessageCall(0, this.ShutdownCallback);
			NetworkMessageCall restartCallback = new NetworkMessageCall(1, this.RestartCallback);

			NetworkController.AddNetworkCallback(shutdownCallback);
			NetworkController.AddNetworkCallback(restartCallback);

			DirectoryInfo settingsDirectory = Directory.GetParent(SHUTDOWN_SCRIPT);
			if(!settingsDirectory.Exists)
			{
				// create the directory
				settingsDirectory.Create();
			}
			System.IO.File.WriteAllLines(SHUTDOWN_SCRIPT, new string[] {
				#if UNITY_EDITOR
				"pause",
				#endif
				"IF [%1]==[] (goto shutdown) ELSE (goto restart)",
				":shutdown",
				"shutdown /s /f /t 5 /c \"Controlled shutdown commencing\"",
				"EXIT 0",
				":restart",
				"shutdown /r /f /t 5 /c \"Controlled restart commencing\"",
				"EXIT 0"
			});
		}

		/**
		 * Make sure to remove the event listener for for your exitEventHandler (if you supply one) in the callback. You can do
		 * that in this way:
		 * 		Process p = sender as Process;
		 * 		p.Exited -= myExitHandlerFunction;
		 */
		public static Process RunBatchFile(string batchFileLocation, params object[] args)
		{
			string cmdText = "/C \"" + batchFileLocation;
			for(int i = 0; i < args.Length; i++)
			{
				cmdText += " " + args[i].ToString();
			}
			cmdText += "\"";

			Process p = new Process();
			ProcessStartInfo pStartInfo = new ProcessStartInfo("cmd.exe", cmdText);
			#if !UNITY_EDITOR
			pStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			#endif
			p.StartInfo = pStartInfo;
			p.Start();
			return p;
		}

		private bool ShutdownCallback(byte[] bytes)
		{
			UnityEngine.Debug.Log("Shutdown callback!");
			RunBatchFile(SHUTDOWN_SCRIPT);
			return true;
		}

		private bool RestartCallback(byte[] bytes)
		{
			UnityEngine.Debug.Log("Restart callback!");
			RunBatchFile(SHUTDOWN_SCRIPT, "restart");
			return true;
		}
	}
}