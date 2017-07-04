using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;


public class HUDController : MonoBehaviour {

	[SerializeField]
	private Text field_numPacketsReceived;

	[SerializeField]
	private Citrus.AsyncClient serverController;

	[SerializeField]
	private Citrus.AsyncServer clientController;
	// Use this for initialization

	private int packetsReceived;

	void Start () {
		this.Reset ();
	}

	private void OnEnable()
	{
		EventManager.StartListening ("ClientReceivedData", this.setMessage);
	}

	private void OnDisable()
	{
		EventManager.StopListening ("ClientReceivedData", this.setMessage);
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void Reset()
	{
		field_numPacketsReceived.text = "0"; 
		packetsReceived = 0;
	}

	private void setMessage()
	{
		packetsReceived++;
		Debug.Log ("Packets received: " + packetsReceived.ToString ());
	}

}
