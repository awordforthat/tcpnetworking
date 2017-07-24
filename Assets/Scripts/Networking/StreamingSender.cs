using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class StreamingSender : MonoBehaviour {

	[SerializeField]
	private Citrus.AsyncServer controller;

	[SerializeField]
	private float sendInterval;

	[SerializeField]
	private Boolean manualSend;

	[SerializeField]
	Text TEMPvalueSent;

	private float timeSinceLastSend;
	private int message;

	// Use this for initialization
	void Start () {
		timeSinceLastSend = 0;
		message = 0;

		SendButtonController.event_SendClicked += SendValueToClient;

	}
	
	// Update is called once per frame
	void Update () {

		if (!manualSend) {
			
			if (controller.GetNumClients () > 0) {
			
				if (timeSinceLastSend + Time.deltaTime > sendInterval) {
					controller.SendToClient (2, Encoding.UTF8.GetBytes ("Testing send " + message.ToString ()), 1);
					timeSinceLastSend = 0;
					message++;
				} else {
					timeSinceLastSend += Time.deltaTime;
				}
			}

		}

	}

	void Destroy()
	{
		
	}

	public void SendValueToClient(int value)
	{
		
		if (manualSend) {
			byte[] message = new byte[4];
			message = Citrus.NetworkController.WriteInt32ToByteArray (value, message);
			this.controller.SendToClient (2, message, 1);
			TEMPvalueSent.text = "Value sent: " + value.ToString();
		} else {
			Debug.LogWarning ("You are trying to send a value to the client manually, but the streaming sender is not set to manual mode. " +
			"Please adjust the value in the StreamingSender inspector.");
		}
	}
}
