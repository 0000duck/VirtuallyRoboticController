using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBox : MonoBehaviour {

	public GameObject messageObject;

	public int maxMessages;


	public void AddMessage(string msg, Color c){
		TextMeshProUGUI newMessage = Instantiate(messageObject, transform).GetComponent<TextMeshProUGUI>();
		newMessage.text = "> " + msg;
		newMessage.color = c;

		if(transform.childCount > maxMessages){
			Destroy(transform.GetChild(0).gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			AddMessage("123456", Color.white);
		}
	}
}
