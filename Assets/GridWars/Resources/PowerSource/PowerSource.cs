using UnityEngine;
using System.Collections.Generic;

public class PowerSource : MonoBehaviour {
	public Player player;

	public float power = 0f;
	public float maxPower = 20f;
	public float generationRate = 1.6f;

	public float trackWidth = 2.5f;
	public float trackLength = 100f;
	public float trackSpacing = 1.0f;

	public bool trackAlongZ = false;

	public GameObject segmentPrefab;
	public float baseSegmentWidth = 10f;
	public float baseSegmentLength = 10f;
	public int segmentCount = 20;

	public GameObject prefab;

	public static PowerSource Instantiate() {
		return Instantiate<GameObject>(Resources.Load<GameObject>("PowerSource/PowerSource")).GetComponent<PowerSource>();
	}

	List<GameObject>segments;

	// Use this for initialization
	void Start () {
		var segmentLength = (trackLength - trackSpacing*(segmentCount - 1))/segmentCount;

		segments = new List<GameObject>();
		for (var i = 0; i < segmentCount; i ++) {
			var segment = Instantiate<GameObject>(segmentPrefab);
			segment.transform.parent = transform;
			segment.transform.localRotation = Quaternion.identity;

			var offset = i*(segmentLength + trackSpacing) - trackLength/2 + segmentLength/2;
			var segmentWidthScale = trackWidth/baseSegmentWidth;
			var segmentLengthScale = segmentLength/baseSegmentLength;

			if (trackAlongZ) {
				segment.transform.localPosition = new Vector3(0, 0, offset);
				segment.transform.localScale = new Vector3(segmentWidthScale, segment.transform.localScale.y, segmentLengthScale);
			}
			else {
				segment.transform.localPosition = new Vector3(offset, 0, 0);
				segment.transform.localScale = new Vector3(segmentLengthScale, segment.transform.localScale.y, segmentWidthScale);
			}

			segment.GetComponent<MeshRenderer>().material = player.enabledMaterial;
			segment.SetActive(false);
			segments.Add(segment);
		}
	}

	void FixedUpdate() {
		power = Mathf.Min(power + Time.fixedDeltaTime*generationRate, maxPower);

		var activeSegmentCount = segmentCount*power/maxPower;

		for (var i = 0; i < segmentCount; i ++) {
			segments[i].SetActive((i + 1) <= activeSegmentCount);
		}
	}

	void OnDrawGizmos() {
		/*
		if (player != null) {
			Gizmos.color = player.color;
		}
		Gizmos.DrawCube(transform.position, new Vector3(trackWidth, 1, trackLength));
		*/
	}
}
