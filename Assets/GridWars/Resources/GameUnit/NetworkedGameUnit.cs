using UnityEngine;
using System.Collections;

public class NetworkedGameUnit : Bolt.EntityBehaviour {
	GameUnit gameUnit {
		get {
			return GetComponent<GameUnit>();
		}
	}

	IGameUnitState state {
		get {
			return entity.GetState<IGameUnitState>();
		}
	}

	public override void Attached() {
		base.Attached();

		if (BoltNetwork.isServer) {
			gameUnit.MasterInit();
			state.isAlive = true;
		}
		else if (BoltNetwork.isClient) {
			gameUnit.ClientInit();

			Debug.Log(this + "state:");
			Debug.Log("isAlive: " + state.isAlive);
			Debug.Log("playerNumber: " + state.playerNumber);
			Debug.Log("position: " + transform.position);
			Debug.Log("rotation: " + transform.rotation);

			Destroy(GetComponent<Rigidbody>());

			if (gameUnit.shouldDestroyColliderOnClient) {
				Debug.Log(this + "DestroyCollider");
				Destroy(GetComponent<Collider>());
			}

			if (!state.isAlive) {
				transform.position = Camera.current.transform.position - 100f*Camera.current.transform.forward;
				gameUnit.HideAndDisable();
			}
		}

		state.AddCallback("isAlive", IsAliveChanged);

		gameUnit.SlaveInit();
	}

	void IsAliveChanged() {
		//Debug.Log(this + " ExistsInWorldChanged: " + state.existsInWorld);
		if (state.isAlive) {
			gameUnit.ShowAndEnable();
		}
		else {
			gameUnit.HideAndDisable();
			gameUnit.SlaveDied();
		}
	}

	void Start() {
		//Debug.Log(this + " NetworkedGameObject Start");
		gameUnit.MasterSlaveStart();
		if (BoltNetwork.isServer) {
			gameUnit.MasterStart();
		}
		else if (BoltNetwork.isClient) {
			gameUnit.ClientStart();
		}
		gameUnit.SlaveStart();
	}

	public override void SimulateOwner() {
		gameUnit.MasterFixedUpdate();
	}

	void FixedUpdate() {
		gameUnit.SlaveFixedUpdate();
	}

	void Update() {
		if (entity.isControllerOrOwner) {
			gameUnit.QueuePlayerCommands();
		}
	}
}
