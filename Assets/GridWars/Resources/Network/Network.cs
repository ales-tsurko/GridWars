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
		BoltNetwork.RegisterTokenClass<GameUnitState>();
		BoltNetwork.RegisterTokenClass<TowerState>();
		BoltNetwork.RegisterTokenClass<PowerSourceState>();
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
			Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(zeusEndpoint));
		}
	}

	public override void Connected(BoltConnection connection) {
		isConnecting = false;
		if (BoltNetwork.isServer) {
			connectedClients.Add(connection);
		}
		else {
			connectionToServer = connection;
		}

		Battlefield.current.StartGame();
	}

	public BoltConnection ConnectionForPlayer(Player player) {
		if (BoltNetwork.IsSinglePlayer) {
			return null;
		}
		else if (BoltNetwork.isServer) {
			if (player.playerNumber == 1) {
				return null;
			}
			else {
				return connectedClients[player.playerNumber - 2];
			}
			
		}
		else {
			if (player.playerNumber == 1) {
				return connectionToServer;
			}
			else {
				return null;
			}
		}
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
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
	BoltConnection connectionToServer;

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

		/*
		//Join Button Creation
		UIButton joinButton = UI.RoundButton ();
		joinButton.SetText ("Join");
		joinButton.SetAction (StartClient);
		joinButton.SetPosition (.9f, .8f); //sets the position relative to the center of the screen based on the height and width

		//Host Button Creation
		UIButton hostButton = UI.RoundButton ();
		hostButton.SetText ("Host");
		hostButton.SetAction (StartServer);
		hostButton.SetPosition (.9f, .4f); //sets the position relative to the center of the screen based on the height and width
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void StartServer () {
		isStarting = true;
		connectedClients = new List<BoltConnection>();
		BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse(listenEndpoint));
	}

	void StartClient () {
		isStarting = true;
		BoltLauncher.StartClient();
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
						StartServer ();
					}
					else if(GUILayout.Button("Join")) {
						StartClient ();
					}
				}
			}
		}
	}
}
