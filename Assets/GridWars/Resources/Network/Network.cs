using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
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

	// Use this for initialization
	void Start () {
		indicator = UI.ActivityIndicator("Loading ...");

		menu = UI.Menu();

		menu.AddItem(UI.MenuItem("Shared Screen PVP", SharedScreenPvpClicked));
		menu.AddItem(UI.MenuItem("Internet PVP", InternetPvpClicked));
		menu.AddItem(UI.MenuItem("You vs. Computer", PlayerVsCompClicked));
		menu.AddItem(UI.MenuItem("Computer vs. Computer", CompVsCompClicked));

		menu.Show();
	}

	void SharedScreenPvpClicked(UIMenuItem item) {
		BoltLauncher.StartSinglePlayer();
		menu.Hide();
	}

	void InternetPvpClicked(UIMenuItem item) {
		menu.Reset();

		menu.AddItem (UI.MenuItem("Host", HostClicked));
		menu.AddItem (UI.MenuItem("Join", JoinClicked));

		menu.Show();
	}

	bool vsComp = false;

	void PlayerVsCompClicked(UIMenuItem item) {
		vsComp = true;
		BoltLauncher.StartSinglePlayer();
		menu.Hide();
	}

	bool compVsComp = false;

	void CompVsCompClicked(UIMenuItem item) {
		compVsComp = true;
		BoltLauncher.StartSinglePlayer();
		menu.Hide();
	}

	public void HostClicked(UIMenuItem item) {
		menu.Hide();

		indicator.SetText("Initializing Network ...");
		indicator.Show();

		StartServer();
	}

	void StartServer() {
		connectedClients = new List<BoltConnection>();
		BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse(listenEndpoint));
	}

	public void JoinClicked(UIMenuItem item) {
		menu.Hide();

		indicator.SetText("Retrieving Game List ...");
		indicator.Show();

		StartClient();
	}

	void StartClient() {
		BoltLauncher.StartClient();
	}

	public override void BoltStartBegin() {
		BoltNetwork.RegisterTokenClass<ServerToken>();
	}


	bool didStart = false;
	public override void BoltStartDone() {
		didStart = true;

		if (BoltNetwork.IsSinglePlayer) {
			Battlefield.current.StartGame();
			if (vsComp || compVsComp) {
				Battlefield.current.PlayerNumbered(2).npcModeOn = true;
			}
			if (compVsComp) {
				Battlefield.current.PlayerNumbered(1).npcModeOn = true;
			}
		}
		else {
			if (BoltNetwork.isServer) {
				var st = new ServerToken();
				st.gameName = System.DateTime.UtcNow.ToString();
				BoltNetwork.SetHostInfo("GridWars", st);
				indicator.SetText("Waiting for players ...");
			}
			else {
				indicator.SetText("Retrieving Game List ...");
			}
			Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(zeusEndpoint));
		}
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		if (BoltNetwork.isClient) {
			Bolt.Zeus.RequestSessionList();
		}
	}

	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		base.SessionListUpdated(sessionList);
		menu.Reset();
		indicator.Hide();
		foreach (var session in sessionList) {
			if (session.Value.HostName == "GridWars") {
				var item = UI.MenuItem(
					(session.Value.GetProtocolToken() as ServerToken).gameName,
					GameItemClicked
				);
				item.data = session.Value;
				menu.AddItem(item);
			}
		}
		menu.Show();
	}

	void GameItemClicked(UIMenuItem item) {
		BoltNetwork.Connect(item.data as UdpKit.UdpSession); //TODO: stopped here
		menu.Hide();
		indicator.SetText("Joining Game ...");
		indicator.Show();
	}

	public override void Connected(BoltConnection connection) {
		if (BoltNetwork.isServer) {
			connectedClients.Add(connection);
		}
		else {
			connectionToServer = connection;
		}

		indicator.Hide();
		menu.Hide();

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

	List<BoltConnection> connectedClients;
	BoltConnection connectionToServer;

	bool isGameFull {
		get {
			return connectedClients.Count > 0;
		}
	}

	UIMenu menu;
	UIActivityIndicator indicator;

	void Update() {
		if (didStart && !BoltNetwork.isRunning) {
			UnityEngine.SceneManagement.SceneManager.LoadScene("BattleField");
		}
	}

	/*
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
	*/
}