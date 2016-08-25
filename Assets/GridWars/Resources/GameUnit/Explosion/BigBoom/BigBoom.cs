using UnityEngine;
using System.Collections;

public class BigBoom : Explosion {

	float power = 100000f;

	float maxBlastRadius = 25f;
	float minBlastRadius = 1f;
	float currentBlastRadius = 0f;
	float blastTime = 1.5f;
	float startTime;


	public override void SlaveStart () {
		base.SlaveStart();
		currentBlastRadius = minBlastRadius;
		startTime = Time.time;
		isTargetable = false;
		//PlaySound();
	}

	public override void MasterFixedUpdate () {
		base.MasterFixedUpdate();
		if (DoneRatio() > 1) {
			DestorySelf();
		} else {
			ApplyForcesAndDamageStep();
		}
	}

	public override void SlaveFixedUpdate () {
		base.SlaveFixedUpdate();
		UpdateRadius();
	}

	public float DoneRatio() {
		return (Time.time - startTime) / blastTime;
	}

	public void UpdateRadius() {
		float r = minBlastRadius + (maxBlastRadius - minBlastRadius) * DoneRatio();
		transform.localScale = new Vector3(r*2, r*2, r*2);
		currentBlastRadius = r;

		Material m = GetComponent<Renderer>().material;
		Color color = m.color;
		color.a = (1f - DoneRatio());
		m.color = color;
	}

	public void ApplyForcesAndDamageStep() {
			
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, currentBlastRadius);

		// find objects in current blast radius

		foreach (Collider hit in colliders) {
			GameObject go = hit.gameObject;
			GameUnit unit = go.GameUnit();

			if (unit && unit != this) {

				// apply force to rigid body if it has one
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				if (rb) {
					rb.AddExplosionForce(power, explosionPos, currentBlastRadius, 3.0F);
				}

				// apply damage to unit
				float dist = Vector3.Distance(explosionPos, unit.gameObject.transform.position);
				//float maxDamage = 1f;
				//float damage = maxDamage / (1f + dist);
				unit.ApplyDamage(0.01f);
			}
		}
	}

	public void ApplyDamage(float damage) {
		// can't be damaged
	}

}
