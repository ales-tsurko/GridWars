using UnityEngine;
using System.Collections;
using SocketIO;
using System;
using System.Collections.Generic;

public interface MatchmakerDelegate {
	void MatchmakerConnected();
	void MatchmakerDisconnected();
	void MatchmakerErrored();
	void MatchmakerReceivedMessage(JSONObject message);
}

public class Matchmaker : MonoBehaviour, AppStateOwner {
	public MatchmakerDelegate matchmakerDelegate;

	public static string MatchmakerConnectedNotification = "MatchmakerConnectedNotification";
	public static string MatchmakerDisconnectedNotification = "MatchmakerDisconnectedNotification";
	public static string MatchmakerErroredNotification = "MatchmakerErroredNotification";

	bool isConnecting;
	public bool isConnected;

	public MatchmakerMenu menu;

	Queue<SocketIOEvent> events;

	public GameObject socketIoPrefab;

	public MatchmakerMessenger messenger;

	public AppState state { get; set; }
	public MatchmakerState matchmakerState {
		get {
			return (MatchmakerState)state;
		}
	}

	public void Setup() {
		menu = new GameObject().AddComponent<MatchmakerMenu>();
		state = new MatchmakerDisconnectedState();
		state.matchmaker = this;
		state.owner = this;
		state.EnterFrom(null);

		events = new Queue<SocketIOEvent>();

		messenger = new MatchmakerMessenger();
	}

	private System.Object lockObj = new System.Object();

	void EnqueueEvent(SocketIOEvent e) {
		lock(lockObj) {
			events.Enqueue(e);
		}
	}

	void ClearQueue() {
		lock(lockObj) {
			events.Clear();
		}
	}

	SocketIOEvent DequeueEvent() {
		SocketIOEvent e;
		lock(lockObj) {
			if (events.Count > 0) {
				e = events.Dequeue();
			}
			else {
				e = null;
			}
		}
		return e;
	}

	public void Update() {
		state.Update();
		if (messenger.isEnabled) {
			messenger.Update();
		}

		var e = DequeueEvent();
		if (e != null) {
			switch (e.name) {
			case "connect":
				SocketConnected(e);
				break;
			case "disconnect":
				SocketDisconnected(e);
				break;
			case "error":
				SocketError(e);
				break;
			case "message":
				Receive(e);
				break;
			}
		}

	}

	public void Connect() {
		if (isConnecting) {
			throw new Exception("Already connecting to Matchmaker");
		}

		isConnecting = true;

		if (isConnected) {
			throw new Exception("Already connected to Matchmaker");
		}

		isConnected = false;

		socket = Instantiate(socketIoPrefab).GetComponent<SocketIOComponent>();
		socket.gameObject.name = "SocketIO";
		socket.gameObject.transform.parent = transform;

		socket.On("connect", EnqueueEvent);
		socket.On("disconnect", EnqueueEvent);
		socket.On("error", EnqueueEvent);
		socket.On("message", EnqueueEvent);

		App.shared.Log("Connect: " + socket.url, this);

		socket.Connect();
	}

	public void Disconnect() {
		App.shared.Log("Disconnect", this);
		socket.Close();
	}

	bool isOpen;

	SocketIOComponent socket;

	void SocketConnected(SocketIOEvent e) {
		isConnecting = false;

		App.shared.Log("SocketConnected", this);
		isConnected = true;
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerConnected();
		}
		App.shared.notificationCenter.NewNotification()
			.SetName(MatchmakerConnectedNotification)
			.SetSender(this)
			.Post();
	}

	void DestroySocket() {
		ClearQueue();

		if (socket != null) {
			socket.Close();

			socket.Off("connect", EnqueueEvent);
			socket.Off("disconnect", EnqueueEvent);
			socket.Off("error", EnqueueEvent);
			socket.Off("message", EnqueueEvent);

			Destroy(socket.gameObject);
		}


		socket = null;
	}

	void SocketDisconnected(SocketIOEvent e) {
		if (socket == null) { //SocketIOComponent sometimes sends events twice
			return;
		}

		DestroySocket();

		isConnecting = false;
		isConnected = false;

		App.shared.Log("SocketDisconnected", this);

		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerDisconnected();
		}
		App.shared.notificationCenter.NewNotification()
			.SetName(MatchmakerDisconnectedNotification)
			.SetSender(this)
			.Post();
	}

	void SocketError(SocketIOEvent e) {
		if (socket == null) { //SocketIOComponent sometimes sends events twice
			return;
		}

		DestroySocket();

		isConnecting = false;
		isConnected = false;

		App.shared.Log("SocketError", this);
		isConnected = false;
		if (matchmakerDelegate != null) {
			matchmakerDelegate.MatchmakerErrored();
		}
		App.shared.notificationCenter.NewNotification()
			.SetName(MatchmakerErroredNotification)
			.SetSender(this)
			.Post();
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

		App.shared.notificationCenter.NewNotification()
			.SetName(ReceivedMessageNotificationName(e.data.GetField("name").str))
			.SetSender(this)
			.SetData(e.data.GetField("data"))
			.Post();
	}

	public string ReceivedMessageNotificationName(string messageName) {
		return "MatchmakerReceived" + messageName.Capitalized() + "Notification";
	}
}
