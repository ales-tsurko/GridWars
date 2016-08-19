using UnityEngine;
using System.Collections;

public class Network : Bolt.GlobalEventListener {
	static Network _shared;
	public static Network shared {
		get {
			if (_shared == null) {
				var go = new GameObject();
				go.name = "Network";
				_shared = go.AddComponent<Network>();
			}
			return _shared;
		}
	}

	public bool isMaster {
		get {
			//return false;
			return Application.isEditor;
		}
	}

	public bool isConnected {
		get {
			return BoltNetwork.isConnected;
		}
	}

	public string connectEndpoint = "74.80.237.108:27000";
	//public string connectEndpoint = "127.0.0.1:27000";
	public string listenEndpoint = "0.0.0.0:27000";

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
