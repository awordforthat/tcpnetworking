using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.Callbacks;
using System.IO;

namespace Citrus
{
	public class BuildController
	{
		// DO NOT CHANGE
		private static readonly string PATH_TO_BUILD_TO = "C:/Jenkins_Unity_Builds/" + PlayerSettings.productName + "/" + PlayerSettings.productName + ".app";

		[MenuItem("Builds/Jenkins Build")]
		static void PerformServerBuild()
		{
			PerformBuild(PATH_TO_BUILD_TO, PlayerSettings.productName + ".exe", FindEnabledEditorScenes());
		}

		[MenuItem("Builds/Build Locally")]
		static void PerformLocalBuild()
		{
			PerformBuild(Environment.CurrentDirectory + "/Builds/" + PlayerSettings.productName + "_" + DateTime.Now.ToString("yyyy-MM-ddTHH_mm_ss"), PlayerSettings.productName + ".exe", FindEnabledEditorScenes());
		}

		private static void PerformBuild(string path, string executableName, string[] scenes)
		{
			PlayerSettings.showUnitySplashScreen = false;
			if(Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}

			GenericBuild(scenes, path + "/" + executableName, BuildTarget.StandaloneWindows, BuildOptions.None);
		}

		private static string[] FindEnabledEditorScenes()
		{
			List<string> EditorScenes = new List<string>();
			foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if(scene.enabled)
				{
					EditorScenes.Add(scene.path);
				}
			}
			return EditorScenes.ToArray();
		}

		static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
			string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
			if(res.Length > 0)
			{
				throw new Exception("BuildPlayer failure: " + res);
			}
		}

		[PostProcessBuildAttribute(0)]
		static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
		{
			// copy the XML file from the assets/data to the project folder
			string toPath = pathToBuildProject.Remove(pathToBuildProject.LastIndexOf("/")) + "/data";
			// check to see if the folder exists at the toPath, and if it does, delete it
			if(Directory.Exists(toPath))
			{
				Directory.Delete(toPath, true);
			}
			if(Directory.Exists(Environment.CurrentDirectory + "/Assets/data"))
			{
				FileUtil.CopyFileOrDirectory(Environment.CurrentDirectory + "/Assets/data", toPath);
				// delete the .meta files in the new folder since we don't need them
				DirectoryInfo di = new DirectoryInfo(toPath);
				FileInfo[] fileList = di.GetFiles("*.meta", SearchOption.AllDirectories);
				for(int i = 0; i < fileList.Length; i++)
				{
					fileList[i].Delete();
				}
			}
		}
	}
}