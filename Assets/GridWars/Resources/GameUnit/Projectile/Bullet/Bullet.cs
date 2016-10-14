using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VolumetricLines;


public class Bullet : Projectile {

	public float muzzleImpulse; // define in prefab inspector

	override public Vector3 ImpusleOnWeapon() {
		return - transform.forward * muzzleImpulse;

	}

	void Start() {
		rigidBody().AddForce (transform.forward * muzzleImpulse);
	}

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();

		rigidBody().AddForce (transform.forward * muzzleImpulse);
	}

	public override void ServerAndClientJoinedGame() {
		base.ServerAndClientJoinedGame();

		var laser = transform.Find("LaserStyle");
		if (laser != null) {
			var playerColor = player.primaryColor;
			var adjustedColor = playerColor.WithV(1.0f);

			var material = laser.Find("Head").GetComponent<MeshRenderer>().sharedMaterial;
			material.color = new Color(adjustedColor.r, adjustedColor.g, adjustedColor.b, material.color.a);

			material = laser.GetComponentInChildren<TrailRenderer>().material;
			material.SetColor("_TintColor", new Color(adjustedColor.r, adjustedColor.g, adjustedColor.b, material.GetColor("_TintColor").a));
		}
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
