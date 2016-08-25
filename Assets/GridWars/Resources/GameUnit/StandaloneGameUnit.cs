using UnityEngine;
using System.Collections;

public class StandaloneGameUnit : MonoBehaviour, GameUnitDelegate {

	//GameUnitDelegate implementation

	Player _player;
	public Player player {
		get {
			return _player;
		}

		set {
			_player = value;
		}
	}

	float _hitPoints;
	public float hitPoints {
		get {
			return _hitPoints;
		}

		set {
			_hitPoints = value;
		}
	}

	public GameUnit Instantiate() { 
		var gameUnit = (GameUnit)Instantiate(gameObject).GetComponent(typeof(GameUnit));
		gameUnit.ApplyInitialState();
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
