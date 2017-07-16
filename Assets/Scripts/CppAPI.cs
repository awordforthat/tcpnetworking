using UnityEngine;
using System.Runtime.InteropServices;

public class CppAPI : MonoBehaviour {
	// The imported function
	[DllImport("TestUnityPlugin", EntryPoint = "TestSort")]
	public static extern void TestSort(int [] a, int length);

	[DllImport("TestUnityPlugin", EntryPoint = "TestMultiply")]
	public static extern float TestMultiply(float a, float b);

	[DllImport("TestUnityPlugin", EntryPoint = "TestDivide")]
	public static extern float TestDivide(float a, float b);


}