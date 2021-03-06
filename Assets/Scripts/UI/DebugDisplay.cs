﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using Citrus;

public class DebugDisplay : MonoBehaviour
{
	[SerializeField]
	private Text textfield;
	[SerializeField]
	private Citrus.AsyncClient clientController;
	[SerializeField]
	private Citrus.AsyncServer serverController;
	private static string message = "Begin";
	private Queue messageLines;
	[SerializeField]
	private ResizableTextBox resizableTextBox;

	// Use this for initialization
	void Awake()
	{
		//Application.logMessageReceivedThreaded += this.OnLogMessageReceived;
		AsyncClient.event_ClientReceivedMessage += this.receiveMessage;
	}

	private void Start()
	{
		messageLines = new Queue ();
	}
	
	// Update is called once per frame
	void Update()
	{
		
		this.textfield.text = DebugDisplay.message;

//		if(this.clientController != null && Input.GetMouseButtonDown(0))
//		{
//			this.clientController.Send(2, Encoding.UTF8.GetBytes("this is a test"));
//		}
//		if(this.serverController != null && Input.GetMouseButtonDown(0))
//		{
//			Debug.Log ("Sending message...");
//			this.serverController.SendToAllClients(2, Encoding.UTF8.GetBytes("this is a test"));
//		}
	}

	void OnLogMessageReceived(string condition, string stackTrace, LogType type)
	{
		DebugDisplay.message += '\n' + condition;
	}
		
	private void receiveMessage(byte[] message)
	{
		String msg = "";
		while (message.Length >= 4) {
			msg += "Packet received: " + NetworkController.NextInt32 (ref message).ToString();
		}
		this.resizableTextBox.AddText (msg);

	}


}
