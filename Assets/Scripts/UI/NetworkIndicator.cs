using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkIndicator : MonoBehaviour {

	[SerializeField]
	private Image connectionColor;

	[SerializeField]
	private Citrus.AsyncClient clientController;

	// Use this for initialization
	void Start () {
		connectionColor.color = Color.clear;
	}
	
	// Update is called once per frame
	void Update () {
		if (clientController.IsConnected()) {
			connectionColor.color = Color.green;
		} else {
			connectionColor.color = Color.red;
		}

	}
}
