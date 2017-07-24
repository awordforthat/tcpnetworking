using UnityEngine;
using System.Collections;

public class DelegateManager : MonoBehaviour
{


	public delegate void EventTest(int i);
	public static EventTest myEvent;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

