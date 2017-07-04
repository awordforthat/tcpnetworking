using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;

public class DebugDisplay : MonoBehaviour
{
	[SerializeField]
	private Text textfield;
	[SerializeField]
	private Citrus.AsyncClient clientController;
	[SerializeField]
	private Citrus.AsyncServer serverController;
	private string message = "Begin";

	// Use this for initialization
	void Awake()
	{
		Application.logMessageReceivedThreaded += this.OnLogMessageReceived;
	}
	
	// Update is called once per frame
	void Update()
	{
		this.textfield.text = this.message;

		if(this.clientController != null && Input.GetMouseButtonDown(0))
		{
			this.clientController.Send(2, Encoding.UTF8.GetBytes("this is a test"));
		}
		if(this.serverController != null && Input.GetMouseButtonDown(0))
		{
			Debug.Log ("Sending message...");
			this.serverController.SendToAllClients(2, Encoding.UTF8.GetBytes("this is a test"));
		}
	}

	void OnLogMessageReceived(string condition, string stackTrace, LogType type)
	{
		this.message += "\n" + condition;
	}
}
