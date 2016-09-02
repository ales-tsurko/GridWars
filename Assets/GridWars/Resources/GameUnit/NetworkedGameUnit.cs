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

		if (BoltNetwork.isServer) {
			gameUnit.ServerInit();
		}
		else {
			gameUnit.ClientInit();
		}

		gameUnit.ServerAndClientInit();
	}
		
	void Start() {
		if (debug) {
			Debug.Log(this + " Start");
		}
			
		if (BoltNetwork.isClient) {
			gameUnit.ClientJoinedGame();
			gameUnit.ServerAndClientJoinedGame();
		}
	}

	bool serverStarted = false;

	public override void SimulateOwner() {
		base.SimulateOwner();

		if (!gameUnit.isInGame) {
			return;
		}

		if (debug) {
			Debug.Log(this + " SimulateOwner");
		}

		if (!serverStarted) {
			gameUnit.ServerJoinedGame();
			gameUnit.ServerAndClientJoinedGame();
			serverStarted = true;
		}

		gameUnit.ServerFixedUpdate();
	}

	protected virtual void FixedUpdate() {
		if (!gameUnit.isInGame) {
			return;
		}

		if (debug) {
			Debug.Log(this + " FixedUpdate");
		}
			
		if (BoltNetwork.isClient) {
			gameUnit.ClientFixedUpdate();
		}

		gameUnit.ServerAndClientFixedUpdate();
	}

	protected virtual void Update() {
		if (!gameUnit.isInGame) {
			return;
		}
			
		if (BoltNetwork.isServer) {
			gameUnit.ServerUpdate();
		}
		else {
			gameUnit.ClientUpdate();
		}

		gameUnit.ServerAndClientUpdate();
	}

	// internal

	bool debug = false;
}
