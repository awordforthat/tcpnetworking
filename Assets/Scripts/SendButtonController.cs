using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendButtonController : MonoBehaviour {

	[SerializeField]
	StreamingSender sender;

	[SerializeField]
	InputField input;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SendButtonPressed()
	{
		Debug.Log ("Sending value...");
		EventManager.TriggerEvent (EventTypes.EVENT_SEND_SERVER_TO_CLIENT);
//		if (input.text != "") {
//			try
//			{
//				sender.SendValueToClient(int.Parse(input.text));
//			}
//			catch (System.Exception e)
//			{
//				Debug.LogWarning("You must enter a numeric value to send to the client.");
//			}
//		}
	}
}
