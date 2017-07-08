using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizableTextBox : MonoBehaviour {

	private Text text;
	private RectTransform transform;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		transform = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.sizeDelta = new Vector2 (transform.rect.width, text.preferredHeight);
	}
}
