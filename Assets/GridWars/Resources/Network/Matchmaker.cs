using UnityEngine;
using System.Collections;
using SocketIO;

public class Matchmaker {
	public Network network;

	public void Init() {
		socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();

		socket.On("open", SocketOpened);
		socket.On("close", SocketClosed);
		socket.On("hostGame", HostGame);
		socket.On("joinGame", JoinGame);
	}

	public void Start() {
		socket.Connect();
	}

	public void Disconnect() {
		socket.Close();
	}

	public void Cancel() {
		Disconnect();
	}

	bool isOpen;

	SocketIOComponent socket;

	void SocketOpened(SocketIOEvent e) {
		if (!isOpen) {
			isOpen = true;
			Debug.Log("SocketOpened");
		}
	}

	void SocketClosed(SocketIOEvent e) {
		Debug.Log("SocketClosed");
	}

	void HostGame(SocketIOEvent e) {
		var gameId = e.data.GetField("gameId").str;
		Debug.Log("HostGame: " + gameId);

		network.HostGame(gameId);
	}

	void JoinGame(SocketIOEvent e) {
		var gameId = e.data.GetField("gameId").str;
		Debug.Log("JoinGame: " + gameId);
		network.JoinGame(gameId);
	}
}
