using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Bullet : Projectile {

	public override void Start () {
		base.Start();

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
