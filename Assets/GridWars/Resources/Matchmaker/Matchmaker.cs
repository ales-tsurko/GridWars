using UnityEngine;
using System.Collections;
using SocketIO;
using System;

public interface MatchmakerDelegate {
	void MatchmakerConnected();
	void MatchmakerDisconnected();
	void MatchmakerErrored();
	void MatchmakerReceivedMessage(JSONObject message);
}

public class Matchmaker : AppStateOwner {
	public Network network;
	public MatchmakerDelegate matchmakerDelegate;

	public bool isConnected;

	public MatchmakerMenu menu;

	public AppState state { get; set; }
	public MatchmakerState matchmakerState {
		get {
			return (MatchmakerState)state;
		}
	}

	public Matchmaker() {
		socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();

		socket.On("connect", SocketConnected);
		socket.On("disconnect", SocketDisconnected);
		socket.On("error", SocketError);
		socket.On("message", Receive);

		menu = new GameObject().AddComponent<MatchmakerMenu>();
		menu.backgroundColor = new Color(0, 0, 0, 0);
		state = new MatchmakerDisconnectedState();
		state.matchmaker = this;
		state.owner = this;
		state.EnterFrom(null);

		//ws://gw-matchmaker.herokuapp.com/socket.io/?EIO=4&transport=websocket
		//ws://localhost:8080/socket.io/?EIO=4&transport=websocket
	}

	public void Connect() {
		if (isConnected) {
			throw new Exception("Already connected to Matchmaker");
		}

		isConnected = false;

		App.shared.Log("Start", this);
		socket.Connect();
	}

	public void Disconnect() {
		App.shared.Log("Disconnect", this);
		socket.Close();
	}

	bool isOpen;

	SocketIOComponent socket;

	void SocketConnected(SocketIOEvent e) {
		App.shared.Log("SocketConnected", this);
		isConnected = true;
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerConnected();
		}
	}

	void SocketDisconnected(SocketIOEvent e) {
		App.shared.Log("SocketDisconnected", this);
		isConnected = false;
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerDisconnected();
		}
	}

	bool receivedError = false;
	private System.Object lockObj = new System.Object();

	void SocketError(SocketIOEvent e) {
		lock(lockObj) {
			receivedError = true;
		}
	}

	public void Update() {
		state.Update();

		if (receivedError) {
			lock(lockObj) {
				receivedError = false;
			}

			App.shared.Log("SocketError", this);
			if (matchmakerDelegate != null) {
				matchmakerDelegate.MatchmakerErrored();
			}
		}

	}

	// Messages

	public void Send(string messageName, JSONObject data) {
		App.shared.Log("Send: " + messageName + ": " + data, this);
		JSONObject message = new JSONObject();
		message.AddField("name", messageName);
		message.AddField("data", data);
		socket.Emit("message", message);
	}

	public void Send(string messageName) {
		Send(messageName, new JSONObject(JSONObject.Type.OBJECT));
	}

	public void Receive(SocketIOEvent e) {
		App.shared.Log("Receive: " + e.data.GetField("name").str + ": " + e.data.GetField("data"), this);
		matchmakerDelegate.MatchmakerReceivedMessage(e.data);
	}
}
