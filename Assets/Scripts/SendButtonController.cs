using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendButtonController : MonoBehaviour {

	[SerializeField]
	StreamingSender sender;

	[SerializeField]
	InputField input;

	public delegate void Event_SendButtonClicked (int i);
	public static Event_SendButtonClicked event_SendClicked;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SendButtonPressed()
	{
		Debug.Log ("Sending value...");


		if (input.text != "") {
			try
			{
				event_SendClicked(int.Parse(input.text));
			}
			catch (System.Exception e)
			{
				Debug.LogWarning("You must enter a numeric value to send to the client.");
			}
		}
	}
}
