using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : GroundVehicle {
	Weapon _turretWeapon;
	public Weapon turretWeapon {
		get {
			if (_turretWeapon == null) {
				_turretWeapon = _t.FindDeepChild("turret").GetComponent<Weapon>();
			}
			return _turretWeapon;
		}
	}

	public override void Start () {
		base.Start();
		thrust = 850;
		rotationThrust = 60;

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