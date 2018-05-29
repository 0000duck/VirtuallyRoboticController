using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using TMPro;

public class NetworkManager : MonoBehaviour {

	#region singleton
	public static NetworkManager instance;
	void Awake(){
		if(instance == null){
			instance = this;
		}
	}
	#endregion


	public string ip;
	public int port;
	public bool connected = false;
	Socket sender;
	public float tickRate = 0.2f;
	public bool sendPackets = false;

	// Use this for initialization
	void Start () {
		//Connect();
		StartCoroutine(NetworkTick());
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Keypad1)){
			UI_Manager.instance.leftLever.value = -1;
		}
		if(Input.GetKeyDown(KeyCode.Keypad4)){
			UI_Manager.instance.leftLever.value = 0;
		}
		if(Input.GetKeyDown(KeyCode.Keypad7)){
			UI_Manager.instance.leftLever.value = 1;
		}
		if(Input.GetKeyDown(KeyCode.Keypad3)){
			UI_Manager.instance.rightLever.value = -1;
		}
		if(Input.GetKeyDown(KeyCode.Keypad6)){
			UI_Manager.instance.rightLever.value = 0;
		}
		if(Input.GetKeyDown(KeyCode.Keypad9)){
			UI_Manager.instance.rightLever.value = 1;
		}

		if(Input.GetKeyDown(KeyCode.LeftArrow)){
			UI_Manager.instance.leftLever.value = -1;
			UI_Manager.instance.rightLever.value = 1;
		}
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			UI_Manager.instance.leftLever.value = 1;
			UI_Manager.instance.rightLever.value = 1;
		}
		if(Input.GetKeyDown(KeyCode.RightArrow)){
			UI_Manager.instance.leftLever.value = 1;
			UI_Manager.instance.rightLever.value = -1;
		}
		if(Input.GetKeyDown(KeyCode.DownArrow)){
			UI_Manager.instance.leftLever.value = -1;
			UI_Manager.instance.rightLever.value = -1;
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			UI_Manager.instance.leftLever.value = 0;
			UI_Manager.instance.rightLever.value = 0;
		}
	}

	void InstantConnect(){
		try{
			IPAddress ipaddress = IPAddress.Parse(ip);
			IPEndPoint ipendpoint = new IPEndPoint(ipaddress, port);
			sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			sender.Connect(ipendpoint);
			AddMessage("Connected", Color.white);
			connected = true;
			UI_Manager.instance.connectButton.interactable = false;
			UI_Manager.instance.disconnectButton.interactable = true;
			UI_Manager.instance.packetBox.AddMessage("Connected", Color.green);
		}
		catch(Exception e){
			AddMessage("Connection Failed: " + e.Message, Color.red);
		}
	}

	//public void Connect(){
	//	IPAddress ipaddress = IPAddress.Parse (ip);
	//	IPEndPoint ipendpoint = new IPEndPoint (ipaddress, port);
	//	sender = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	//	sender.Connect (ipendpoint);
	//	Debug.Log ("Connected to tank @ " + sender.RemoteEndPoint.ToString ());
		//connected = true;
	//}

	public void Connect(){
		AddMessage("Attempting to connect to " + ip + ":" + port, Color.white);
		UI_Manager.instance.connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connecting...";
		StartCoroutine(ConnectAfterFrame());
	}

	IEnumerator ConnectAfterFrame(){
		yield return null;
		try{
			IPAddress ipaddress = IPAddress.Parse(ip);
			IPEndPoint ipendpoint = new IPEndPoint(ipaddress, port);
			sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			sender.Connect(ipendpoint);
			AddMessage("Connected", Color.white);
			connected = true;
			UI_Manager.instance.connectButton.interactable = false;
			UI_Manager.instance.disconnectButton.interactable = true;
			UI_Manager.instance.packetBox.AddMessage("Connected", Color.green);
		}
		catch(Exception e){
			AddMessage("Connection Failed: " + e.Message, Color.red);
		}
		UI_Manager.instance.connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connect";
	}

	void Send(string message){
		if(connected){
			UI_Manager.instance.packetBox.AddMessage(message, Color.white);
			byte[] msg = Encoding.ASCII.GetBytes(message);
			sender.Send(msg);
		}
		else{
			UI_Manager.instance.packetBox.AddMessage("[!] " + message, Color.red);
		}

	}

	public void Disconnect(){
		if(!connected){
			AddMessage("Can't disconnect: not connected.", Color.white);
			return;
		}
		AddMessage("Closed network conection.", Color.white);
		sender.Shutdown (SocketShutdown.Both);
		sender.Close ();
		connected = false;
		UI_Manager.instance.packetBox.AddMessage("Disconnected.", Color.red);
		UI_Manager.instance.connectButton.interactable = true;
		UI_Manager.instance.disconnectButton.interactable = false;
	}

	void OnApplicationQuit(){
		Disconnect ();
	}

	void AddMessage(string msg, Color c){
		UI_Manager.instance.mainMessageBox.AddMessage(msg, c);
	}

	IEnumerator NetworkTick(){
		while(true){
			if(sendPackets && connected){
				float left = UI_Manager.instance.leftLever.value;
				float right = UI_Manager.instance.rightLever.value;

				if (left < 0) {
					Send("lb" + Mathf.RoundToInt(Mathf.Abs(left) * 100));
				} else {
					Send("lf" + Mathf.RoundToInt(Mathf.Abs(left) * 100));
				}

				if (right < 0) {
					Send("rb" + Mathf.RoundToInt(Mathf.Abs(right) * 100));
				} else {
					Send("rf" + Mathf.RoundToInt(Mathf.Abs(right) * 100));
				}
			}
			yield return new WaitForSeconds(tickRate);
		}
	}
}
