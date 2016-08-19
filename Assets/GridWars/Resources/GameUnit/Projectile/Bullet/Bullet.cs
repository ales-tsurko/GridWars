using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Bullet : Projectile {

	public float muzzleImpulse; // define in prefab inspector

	public Bullet() : base() {
		//boltPrefabId = BoltPrefabs.TankShell;
	}

	public override void Start () {
		base.Start();

		rigidBody().AddForce (transform.forward * muzzleImpulse);
	}
		
	protected override GameObject CreateExplosion() {
		var explosion = base.CreateExplosion();
		//var missedUnit = lastCollision.gameObject.GetComponent<GameUnit>() == null;
		//explosion.transform.FindDeepChild("Dirt").gameObject.SetActive(missedUnit);
		//explosion.transform.FindDeepChild("Sparks").gameObject.SetActive(!missedUnit);
		return explosion;
	}

	/*
	public override void FixedUpdate () {
		MakeShot();
	}

	public void MakeShot()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
			print("Found an object - distance: " + hit.distance);
			print("Found an object - distance: " + hit.distance);
		}
	}
	*/
		
}
