﻿using UnityEngine;
using System.Collections;

public class BigBoom : Explosion {

	float power = 100000f;

	float maxBlastRadius = 23f;
	float minBlastRadius = 1f;
	float currentBlastRadius = 0f;
	float blastTime = 1f;
	float startTime;
	Vector3 initScale;

	public override void ServerAndClientJoinedGame () {
		base.ServerAndClientJoinedGame();
		Instantiate (Resources.Load<GameObject>("NukeEffect"), _t.position + new Vector3 (0, 3, 0), _t.rotation);
		currentBlastRadius = minBlastRadius;
		startTime = Time.time;
		isTargetable = false;
		initScale = transform.localScale;
	}

	public override void ServerFixedUpdate () {
		base.ServerFixedUpdate();
		if (Mathf.Approximately(DoneRatio(), 1f)) {
			Die();
		} else {
			ApplyForcesAndDamageStep();
		}
	}

	public override void ServerAndClientFixedUpdate () {
		base.ServerAndClientFixedUpdate();
		UpdateRadius();
	}

	public float DoneRatio() {
		return Mathf.Clamp((Time.time - startTime) / blastTime, 0f, 1f);
	}

	public void UpdateRadius() {
		float r = minBlastRadius + (maxBlastRadius - minBlastRadius) * DoneRatio();
		//transform.localScale = new Vector3(initScale.x * r * 2, 0* initScale.y * r * 2, initScale.z * r * 2);
		transform.localScale = new Vector3(initScale.x * r * 2, 0.1f, initScale.z * r * 2);
		currentBlastRadius = r;
		/*
		Material m = GetComponent<Renderer>().material;
		Color color = m.color;
		//color.a = (1f - DoneRatio());
		GetComponent<Renderer>().enabled = false;
		m.color = color;
		*/
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
					rb.AddExplosionForce(power, explosionPos, currentBlastRadius, 0.2f);
				}
					
				// apply damage to unit
				//float dist = Vector3.Distance(explosionPos, unit.gameObject.transform.position);
				//float maxDamage = 1f;
				//float damage = maxDamage / (1f + dist);
				if (unit.IsOfType(typeof(Vehicle))) {
					unit.ApplyDamage(8f * (1 - DoneRatio()));
				} else {
					unit.ApplyDamage(0.4f);
				}
			}
		}
	}

	public override void ApplyDamage(float damage) {
		// can't be damaged
	}

}
