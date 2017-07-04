using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class StreamingSender : MonoBehaviour {

	[SerializeField]
	private Citrus.AsyncServer controller;

	[SerializeField]
	private float sendInterval;

	private float timeSinceLastSend;
	private int message;

	// Use this for initialization
	void Start () {
		timeSinceLastSend = 0;
		message = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (controller.GetNumClients() > 0) {
			if (timeSinceLastSend + Time.deltaTime > sendInterval) {
				controller.SendToClient (2, Encoding.UTF8.GetBytes("Testing send " + message.ToString()), 1);
				timeSinceLastSend = 0;
				message++;
			} else {
				timeSinceLastSend += Time.deltaTime;
			}
		}

	}
}
