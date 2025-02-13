﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Projectile : GameUnit {
	public GameObject explosionPrefab;
	public float damage = 10;

	public AudioClip damageClip;
	public float damageClipVolume;
	public float damageVarianceRatio;
	public bool allowFriendlyFire = true;
	public GameUnit ownerUnit;
	public Weapon ownerWeapon; // can be null
	public float birthTime;
	public float lifeSpan;

	[HideInInspector]

	static public void SetupLayerCollisions() {
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Projectile"), false);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"), true);
	}

	virtual public Vector3 ImpusleOnWeapon() {
		// for subclasses to override if needed
		return new Vector3(0, 0, 0);
	}

	public Vector3 MuzzleKickForce() {
		return - rigidBody().mass * rigidBody().velocity;
	}

	public void copyVelocityFrom(GameObject obj) {
		rigidBody().velocity = obj.GetComponent<Rigidbody>().velocity;
	}

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();

		fadeInPeriod = 0f;
	}

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();
		isTargetable = false;
		damage += damageVarianceRatio * (UnityEngine.Random.value*2f - 1f);
		birthTime = Time.time;
	}

	public override void ServerAndClientJoinedGame() {
		isPlayerPainted = false;
		base.ServerAndClientJoinedGame();
		PlayBirthSound();
	}

	public override void ServerFixedUpdate() {
		//base.ServerFixedUpdate();
		float age = Time.time - birthTime;
		if (age > lifeSpan) {
			Die();
		} else if (age > 4 || age < 0) {
			Die();
			Debug.Log("wut");
		}
	}

	public override void Think() {
		// doesn't need to pick targets
	}

	protected Collision lastCollision;

	public override void OnCollisionEnter(Collision collision) {
		GameUnit otherUnit = collision.gameObject.GameUnit();

		if (otherUnit && otherUnit.player == player) {
			return;
		}

		lastCollision = collision;
		Explode();
		ApplyDamageTo(collision.gameObject);
	}

	bool CanDamageUnit(GameUnit otherUnit) {
		if (player != null) {
			if (allowFriendlyFire == false) {
				if (otherUnit.player == player) {
					return false;
				}
			}
		}
		return true;
	}

	void ApplyDamageTo(GameObject otherGameObject) {
		var otherUnit = otherGameObject.GetComponent<GameUnit>();

		if (otherUnit != null ) {
			if (CanDamageUnit(otherUnit)) {
				bool wasDead = otherUnit.IsDead();
					
				otherUnit.ApplyDamage(damage);

				bool isDead = otherUnit.IsDead();
				if (wasDead == false && isDead == true && ownerUnit != null) {
					ownerUnit.DidKill(otherUnit);
				}
			}
		}
	}

	protected void Explode() {
		if (!gameObject.IsDestroyed()) {
            var rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.isKinematic = true;
            }

            Die();
		}
	}

	public override void ServerAndClientLeftGame(){
		base.ServerAndClientLeftGame();
		AttemptCreateExplosion();
	}

	public void AttemptCreateExplosion() {
		if (explosionPrefab != null) {
			CreateExplosion();
		}

		//Transform t = obj.transform;
		//obj.transform.eulerAngles = t.eulerAngles + rotOffset.eulerAngles;
		//print("explode!");
	}

	protected virtual GameObject CreateExplosion() {
		var obj = Instantiate(explosionPrefab);
		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;

		if (damageClip != null) {
			obj.AddComponent<AudioSource>().PlayOneShot(damageClip, damageClipVolume);
			//audioSource.PlayOneShot(damageClip);
		}
		return obj;
	}
		
	/*
	#if UNITY_EDITOR

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;

		BoxCollider col = GetComponent<BoxCollider>();

		if ( col )
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube( Vector3.zero + col.center, col.size );
		}
	}

	#endif
	*/

	public void IgnoreCollisionsWith(GameObject obj) {
		Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());
	}
}
