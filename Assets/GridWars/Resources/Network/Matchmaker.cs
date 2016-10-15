﻿using UnityEngine;
using System.Collections;
using SocketIO;
using System;

public interface MatchmakerDelegate {
	void MatchmakerDisconnected();
	void MatchmakerErrored();
	void MatchmakerReceivedHost(string gameId);
	void MatchmakerReceivedJoin(string gameId);
	void MatchmakerReceivedVersion(string version);
}

public class Matchmaker {
	public Network network;
	public MatchmakerDelegate matchmakerDelegate;
	public bool isConnected {
		get {
			return socket.IsConnected;
		}
	}

	public Matchmaker() {
		socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();

		socket.On("connect", SocketConnected);
		socket.On("disconnect", SocketDisconnected);
		socket.On("error", SocketError);
		socket.On("hostGame", HostGame);
		socket.On("joinGame", JoinGame);
		socket.On("version", ReceiveVersion);

		//ws://gw-matchmaker.herokuapp.com/socket.io/?EIO=4&transport=websocket
		//ws://localhost:8080/socket.io/?EIO=4&transport=websocket
	}

	public void Start() {
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
		SendVersion();
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
}
