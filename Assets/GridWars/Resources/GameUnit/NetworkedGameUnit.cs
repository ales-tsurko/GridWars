using UnityEngine;
using System.Collections;

public class NetworkedGameUnit : NetworkObject, GameUnitDelegate {
	//GameUnitDelegate implementation

	public GameUnit InstantiateGameUnit() {
		return (GameUnit) BoltNetwork.Instantiate(entity.ModifySettings().prefabId, gameUnit.gameUnitState, gameUnit.gameUnitState.position, gameUnit.gameUnitState.rotation).GetComponent(typeof(GameUnit));
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

	public override void MasterSlaveStart() {
		gameUnit.gameUnitState = (entity.attachToken as GameUnitState);

		boltState.SetTransforms(boltState.transform, transform);

		if (typeof(ITurretedUnitState).IsAssignableFrom(GetType())) {
			var s = entity.GetState<ITurretedUnitState>();
			//TODO: this won't work for more than 1 weapon
			foreach (var weapon in gameUnit.Weapons()) {
				s.SetTransforms(s.turretXTransform, weapon.turretObjX.transform);
				s.SetTransforms(s.turretYTransform, weapon.turretObjY.transform);
			}
		}

		base.MasterSlaveStart();
	}

	public override void MasterStart() {
		gameUnit.gameUnitState.ApplyToBoltState();
		boltState.receivedFirstUpdate = true;
		base.MasterStart();
	}


	public override void SlaveStart() {
		base.SlaveStart();

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
}
