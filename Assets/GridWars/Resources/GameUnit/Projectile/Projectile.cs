﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Projectile : GameUnit {
	public GameObject explosionPrefab;
	public float damage = 10;

	public AudioClip damageClip;
	public float damageClipVolume;


	public void copyVelocityFrom(GameObject obj) {
		rigidBody().velocity = obj.GetComponent<Rigidbody>().velocity;
	}

	public override void MasterStart () {
		base.MasterStart();
		isTargetable = false;
	}

	public override void SlaveStart() {
		base.SlaveStart();
		PlayBirthSound();
		gameObject.Paint(Color.white, "Unit");
	}

	protected Collision lastCollision;

	void OnCollisionEnter(Collision collision) {
		lastCollision = collision;
		Explode();
		ApplyDamageTo(collision.gameObject);
	}

	void ApplyDamageTo(GameObject otherGameObject) {
		var otherUnit = otherGameObject.GetComponent<GameUnit>();

		if (otherUnit != null) {
			otherUnit.ApplyDamage(damage);
		}
	}

	protected void Explode() {
		if (explosionPrefab != null) {
			CreateExplosion();
		}

		//Transform t = obj.transform;
		//obj.transform.eulerAngles = t.eulerAngles + rotOffset.eulerAngles;
		//print("explode!");

		Destroy (gameObject);
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
