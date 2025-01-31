﻿using UnityEngine;
using System.Collections.Generic;

public class PowerSource : GroundBuilding {
	public Vector3 bounds;

	float _power;
	public float power {
		get {
			return entity.GetState<IPowerSourceState>().power;
		}

		set {
			entity.GetState<IPowerSourceState>().power = value;
		}
	}

	public float maxPower = 20f;
	public float generationRate;

	public float trackSpacing = 1.0f;

	public GameObject segmentPrefab;
	public float baseSegmentWidth = 10f;
	public float baseSegmentLength = 10f;
	public int segmentCount = 20;
	public float generationRateAdjustment = 1f;
	public float segmentVAdjust = 1.0f;

	public GameObject prefab;

    public bool gameStart;

	float trackLength {
		get {
			return bounds.x;
		}
	}

	float trackWidth {
		get {
			return bounds.z;
		}
	}

	List<GameObject>segments;

	public bool IsAtMax() {
		return Mathf.Approximately(power, maxPower);
	}

	public void MakeMax() {
		power = maxPower;
	}

	public float PowerRatio() {
		float p = power / maxPower;
		if (Mathf.Approximately(p, 1f)) {
			return 1f;
		}
		return p;
	}

	public void ShutDown() {
		generationRate = -7f;

	}

	public override void Awake () {
		base.Awake();
        gameStart = false;
		bounds = new Vector3(0f, 1.0f, 2.5f);
		generationRate = 1.3f;
		generationRateAdjustment = 1f;
	}

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();
		shouldAddToPlayerUnits = false;
	}

	public override void ServerInit() {
		base.ServerInit();
		isTargetable = false;
		power = 0f;

		if (App.shared.testEndOfGameMode) {
			MakeMax();
		}
		//MakeMax();
	}

	public override void ClientJoinedGame() {
		base.ClientJoinedGame();
		//need to setup here so its available to other objects.  Server sets it when creating fortress
		SetOnFortress();
	}

	protected virtual void SetOnFortress() {
		player.fortress.powerSource = this;
	}

	public override void ServerAndClientJoinedGame() {
		base.ServerAndClientJoinedGame();

		bounds = new Vector3(player.fortress.bounds.x, bounds.y, bounds.z);

		var segmentLength = (trackLength - trackSpacing*(segmentCount - 1))/segmentCount;

		segments = new List<GameObject>();
		for (var i = 0; i < segmentCount; i ++) {
			var segment = Instantiate<GameObject>(segmentPrefab);
			segment.transform.parent = transform;
			segment.transform.localRotation = Quaternion.identity;

			var offset = -trackLength/2 + segmentLength/2 + i*(segmentLength + trackSpacing);
			var segmentWidthScale = trackWidth/baseSegmentWidth;
			var segmentLengthScale = segmentLength/baseSegmentLength;

			segment.transform.localPosition = new Vector3(offset, 0.1f, 0);
			segment.transform.localScale = new Vector3(segmentLengthScale, segment.transform.localScale.y, segmentWidthScale);

			player.Paint(segment.gameObject, segmentVAdjust);

			//segment.SetActive(false);
			segments.Add(segment);
		}

		App.shared.battlefield.canCheckGameOver = true;
	}

	public override void ServerFixedUpdate() {
		//base.ServerFixedUpdate(); TODO: extract another class from GameUnit so we don't have to perform this perf opt.
		if (!gameStart || App.shared.battlefield.isPaused) {
			return;
		}

		float rate = generationRate;
		float r = power / maxPower;
		rate *= (1 + r / 2);

		power = Mathf.Clamp(power + Time.fixedDeltaTime * rate * generationRateAdjustment, 0f, maxPower);
	}

	public override void Think() {
		// doesn't need to pick targets
	}

	public int lastActiveSegmentCount = 0;

	public int ActiveSegmentCount() {
		return (int)(segmentCount * power / maxPower);
	}

	public override void ServerAndClientUpdate() {
		base.ServerAndClientUpdate();

		int activeSegmentCount = ActiveSegmentCount();

		for (var i = 0; i < segmentCount; i ++) {
			GameObject segment = segments[i];
			bool isActive = (i + 1) <= activeSegmentCount;
			bool wasActive = segment.activeSelf;

			if (isActive != wasActive) {
				segment.SetActive(isActive);

				if (wasActive == false && isActive == true) {
					PulseSegment(segment);
				}
			}
		}

		// pulse segments if power is full

		if ((activeSegmentCount != lastActiveSegmentCount) && 
			(activeSegmentCount == segmentCount)) {
			ShowFull();
		}

		lastActiveSegmentCount = activeSegmentCount;

		// animate shutdown if not generating power (you lost the game)

		if (generationRate < 0f) {
			ShowShutDown();
		}
	}

	void PulseSegment(GameObject segment) {
		BrightFadeInGeneric fader = segment.GetComponent<BrightFadeInGeneric>();
		if (fader.enabled == false) {
			fader.enabled = true;
		}
	}

	void ShowShutDown() {
		for (var i = 0; i < segmentCount; i ++) {
			var segment = segments[i];

			if (UnityEngine.Random.value < 0.02f) {
				PulseSegment(segment);
			}
		}
	}

	void ShowFull() {
		for (var i = 0; i < segmentCount; i ++) {
			var segment = segments[i];
			PulseSegment(segment);
		}
		PlaySoundNamed("fullpower", 0.25f);
	}

	public override void ApplyDamage(float damage) {
		// can't be damaged
	}

}
