using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Projectile : GameUnit {
	public GameObject explosionPrefab;
	public float damage = 10;

	public AudioClip damageClip;

	public virtual Rigidbody rigidBody() {
		return GetComponent<Rigidbody> ();
	}

	public void copyVelocityFrom(GameObject obj) {
		rigidBody().velocity = obj.GetComponent<Rigidbody>().velocity;
	}

	public virtual void Start () {
		//base.Start();
		PlayBirthSound();
	}
		
	public virtual void FixedUpdate () {
	}

	void OnCollisionEnter(Collision collision) {

				Explode();
		ApplyDamageTo(collision.gameObject);

	}

	void ApplyDamageTo(GameObject otherGameObject) {
		var otherUnit = otherGameObject.GetComponent<GameUnit>();

		print("Explode otherUnit = " + otherUnit);
		if (otherUnit != null) {
			otherUnit.ApplyDamage(damage);
		}
	}

	void Explode() {
		var obj = Instantiate(explosionPrefab);
		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;

		if (damageClip != null) {
			obj.AddComponent<AudioSource>().PlayOneShot(damageClip);
			//audioSource.PlayOneShot(damageClip);
		}

		//Transform t = obj.transform;
		//obj.transform.eulerAngles = t.eulerAngles + rotOffset.eulerAngles;
		//print("explode!");

		Destroy (gameObject);
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
}
