using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Projectile : GameUnit {
	public GameObject explosionPrefab;
	public float damage = 10;

	public AudioClip damageClip;
	public float damageClipVolume;
	public bool allowFriendlyFire = true;

	[HideInInspector]


	public void copyVelocityFrom(GameObject obj) {
		rigidBody().velocity = obj.GetComponent<Rigidbody>().velocity;
	}

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();
		isTargetable = false;
	}

	public override void ServerAndClientJoinedGame() {
		isPlayerPainted = false;
		base.ServerAndClientJoinedGame();
		PlayBirthSound();
		//gameObject.Paint(Color.white, "Unit");
	}

	public override void Think() {
		// doesn't need to pick targets
	}

	protected Collision lastCollision;

	public override void OnCollisionEnter(Collision collision) {
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
				otherUnit.ApplyDamage(damage);
			}
		}
	}

	protected void Explode() {
		if (!gameObject.IsDestroyed()) {
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
