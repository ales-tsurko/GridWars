using UnityEngine;
using System.Collections;

public class StandaloneGameUnit : MonoBehaviour, GameUnitDelegate {

	//GameUnitDelegate implementation

	public GameUnit Instantiate(Vector3 position, Quaternion rotation, GameUnitState initialState) { 
		var gameUnit = (GameUnit)Instantiate(gameObject).GetComponent(typeof(GameUnit));
		gameUnit.transform.position = position;
		gameUnit.transform.rotation = rotation;
		gameUnit.gameUnitState = initialState;
		initialState.gameUnit = gameUnit;
		return gameUnit;
	}

	public void DestroySelf() {
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
