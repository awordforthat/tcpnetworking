using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizableTextBox : MonoBehaviour {

	[SerializeField]
	public int maxNumLines;

	private Text textField;
	private RectTransform transform;
	private Queue<string> messageLines;
	private string storedText = "";
	// Use this for initialization
	void Start () {
		textField = GetComponent<Text> ();
		transform = GetComponent<RectTransform> ();
		messageLines = new Queue<string> ();
	}
	
	// Update is called once per frame
	void Update () {

		textField.text = storedText;

	}

	public void AddText(string message)
	{
		if (messageLines.Count >= maxNumLines) 
		{
			messageLines.Dequeue ();
		}

		messageLines.Enqueue (message);

		this.updateText ();
	}

	private void updateText()
	{
		this.storedText = "";

		foreach (string line in messageLines) 
		{
			this.storedText = this.storedText + "\n" + line;
		}
	}
}
