using UnityEngine;
using System.Collections;

public class NetworkedGameUnit : Bolt.EntityBehaviour {
	GameUnit gameUnit;

	public override void Attached() {
		base.Attached();

		serverStarted = false;

		gameUnit = GetComponent<GameUnit>();

		gameUnit.gameUnitState = entity.GetState<IGameUnitState>();

		//Debug.Log(this + " Attached");

		if (BoltNetwork.isServer) {
			gameUnit.ServerInit();
		}
		else {
			gameUnit.ClientInit();
		}

		gameUnit.ServerAndClientInit();
	}

	bool serverStarted = false;

	public override void SimulateOwner() {
		base.SimulateOwner();

		if (!gameUnit.isInGame) {
			//App.shared.Log("!isInGame", gameUnit);
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
		gameUnit.ServerAndClientFixedUpdate();
	}
		
	bool clientStarted = false;
		
	protected virtual void FixedUpdate() {
		if (!gameUnit.isInGame) {
			return;
		}

		if (BoltNetwork.isClient) {
			if (debug) {
				Debug.Log(this + " FixedUpdate");
			}

			if (!clientStarted) {
				gameUnit.ClientJoinedGame();
				gameUnit.ServerAndClientJoinedGame();
				clientStarted = true;
			}

			gameUnit.ClientFixedUpdate();
			gameUnit.ServerAndClientFixedUpdate();
		}
	}

	protected virtual void Update() {
		if (!gameUnit.isInGame) {
			return;
		}
			
		if (BoltNetwork.isServer && serverStarted) {
			gameUnit.ServerUpdate();
			gameUnit.ServerAndClientUpdate();
		}
		else if (clientStarted) {
			gameUnit.ClientUpdate();
			gameUnit.ServerAndClientUpdate();
		}
	}

	// internal

	bool debug = false;
}
