using UnityEngine;
using System.Collections;

public class BombTruck : GroundVehicle {
	public GameObject bombPrefab;
	public Transform bombTransform;

	[HideInInspector]
	public GameUnit bomb;

	public override void ServerJoinedGame() {
		base.ServerJoinedGame();

		/*
		var initialState = new InitialGameUnitState();
		initialState.position = bombTransform.position;
		initialState.rotation = bombTransform.rotation;
		initialState.player = player;

		bomb = bombPrefab.GetComponent<GameUnit>().Instantiate(initialState);
		*/
	}



}
