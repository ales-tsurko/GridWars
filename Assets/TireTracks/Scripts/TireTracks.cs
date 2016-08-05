﻿using UnityEngine;
using System.Collections.Generic;
public class TireTracks: MonoBehaviour {
	private static TireTracks _instance;
	public static TireTracks instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<TireTracks> ();
			}
			if (_instance == null) {
				GameObject go = new GameObject ();
				go.name = "TireTrackManager";
				_instance = go.AddComponent<TireTracks> ();
				_instance.skidmarksMaterial = Resources.Load<Material> ("TireTracks/Tracks");
			}
			return _instance;
		}
	}
	[SerializeField]
	Material skidmarksMaterial;

	class MarkSection {
		public Vector3 Pos = Vector3.zero;
		public Vector3 Normal = Vector3.zero;
		public Vector4 Tangent = Vector4.zero;
		public Vector3 Posl = Vector3.zero;
		public Vector3 Posr = Vector3.zero;
		public byte Intensity;
		public int LastIndex;
	};

	const int MAX_MARKS = 1024; // Max number of marks on the map
	const float MARK_WIDTH = 0.45f; // Width of the skidmarks.
	const float GROUND_OFFSET = 0.02f;	// Distance above surface
	const float MIN_DISTANCE = 1.0f; // Distance between points in metres. Bigger = more clunky, straight-line skidmarks
	const float MIN_SQR_DISTANCE = MIN_DISTANCE * MIN_DISTANCE;

	int markIndex;
	MarkSection[] skidmarks;
	Mesh marksMesh;
	MeshRenderer mr;
	MeshFilter mf;

	Vector3[] vertices;
	Vector3[] normals;
	Vector4[] tangents;
	Color32[] colors;
	Vector2[] uvs;
	int[] triangles;

	bool updated;
	bool haveSetBounds;
	GameObject terrain;

	void Start() {
		terrain = GameObject.Find ("BattlefieldPlane");
		skidmarks = new MarkSection[MAX_MARKS];
		for (int i = 0; i < MAX_MARKS; i++) {
			skidmarks[i] = new MarkSection();
		}

		mf = GetComponent<MeshFilter> ();
		mr = GetComponent<MeshRenderer>();
		if (mr == null) {
			mr = gameObject.AddComponent<MeshRenderer>();
		}

		marksMesh = new Mesh();
		marksMesh.MarkDynamic();
		if (mf == null) {
			mf = gameObject.AddComponent<MeshFilter>();
		}
		mf.sharedMesh = marksMesh;

		vertices = new Vector3[MAX_MARKS * 4];
		normals = new Vector3[MAX_MARKS * 4];
		tangents = new Vector4[MAX_MARKS * 4];
		colors = new Color32[MAX_MARKS * 4];
		uvs = new Vector2[MAX_MARKS * 4];
		triangles = new int[MAX_MARKS * 6];

		mr.castShadows = false;
		mr.receiveShadows = false;
		mr.material = skidmarksMaterial;
		mr.useLightProbes = false;
	}

	protected void LateUpdate() {
		if (!updated) return;
		updated = false;

		// Reassign the mesh if it's changed this frame
		marksMesh.vertices = vertices;
		marksMesh.normals = normals;
		marksMesh.tangents = tangents;
		marksMesh.triangles = triangles;
		marksMesh.colors32 = colors;
		marksMesh.uv = uvs;

		if (!haveSetBounds) {
			marksMesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
			haveSetBounds = true;
		}
		mf.sharedMesh = marksMesh;
	}

	public List<float> timeToDestroy = new List<float>();

	public int AddSkidMark(Vector3 pos, Vector3 normal, float intensity, int lastIndex) {
		if (intensity > 1) intensity = 1.0f;
		else if (intensity < 0) return -1; if (lastIndex > 0) {
			float sqrDistance = (pos - skidmarks[lastIndex].Pos).sqrMagnitude;
			if (sqrDistance < MIN_SQR_DISTANCE) return lastIndex;
		}
		if (skidmarks == null) {
			return -1;
		}
		pos = new Vector3 (pos.x, terrain.transform.position.y, pos.z);
		MarkSection curSection = skidmarks[markIndex];

		curSection.Pos = pos + normal * GROUND_OFFSET;
		curSection.Normal = normal;
		curSection.Intensity = (byte)(intensity * 255f);
		curSection.LastIndex = lastIndex;

		if (lastIndex != -1) {
			MarkSection lastSection = skidmarks[lastIndex];
			Vector3 dir = (curSection.Pos - lastSection.Pos);
			Vector3 xDir = Vector3.Cross(dir, normal).normalized;

			curSection.Posl = curSection.Pos + xDir * MARK_WIDTH * 0.5f;
			curSection.Posr = curSection.Pos - xDir * MARK_WIDTH * 0.5f;
			curSection.Tangent = new Vector4(xDir.x, xDir.y, xDir.z, 1);

			if (lastSection.LastIndex == -1) {
				lastSection.Tangent = curSection.Tangent;
				lastSection.Posl = curSection.Pos + xDir * MARK_WIDTH * 0.5f;
				lastSection.Posr = curSection.Pos - xDir * MARK_WIDTH * 0.5f;
			}
		}

		UpdateSkidmarksMesh();

		int curIndex = markIndex;
		/*timeToDestroy.Add (Time.time + 5);
		for (int i = 0; i < timeToDestroy.Count; i++) {
			if (Time.time >= timeToDestroy [i]) {
				timeToDestroy [i] = 1000000;
				skidmarks [i] = null;
			}
		}*/
		markIndex = ++markIndex % MAX_MARKS;

		return curIndex;
	}
		
	void UpdateSkidmarksMesh() {
		MarkSection curr = skidmarks[markIndex];

		if (curr.LastIndex == -1) return;

		MarkSection last = skidmarks[curr.LastIndex];
		vertices[markIndex * 4 + 0] = last.Posl;
		vertices[markIndex * 4 + 1] = last.Posr;
		vertices[markIndex * 4 + 2] = curr.Posl;
		vertices[markIndex * 4 + 3] = curr.Posr;

		normals[markIndex * 4 + 0] = last.Normal;
		normals[markIndex * 4 + 1] = last.Normal;
		normals[markIndex * 4 + 2] = curr.Normal;
		normals[markIndex * 4 + 3] = curr.Normal;

		tangents[markIndex * 4 + 0] = last.Tangent;
		tangents[markIndex * 4 + 1] = last.Tangent;
		tangents[markIndex * 4 + 2] = curr.Tangent;
		tangents[markIndex * 4 + 3] = curr.Tangent;

		colors[markIndex * 4 + 0] = new Color32(0, 0, 0, last.Intensity);
		colors[markIndex * 4 + 1] = new Color32(0, 0, 0, last.Intensity);
		colors[markIndex * 4 + 2] = new Color32(0, 0, 0, curr.Intensity);
		colors[markIndex * 4 + 3] = new Color32(0, 0, 0, curr.Intensity);

		uvs[markIndex * 4 + 0] = new Vector2(0, 0);
		uvs[markIndex * 4 + 1] = new Vector2(1, 0);
		uvs[markIndex * 4 + 2] = new Vector2(0, 1);
		uvs[markIndex * 4 + 3] = new Vector2(1, 1);

		triangles[markIndex * 6 + 0] = markIndex * 4 + 0;
		triangles[markIndex * 6 + 2] = markIndex * 4 + 1;
		triangles[markIndex * 6 + 1] = markIndex * 4 + 2;

		triangles[markIndex * 6 + 3] = markIndex * 4 + 2;
		triangles[markIndex * 6 + 5] = markIndex * 4 + 1;
		triangles[markIndex * 6 + 4] = markIndex * 4 + 3;

		updated = true;
	}
}