using UnityEngine;
using System.Collections;

public class BigBoom : Explosion {

	float power = 100000f;

	float maxBlastRadius = 30f;
	float minBlastRadius = 30f;
	float currentBlastRadius = 0f;
	float blastTime = 2.0f;
	float startTime;


	public override void Start () {
		base.Start();
		currentBlastRadius = minBlastRadius;
		startTime = Time.time;
		//PlaySound();
	}

	public override void FixedUpdate () {
		base.FixedUpdate();
	}



	public void BlowUpUpdate() {
		float ratioDone = (Time.time - startTime)/blastTime;

		if (ratioDone > 1) {
			Destroy(this);
		} else {
			currentBlastRadius = minBlastRadius + (maxBlastRadius - minBlastRadius) * ratioDone;
			ApplyForcesAndDamageStep();
		}
	}

	public void ApplyForcesAndDamageStep() {
			
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, currentBlastRadius);

		foreach (Collider hit in colliders) {
			GameObject go = hit.gameObject;
			GameUnit unit = go.GameUnit();

			if (unit) {

				// apply force to rigid body if it has one
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				if (rb) {
					rb.AddExplosionForce(power, explosionPos, currentBlastRadius, 3.0F);
				}

				// apply damage to unit
				float dist = Vector3.Distance(explosionPos, unit.gameObject.transform.position);
				float maxDamage = 1000f;
				float damage = maxDamage / (1f + dist);
				unit.ApplyDamage(damage);
			}

		}
	}
}
