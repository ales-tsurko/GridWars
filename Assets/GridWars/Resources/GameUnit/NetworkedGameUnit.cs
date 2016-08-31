using UnityEngine;
using System.Collections;

public class NetworkedGameUnit : Bolt.EntityBehaviour {
	GameUnit gameUnit {
		get {
			return GetComponent<GameUnit>();
		}
	}

	public override void Attached() {
		base.Attached();

		//Debug.Log(this + " Attached");

		gameUnit.ServerAndClientInit();

		if (BoltNetwork.isServer) {
			gameUnit.ServerInit();
		}
		else {
			gameUnit.ClientInit();
		}
	}
		
	void Start() {
		if (debug) {
			Debug.Log(this + " Start");
		}
			
		if (BoltNetwork.isClient) {
			gameUnit.ServerAndClientJoinedGame();
			gameUnit.ServerJoinedGame();
		}
	}

	bool serverStarted = false;

	public override void SimulateOwner() {
		base.SimulateOwner();

		if (debug) {
			Debug.Log(this + " SimulateOwner");
		}

		if (!serverStarted) {
			gameUnit.ServerAndClientJoinedGame();
			gameUnit.ServerJoinedGame();
			serverStarted = true;
		}

		gameUnit.ServerFixedUpdate();
	}

	protected virtual void FixedUpdate() {
		if (debug) {
			Debug.Log(this + " FixedUpdate");
		}
		gameUnit.ServerAndClientFixedUpdate();
		if (BoltNetwork.isClient) {
			gameUnit.ClientFixedUpdate();
		}
	}

	protected virtual void Update() {
		gameUnit.ServerAndClientUpdate();
		if (BoltNetwork.isServer) {
			gameUnit.ServerUpdate();
		}
		else {
			gameUnit.ClientUpdate();
		}
	}

	// internal

	bool debug = false;
}
