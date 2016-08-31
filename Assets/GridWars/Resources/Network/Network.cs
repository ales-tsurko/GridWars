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

	UIMenu menu;
	UIActivityIndicator indicator;

	// Use this for initialization
	void Start () {
		indicator = UI.ActivityIndicator ("Loading...");

		menu = UI.Menu ();

		menu.AddItem (UI.MenuItem ("Host", HostClicked, MenuItemType.ButtonRound));
		menu.AddItem (UI.MenuItem ("Join", JoinClicked, MenuItemType.ButtonRound));
		menu.AddItem (UI.MenuItem ("Hide", HideAll, MenuItemType.ButtonRound));

		menu.Show();

		if (singlePlayer) {
			BoltLauncher.StartSinglePlayer();
		}
	}

	void RetrievedGameList(Game[] games) {
		menu.Reset();
		foreach (var game in games) {
			var menuItem = UI.MenuItem (game.name, GameClicked, MenuItemType.ButtonSquare);
			menuItem.data = game;
			menuItem.SetSize (200, 50, false);
			menuItem.SetImageType (Image.Type.Sliced);
			menu.AddItem (menuItem);
		}
		menu.Show ();
	}
	void GameClicked(UIMenuItem item) {
		(item.data as Game).Start();
	}

	void RetrieveGameList () {
		Game[] game = new Game[4];
		for (int i = 0; i < game.Length; i++){
			game[i] = new Game () { name = "Game" + UnityEngine.Random.Range (1, 10000) };
		}
		RetrievedGameList (game);
	}

	public void JoinClicked(UIMenuItem item) {
		menu.Hide();
		indicator.SetText ("Retrieving Game List");
		indicator.Show();
		RetrieveGameList();
	}
	public void HostClicked(UIMenuItem item) {
		menu.Hide();
		indicator.SetText ("Waiting for Players");
		indicator.Show();
	}
	public void HideAll (UIMenuItem item) {
		menu.Hide ();
		indicator.Hide ();
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

class Game {
	public string name;
	public void Start(){}
}
