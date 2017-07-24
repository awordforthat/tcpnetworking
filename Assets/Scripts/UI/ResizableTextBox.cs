using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizableTextBox : MonoBehaviour {

	[SerializeField]
	public int maxNumLines;

	private Text textField;
	private RectTransform transform;

	// Use this for initialization
	void Start () {
		textField = GetComponent<Text> ();
		transform = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (textField.text.Split ('\n').Length - 1 > maxNumLines) {
			textField.color = Color.red;
			int newlineIndex = textField.text.IndexOf ('\n');
			string newText = textField.text.Substring (newlineIndex + 1, textField.text.Length - 1 - newlineIndex);
			Debug.Log (newText);
			textField.text = newText;

		} else 
		{
			textField.color = Color.black;
			transform.sizeDelta = new Vector2 (transform.rect.width, textField.preferredHeight);
		}

	}
}
