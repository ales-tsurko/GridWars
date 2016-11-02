using UnityEngine;
using System.Collections;
using SocketIO;
using System;

public interface MatchmakerDelegate {
	void MatchmakerConnected();
	void MatchmakerDisconnected();
	void MatchmakerErrored();
	void MatchmakerReceivedMessage(JSONObject message);


	void MatchmakerReceivedHost(string gameId);
	void MatchmakerReceivedJoin(string gameId);
	void MatchmakerReceivedVersion(string version);
}

public class Matchmaker : AppStateOwner {
	public Network network;
	public MatchmakerDelegate matchmakerDelegate;

	public bool isConnected {
		get {
			return socket.IsConnected;
		}
	}

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
		state = new MatchmakerDisconnectedState();
		state.matchmaker = this;
		state.owner = this;
		state.EnterFrom(null);

		//ws://gw-matchmaker.herokuapp.com/socket.io/?EIO=4&transport=websocket
		//ws://localhost:8080/socket.io/?EIO=4&transport=websocket
	}

	public void Connect() {
		if (socket.IsConnected) {
			throw new Exception("Already connected to Matchmaker");
		}

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
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerConnected();
		}
	}

	void SocketDisconnected(SocketIOEvent e) {
		App.shared.Log("SocketDisconnected", this);
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerDisconnected();
		}
	}

	void SocketError(SocketIOEvent e) {
		App.shared.Log("SocketError", this);
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerErrored();
		}
	}

	void SendVersion() {
		App.shared.Log("SendVersion: " + App.shared.version, this);
		var data = new JSONObject();
		data.AddField("version", App.shared.version);
		socket.Emit("version", data);
	}

	void ReceiveVersion(SocketIOEvent e) {
		var version = e.data.GetField("version").str;
		App.shared.Log("ReceiveVersion: " + version, this);
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerReceivedVersion(version);
		}
	}

	void HostGame(SocketIOEvent e) {
		var gameId = e.data.GetField("gameId").str;
		App.shared.Log("HostGame: " + gameId, this);
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerReceivedHost(gameId);
		}
	}

	void JoinGame(SocketIOEvent e) {
		var gameId = e.data.GetField("gameId").str;
		App.shared.Log("JoinGame: " + gameId, this);
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerReceivedJoin(gameId);
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
