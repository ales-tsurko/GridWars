using UnityEngine;
using System.Collections;

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

	public bool isMaster {
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

	public bool isServerOrDisabled {
		get {
			if (enabled) {
				return BoltNetwork.isServer;
			}
			else {
				return true;
			}
		}
	}

	//public string connectEndpoint = "74.80.237.108:27000";
	public string connectEndpoint = "127.0.0.1:27000";
	public string listenEndpoint = "0.0.0.0:27000";

	public override void BoltStartBegin() {
		BoltNetwork.RegisterTokenClass<TowerProtocolToken>();
	}

	public override void BoltStartDone() {
		if (BoltNetwork.isClient) {
			BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(connectEndpoint));
		}
	}

	public override void Connected(BoltConnection connection) {
	}

	// Use this for initialization
	void Start () {
		if (isMaster) {
			BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse(listenEndpoint));
		}
		else {
			BoltLauncher.StartClient();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
