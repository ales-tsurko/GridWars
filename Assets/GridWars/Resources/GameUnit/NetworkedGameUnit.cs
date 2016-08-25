using UnityEngine;
using System.Collections;

public class NetworkedGameUnit : NetworkObject, GameUnitDelegate {
	//GameUnitDelegate implementation

	public Player player {
		get {
			return Battlefield.current.PlayerNumbered(gameUnitState.playerNumber);
		}

		set {
			gameUnitState.playerNumber = value.playerNumber;
		}
	}

	public float hitPoints {
		get {
			return gameUnitState.hitPoints;
		}

		set {
			gameUnitState.hitPoints = value;
		}
	}

	public GameUnit Instantiate() {
		return (GameUnit) BoltNetwork.Instantiate(entity.ModifySettings().prefabId).GetComponent(typeof(GameUnit));
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
		base.MasterStart();

		gameUnitState.SetTransforms(gameUnitState.transform, transform);

		if (typeof(ITurretedUnitState).IsAssignableFrom(GetType())) {
			var s = entity.GetState<ITurretedUnitState>();
			//TODO: this won't work for more than 1 weapon
			foreach (var weapon in gameUnit.Weapons()) {
				s.SetTransforms(s.turretXTransform, weapon.turretObjX.transform);
				s.SetTransforms(s.turretYTransform, weapon.turretObjY.transform);
			}
		}
	}

	public override void SlaveStart() {
		base.SlaveStart();
		if (!BoltNetwork.isServer) {
			Destroy(GetComponent<Rigidbody>());
			Destroy(GetComponent<Collider>());
		}
	}

	//internal

	GameUnit gameUnit {
		get {
			return GetComponent<GameUnit>();
		}
	}

	IGameUnitState gameUnitState {
		get {
			return entity.GetState<IGameUnitState>();
		}
	}
}
