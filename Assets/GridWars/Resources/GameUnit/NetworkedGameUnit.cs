using UnityEngine;
using System.Collections;

public class NetworkedGameUnit : NetworkObject, GameUnitDelegate {
	//GameUnitDelegate implementation

	public GameUnit Instantiate(Vector3 position, Quaternion rotation, GameUnitState initialState) {
		return (GameUnit) BoltNetwork.Instantiate(entity.ModifySettings().prefabId, initialState, position, rotation).GetComponent(typeof(GameUnit));
	}

	public void DestroySelf() {
		if (BoltNetwork.isServer) {
			BoltNetwork.Destroy(gameObject);
		}
		else {
			throw new System.Exception("Can't destroy NetworkObject on the Client");
		}
	}

	//NetworkObject overrides

	public override void MasterStart() {
		gameUnit.gameUnitState.ApplyToBoltState();
		boltState.receivedFirstUpdate = true;
		base.MasterStart();
	}

	public override void MasterSlaveStart() {
		gameUnit.gameUnitState = (entity.attachToken as GameUnitState);
		gameUnit.gameUnitState.gameUnit = gameUnit;
		base.MasterSlaveStart();
	}


	public override void SlaveStart() {
		base.SlaveStart();

		boltState.SetTransforms(boltState.transform, transform);

		if (typeof(ITurretedUnitState).IsAssignableFrom(GetType())) {
			var s = entity.GetState<ITurretedUnitState>();
			//TODO: this won't work for more than 1 weapon
			foreach (var weapon in gameUnit.Weapons()) {
				s.SetTransforms(s.turretXTransform, weapon.turretObjX.transform);
				s.SetTransforms(s.turretYTransform, weapon.turretObjY.transform);
			}
		}

		if (!BoltNetwork.isServer) {
			Destroy(GetComponent<Rigidbody>());
			Destroy(GetComponent<Collider>());
		}
	}

	//internal

	void ReceivedFirstUpdate() {
		if (boltState.receivedFirstUpdate) {
			gameUnit.gameUnitState.useBoltState = true;
		}
	}

	GameUnit gameUnit {
		get {
			return GetComponent<GameUnit>();
		}
	}

	IGameUnitState boltState {
		get {
			return entity.GetState<IGameUnitState>();
		}
	}

	GameUnitState initialState;
}
