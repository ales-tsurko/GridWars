using UnityEngine;
using System.Collections.Generic;
using System;

public class ServerToken : Bolt.IProtocolToken {
	public string serverVersion = "0.1";
	public string gameName = "0.1";

	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteString(serverVersion);
		packet.WriteString(gameName);
	}

	public void Read(UdpKit.UdpPacket packet) {
		serverVersion = packet.ReadString();
		gameName = packet.ReadString();
	}
}

public class Network : Bolt.GlobalEventListener {
	static Network _shared;
	public static Network shared {
		get {
			if (_shared == null) {
				_shared = new GameObject().AddComponent<Network>();
				_shared.gameObject.name = "Network";
			}
			return _shared;
		}
	}

	public bool singlePlayer = true;

	public bool isServer {
		get {
			//return false; 
			return Application.isEditor;
			//return !Application.isEditor;
		}
	}

	public bool isConnected {
		get {
			return BoltNetwork.isConnected;
		}
	}

	//public string connectEndpoint = "74.80.237.108:27000";
	public string connectEndpoint = "127.0.0.1:27000";
	public string listenEndpoint = "0.0.0.0:27000";
	public string zeusEndpoint = "159.8.0.207:24000";

	public override void BoltStartBegin() {
		BoltNetwork.RegisterTokenClass<TowerProtocolToken>();
		BoltNetwork.RegisterTokenClass<ServerToken>();
	}

	public override void BoltStartDone() {
		isStarting = false;
		if (BoltNetwork.IsSinglePlayer) {
			Battlefield.current.StartGame();
		}
		else {
			if (BoltNetwork.isServer) {
				var st = new ServerToken();
				st.gameName = System.DateTime.UtcNow.ToString();
				BoltNetwork.SetHostInfo("GridWars", st);
			}
			else {
				isRetrievingGameList = true;
			}
			Debug.Log("Conencting to Zeus");
			Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(zeusEndpoint));
		}
	}

	public override void Connected(BoltConnection connection) {
		isConnecting = false;
		if (BoltNetwork.isServer) {
			connectedClients.Add(connection);
		}
		Battlefield.current.StartGame();
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		Debug.Log("Connected to Zeus");
		if (BoltNetwork.isClient) {
			isRetrievingGameList = true;
			Bolt.Zeus.RequestSessionList();
		}
	}

	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		isRetrievingGameList = false;
		base.SessionListUpdated(sessionList);
	}

	List<BoltConnection> connectedClients;

	bool isGameFull {
		get {
			return connectedClients.Count > 0;
		}
	}

	bool isConnecting = false;
	bool isStarting = false;
	bool isRetrievingGameList = false;

	// Use this for initialization
	void Start () {
		if (singlePlayer) {
			BoltLauncher.StartSinglePlayer();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (!singlePlayer) {
			if (BoltNetwork.isRunning) {
				if (BoltNetwork.isServer) {
					if (!isGameFull) {
						GUILayout.Label("Waiting for players ...");
					}
				}
				else {
					if (!BoltNetwork.isConnected) {
						if (isConnecting) {
							GUILayout.Label("Joining Game ...");
						}
						else if (isRetrievingGameList) {
							GUILayout.Label("Retrieving Game List ...");
						}
						else {
							foreach (var session in BoltNetwork.SessionList) {
								if (session.Value.HostName == "GridWars") {
									if (GUILayout.Button((session.Value.GetProtocolToken() as ServerToken).gameName)) {
										BoltNetwork.Connect(session.Value);
									}
								}
							}
						}
					}
				}
			}
			else {
				if (isStarting) {
					GUILayout.Label("Initializing Network ...");
				}
				else {
					if (GUILayout.Button("Host")) {
						isStarting = true;
						connectedClients = new List<BoltConnection>();
						BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse(listenEndpoint));
					}
					else if(GUILayout.Button("Join")) {
						isStarting = true;
						BoltLauncher.StartClient();
					}
				}
			}
		}
	}
}
