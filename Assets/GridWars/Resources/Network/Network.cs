using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using AssemblyCSharp;
using System.Net.NetworkInformation;

public class Network : Bolt.GlobalEventListener {
	// public interface
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
		
	public string listenEndpoint = "0.0.0.0";
	public string zeusEndpoint = "159.8.0.207:24000";

	public bool isConnected {
		get {
			return BoltNetwork.isConnected;
		}
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
				return networkDelegate.connection;
			}

		}
		else {
			if (player.playerNumber == 1) {
				return networkDelegate.connection;
			}
			else {
				return null;
			}
		}
	}

	public bool isGameFull {
		get {
			return networkDelegate.connection != null;
		}
	}

	// MonoBehaviour

	void Start () {
		Profiler.maxNumberOfSamplesPerFrame = 1048576; //Unity bug
		App.shared.enabled = true; //Load App so Start gets called
		App.shared.debug = true;

		indicator = UI.ActivityIndicator("Loading ...");

		menu = UI.Menu();

		menu.AddItem(UI.MenuItem("Player vs Player", MultiplayerClicked));
		menu.AddItem(UI.MenuItem("Player vs AI", PlayerVsCompClicked));
		menu.AddItem(UI.MenuItem("AI vs AI", CompVsCompClicked));

		menu.Show();
	}

	void Update() {
		if (!BoltNetwork.isRunning && !Bolt.Zeus.IsConnected) {
			if (didLeaveGame) {
				didLeaveGame = false;
				if (networkDelegate != null) {
					networkDelegate.BoltShutdownCompleted();
				}
				networkDelegate = null; //TODO: is this needed?  ie will anyone try to use it after LoadScene?
				UnityEngine.SceneManagement.SceneManager.LoadScene("BattleField");
			}
			else if (networkDelegate != null && networkDelegate.boltStarted) {
				networkDelegate.boltStarted = false;
				networkDelegate.BoltShutdownCompleted();
			}
		}
	}

	//Menus 

	UIMenu menu;
	UIActivityIndicator indicator;

	void MultiplayerClicked(UIMenuItem item) {
		menu.Reset();

		menu.AddItem(UI.MenuItem("Internet", InternetPvpClicked));
		menu.AddItem(UI.MenuItem("Shared Screen", SharedScreenPvpClicked));
	}

	void SharedScreenPvpClicked(UIMenuItem item) {
		new PvpShared().Start();
		menu.Hide();
	}

	void PlayerVsCompClicked(UIMenuItem item) {
		new VsComp().Start();
		menu.Hide();
	}

	void CompVsCompClicked(UIMenuItem item) {
		new CompVsComp().Start();
		menu.Hide();
	}

	void InternetPvpClicked(UIMenuItem item) {
		menu.Hide();
		indicator.SetText("Finding a game ...");
		indicator.Show();

		new PvpClient().Start();
		//new PvpServer().Start();
	}

	/*
	int nextAvailablePort {
		get {
			IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

			var usedPorts = new List<int>();

			foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
			{
				usedPorts.Add(tcpi.LocalEndPoint.Port)
				if (tcpi.LocalEndPoint.Port==port)
				{
					isAvailable = false;
					break;
				}
			}
		}
	}
	*/

	//Bolt

	public NetworkDelegate networkDelegate;
	bool didLeaveGame = false;

	public override void BoltStartBegin() {
		BoltNetwork.RegisterTokenClass<ServerToken>();
	}
		
	public override void BoltStartDone() {
		base.BoltStartDone();
		networkDelegate.BoltStartDone();
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);
		networkDelegate.ZeusConnected(endpoint);
	}

	public override void ZeusConnectFailed(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnectFailed(endpoint);
		//TODO: implement this
		App.shared.Log("ZeusConnectFailed", this);
	}

	public override void ZeusDisconnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusDisconnected(endpoint);
		App.shared.Log("ZeusDisconnected", this);
	}
		
	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		base.SessionListUpdated(sessionList);
		networkDelegate.SessionListUpdated(sessionList);
	}

	public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRequest(endpoint, token);
		networkDelegate.ConnectRequest(endpoint, token);
	}

	//Game was full
	public override void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRefused(endpoint, token);
		networkDelegate.ConnectRefused(endpoint, token);
	}

	public override void Connected(BoltConnection connection) {
		base.Connected(connection);
		networkDelegate.Connected(connection);
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		networkDelegate.Disconnected(connection);
	}

	public override void BoltShutdownBegin(Bolt.AddCallback registerDoneCallback) {
		base.BoltShutdownBegin(registerDoneCallback);
		App.shared.Log("BoltShutdownBegin", this);
	}

	public void StartGame() {
		indicator.Hide();
		Battlefield.current.StartGame();
	}

	public void RestartBolt() {
		BoltLauncher.Shutdown();
		Bolt.Zeus.Disconnect();
	}

	public void LeaveGame(bool restartBolt = true) {
		didLeaveGame = true;
		Battlefield.current.Pause();
		if (restartBolt) {
			RestartBolt();
		}
	}
}
