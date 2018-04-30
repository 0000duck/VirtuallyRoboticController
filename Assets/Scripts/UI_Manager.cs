using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	#region singleton
	public static UI_Manager instance;
	void Awake(){
		if(instance == null){
			instance = this;
		}
	}
	#endregion

	public MessageBox mainMessageBox, packetBox;
	public TMP_InputField ipInput, portInput, tickRateInput;
	public Button connectButton, disconnectButton;
	public Slider leftLever, rightLever;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ProcessInput();
	}

	void ProcessInput(){
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			leftLever.value = 1;
			rightLever.value = 1;
		}
		if(Input.GetKeyDown(KeyCode.DownArrow)){
			leftLever.value = -1;
			rightLever.value = -1;
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow)){
			leftLever.value = -1;
			rightLever.value = 1;
		}
		if(Input.GetKeyDown(KeyCode.RightArrow)){
			leftLever.value = 1;
			rightLever.value = -1;
		}
	}

	public void Connect(){
		NetworkManager.instance.ip = ipInput.text;
		int result;
		if(!int.TryParse(portInput.text, out result)){
			mainMessageBox.AddMessage(portInput.text + " is not a valid port.", Color.red);
			return;
		}
		NetworkManager.instance.port = result;
		NetworkManager.instance.Connect();
	}

	public void Disconnect(){
		NetworkManager.instance.Disconnect();
	}

	public void UpdateTickRate(){
		float result;
		if(float.TryParse(tickRateInput.text, out result)){
			if(result < 0.01f){
				mainMessageBox.AddMessage("Tickrate cannot be less than 0.01.", Color.red);
			}
			else{
				NetworkManager.instance.tickRate = result;
				mainMessageBox.AddMessage("Network Tickrate set to " + result, Color.white);
			}
		}
		else{
			mainMessageBox.AddMessage(tickRateInput.text + " is not a valid tickrate. ", Color.red);
		}

	}
}
