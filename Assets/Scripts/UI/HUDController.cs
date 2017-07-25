using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using Citrus;


public class HUDController : MonoBehaviour {

	[SerializeField]
	private Text field_numPacketsReceived;

	[SerializeField]
	private Citrus.AsyncClient clientController;

	[SerializeField]
	private Citrus.AsyncServer serverController;

	private int packetsReceived;

	void Start () {
		this.Reset ();
	}

	private void OnEnable()
	{
		AsyncClient.event_ClientReceivedMessage += this.updateFields;
	}

	private void OnDisable()
	{
		AsyncClient.event_ClientReceivedMessage -= this.updateFields;
	}
	// Update is called once per frame
	void Update () {
		field_numPacketsReceived.text = "Received this many packets so far: " + packetsReceived.ToString ();
	}

	public void Reset()
	{
		field_numPacketsReceived.text = "0"; 
		packetsReceived = 0;
	}

	private void updateFields(byte[] message)
	{
		this.incrementMessageCounter();
	}

	private void incrementMessageCounter()
	{
		packetsReceived++;
	}

}
