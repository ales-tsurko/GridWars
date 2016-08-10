using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : GroundVehicle {
	public Weapon turretWeapon;

	public override void Start () {
		base.Start();
		thrust = 850;
		rotationThrust = 60;

		turretWeapon = _t.FindDeepChild("turret").GetComponent<Weapon>();
		turretWeapon.enabled = true;
		turretWeapon.owner = gameObject;
		turretWeapon.isFixed = false;
		turretWeapon.aimedAngle = 2.0f;
	}
		
	public override void FixedUpdate () {
		base.FixedUpdate();
		pickTarget ();
		steerTowardsNearestEnemy ();
	}

	public override void pickTarget () {
		base.pickTarget();
		turretWeapon.target = target;
	}

	void OnDisable() {
		turretWeapon.enabled = false;
	}
}