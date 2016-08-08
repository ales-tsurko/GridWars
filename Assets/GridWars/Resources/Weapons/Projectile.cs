using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Projectile : MonoBehaviour {
	public Player player;


	public virtual Rigidbody rigidBody() {
		return GetComponent<Rigidbody> ();
	}


	public virtual void Start () {
		//base.Start();

	}
		
	public virtual void FixedUpdate () {
		

	}
}
