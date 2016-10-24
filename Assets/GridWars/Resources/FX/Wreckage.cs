﻿using UnityEngine;
using System.Collections;

	/*
	 * Setting this script on an object will
	 * disable it's collisions with the Default layer,
	 * wait until the object comes to rest, then 
	 * cause it to sink into the ground and then be removed from the game.
	 * 
	 * */

	public class Wreckage : MonoBehaviour {

	private float deathHeight;
	private float chillPeriod = .5f;
	private float sinkPeriod  = .5f;
	private float chillDoneTime;
	private float sinkStartTime;
	private float sinkDoneTime;

	public AudioClip deathSound;
	public float deathSoundVolume = 1f;
	private AudioSource audioSource;

	public void SetChillPeriod(float v) {
		chillPeriod = v;
	}

	public void SetSinkPeriod(float v) {
		sinkPeriod = v;
	}

	static public void SetupLayerCollisions() {
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wreckage"), LayerMask.NameToLayer("Terrain"), false);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wreckage"), LayerMask.NameToLayer("Default"), true);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wreckage"), LayerMask.NameToLayer("Wreckage"), false);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wreckage"), LayerMask.NameToLayer("Projectile"), true);
	}

	public void Start () {
		VerifyLayer();
		Collider bc = gameObject.GetComponent<Collider>();
		deathHeight = bc.bounds.size.y * 2f;

		chillDoneTime = Time.time + chillPeriod;

		if (Physics.GetIgnoreLayerCollision(LayerMask.NameToLayer("Wreckage"), LayerMask.NameToLayer("Terrain"))) {
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wreckage"), LayerMask.NameToLayer("Terrain"), false);
		}

		if (deathSound != null) {
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.pitch = 1f - 0.2f * (UnityEngine.Random.value);
			audioSource.PlayOneShot(deathSound, deathSoundVolume);
		}
	}

	public void VerifyLayer() {
		if (gameObject.layer != LayerMask.NameToLayer("Wreckage")) {
			print("WARNING: you forgot to set the layer of " + gameObject.name + " to Wreckage");
			gameObject.layer = LayerMask.NameToLayer("Wreckage");
		}
	}

	public void FixedUpdate() {
		float y = transform.position.y;
		if (Time.time > chillDoneTime) {
			if (y < 0.1f) {
				if (sinkDoneTime == 0) {
					sinkStartTime = Time.time;
					sinkDoneTime = Time.time + sinkPeriod;

					DisableRemainingCollisions();
				}
			}
			SinkStep();
		}
	}

	public void DisableRemainingCollisions() {
		gameObject.GetComponent<Rigidbody>().detectCollisions = false;
		gameObject.GetComponent<Collider>().enabled = false;
	}

	public void SinkStep() {
		//Paint();

		float ratio = (Time.time - sinkStartTime) / sinkPeriod;
		SetY( - ratio * deathHeight );

		if (Time.time > sinkDoneTime) {
			if (audioSource == null || !audioSource.isPlaying) {
				AddToDestroyQueue();
			}
		}
	}

	public void SetY(float y) {
		Vector3 pos = transform.position;
		pos.y = y;
		transform.position = pos;
	}

	public bool IsStill() {
		return Mathf.Approximately(gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude, 0.0f);
	}
		
	void AddToDestroyQueue() {
		App.shared.AddToDestroyQueue(gameObject);
	}

	void OnDestroy() {

	}

	int curColor = 1;

	void Paint() {
		if (curColor == 1) {
			curColor = 0;
		} else {
			curColor = 1;
		}

		Color currentColor = new Color(curColor, curColor, curColor);

		gameObject.EachMaterial(mat => {
			mat.color = currentColor;
		});
	}
}
