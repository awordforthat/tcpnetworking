using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Citrus;

public class PacketInfoDisplayController : MonoBehaviour {

	[SerializeField]
	private Text numPacketsReceivedField;
	private int numPacketsReceived;
	// Use this for initialization
	void Start () {
		AsyncClient.event_ClientReceivedMessage += this.updateFields;
		this.reset ();
	}
	
	// Update is called once per frame
	void Update () {
		this.numPacketsReceivedField.text = "Num packets received: " + this.numPacketsReceived.ToString ();
	}

	private void reset()
	{
		this.numPacketsReceivedField.text = "";
		this.numPacketsReceived = 0;
	}

	//probably should restructure event chain once packet parser is in place
	private void updateFields(byte[] message)
	{
		Debug.Log ("Updating display fields");
		this.numPacketsReceived += 1;
	}
}
