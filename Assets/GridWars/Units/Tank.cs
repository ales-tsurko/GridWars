using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : GroundVehicle {

	//public GameObject turret;
	public Weapon turretWeapon;

	public override void Start () {
		base.Start();
		thrust = 850;
		rotationThrust = 60;
		GameObject turret = _t.Find("headdus1").gameObject;
		turretWeapon = turret.GetComponent<Weapon>();
	}


	public override void pickTarget () {
		base.pickTarget();
		turretWeapon.target = target;
	}


	public override void FixedUpdate () {
		pickTarget ();
		steerTowardsNearestEnemy ();
	}
}