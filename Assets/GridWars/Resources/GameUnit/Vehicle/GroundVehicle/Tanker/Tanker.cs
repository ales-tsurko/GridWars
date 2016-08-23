using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tanker : GroundVehicle {
	float power = 10000f;
	float blastRadius = 30f;


	public override void FixedUpdate() {
		base.FixedUpdate();

		if (IsInStandoffRange()) {
			hitPoints = 0;
		}
	}

	public void BlowUp() {

		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, blastRadius);

		foreach (Collider hit in colliders) {
			GameObject go = hit.gameObject;
			GameUnit unit = go.GameUnit();

			if (unit) {

				// apply force to rigid body if it has one
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				if (rb) {
					rb.AddExplosionForce(power, explosionPos, blastRadius, 3.0F);
				}

				// apply damage to unit
				float dist = Vector3.Distance(explosionPos, unit.gameObject.transform.position);
				float maxDamage = 4000f;
				float damage = maxDamage / (1f + dist);
				unit.ApplyDamage(damage);
			}

		}
	}

	public override void OnDead() {
		base.OnDead();
		BlowUp();
	}

}