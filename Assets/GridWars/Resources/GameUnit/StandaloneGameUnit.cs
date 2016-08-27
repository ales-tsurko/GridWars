using UnityEngine;
using System.Collections;

public class StandaloneGameUnit : MonoBehaviour, GameUnitDelegate {

	//GameUnitDelegate implementation

	//return (GameUnit) BoltNetwork.Instantiate(entity.ModifySettings().prefabId, gameUnit.gameUnitState, gameUnit.gameUnitState.position, gameUnit.gameUnitState.rotation).GetComponent(typeof(GameUnit));

	public GameUnit InstantiateGameUnit() { 
		var newGameUnit = (GameUnit)Instantiate(gameObject).GetComponent(typeof(GameUnit));
		newGameUnit.transform.position = gameUnit.gameUnitState.position;
		newGameUnit.transform.rotation = gameUnit.gameUnitState.rotation;
		newGameUnit.gameUnitState = gameUnit.gameUnitState;
		return newGameUnit;
	}

	public virtual void DestroySelf() {
		Destroy(gameObject);
	}

	//Mock NetworkObjectDelegate methods

	void Start() {
		gameUnit.MasterStart();
		gameUnit.SlaveStart();
	}

	void FixedUpdate() {
		gameUnit.MasterFixedUpdate();
		gameUnit.SlaveFixedUpdate();
	}

	//internal

	GameUnit gameUnit {
		get {
			return GetComponent<GameUnit>();
		}
	}
}
