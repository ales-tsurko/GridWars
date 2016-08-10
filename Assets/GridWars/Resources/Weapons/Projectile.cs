using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Projectile : MonoBehaviour {
	public GameObject explosionPrefab;
	public float damage = 10;

	public Player player;


	public virtual Rigidbody rigidBody() {
		return GetComponent<Rigidbody> ();
	}

	public void copyVelocityFrom(GameObject obj) {
		rigidBody().velocity = obj.GetComponent<Rigidbody>().velocity;
	}

	public virtual void Start () {
		//base.Start();

	}
		
	public virtual void FixedUpdate () {
	}

	void OnCollisionEnter(Collision collision) {
		// destroy on ground collision
		//if (collision.collider.name == "BattlefieldPlane") {
			//if (collision.relativeVelocity.magnitude > 2) {
				Explode();
				print("Projectile collision");
			//}
		//}
	}

	void Explode() {
		var obj = Instantiate(explosionPrefab);
		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;

		//Transform t = obj.transform;
		//obj.transform.eulerAngles = t.eulerAngles + rotOffset.eulerAngles;
		//print("explode!");
		var otherUnit = gameObject.GetComponent<GameUnit>();
		if (otherUnit != null) {
			otherUnit.ApplyDamage(damage);
		}

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
