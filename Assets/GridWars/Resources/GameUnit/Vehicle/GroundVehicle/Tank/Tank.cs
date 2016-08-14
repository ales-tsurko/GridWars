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

	/*
	public override void Start () {
		base.Start();
	}
	*/
		
	public override void FixedUpdate () {
		base.FixedUpdate();
		PickTarget ();
		steerTowardsNearestEnemy ();
	}
}