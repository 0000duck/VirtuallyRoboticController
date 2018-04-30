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
		
	}

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
			if(sendPackets){
				Send("L" + UI_Manager.instance.leftLever.value);
				Send("R" + UI_Manager.instance.rightLever.value);
			}
			yield return new WaitForSeconds(tickRate);
		}
	}
}
