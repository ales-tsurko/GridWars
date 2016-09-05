using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirVehicle : Vehicle {
	public override void Awake() {
		base.Awake();

		launchDirection = Vector3.up;
	}

	public override void ServerJoinedGame() {
		base.ServerJoinedGame();
		IgnoreAirVehicleCollisions();
	}

	public void IgnoreAirVehicleCollisions() {
		AirVehicle[] units = (AirVehicle[])UnityEngine.Object.FindObjectsOfType(typeof(AirVehicle));

		foreach (var unit in units) {
			if (unit != this) {
				Physics.IgnoreCollision(unit.BoxCollider(), BoxCollider(), true);
			}
		}
	}

	override public void UpdateNearestObstacle() {
	}


}
