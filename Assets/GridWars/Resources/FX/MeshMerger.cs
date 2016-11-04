using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshMerger : MonoBehaviour {

	private Dictionary<string, List<MeshFilter>> meshes = null;
	private bool foundPrefab = false;

	public void Start_DISABLED_DISABLED_DISABLED_DISABLED() {

		if (gameObject.scene.name == null) {
			Debug.Log(gameObject.name + " is a prefab - MeshMerger bailing");
			return;
		}


		string goName = gameObject.name;
		Debug.Log(gameObject.name + " MeshMerger");

		Vector3 origPos = transform.position;
		Quaternion origRot = transform.rotation;

		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		meshes = new Dictionary<string, List<MeshFilter>>();

		// walk transforms and find meshes
		// sort into dict by material
		// don't go into children with transforms labeled as rotation points
		WalkTransform(transform);

		// walk dict and merge meshes with similar material
		if (!foundPrefab) {
			MergeMeshesForMaterials();
		} else {
			Debug.Log(gameObject.name + " MeshMerger BAILED");
		}

		transform.position = origPos;
		transform.rotation = origRot;

		//BrightFadeInGeneric fader = gameObject.GetComponent<BrightFadeInGeneric>();
		//fader.SetupMaterialColors();
	}

	void WalkTransform(Transform t) {
		if (t.name.StartsWith("Turret")) {
			return;
		}
			
		AddMeshForTransform(t);

		foreach (Transform child in t) {
			WalkTransform(child);
			if (foundPrefab) {
				break;
			}
		}
	}

	void AddMeshForTransform(Transform t) {
		MeshFilter meshFilter = t.GetComponent<MeshFilter>();
		Renderer renderer = t.GetComponent<Renderer>();

		if (meshFilter != null && renderer != null) {
			Material mat = null;

			try {
				mat = renderer.material;
				//mat = renderer.sharedMaterial;
			} catch {
				Debug.Log(gameObject.name + " MeshMerger: found a prefab");
				foundPrefab = true;
				return;
			}

			if (mat != null) {
				if (!meshes.ContainsKey(mat.name)) {
					meshes[mat.name] = new List<MeshFilter>();
				}
				meshes[mat.name].Add(meshFilter);
			} else {
				Debug.Log(gameObject.name + " MeshMerger: missing material on renderer");
				foundPrefab = true;
				return;
			}
		}
	}

	void MergeMeshesForMaterials() {
		//Debug.Log(gameObject.name + " merging " + meshes.Count + " material meshes");

		foreach(KeyValuePair<string, List<MeshFilter>> entry in meshes) {
			//List<MeshFilter> mfs = entry.Value;
			MergeMaterialMeshes(entry.Key, entry.Value);
		}
	}

	void MergeMaterialMeshes(string materialName, List<MeshFilter> mfs) {

		CombineInstance[] combine = new CombineInstance[mfs.Count];
		Material material = null;

		// add old meshes to combine
		for (int i = 0; i < mfs.Count; i++) {
			MeshFilter meshFilter = mfs[i];
			//combine[i].mesh = meshFilter.sharedMesh;
			combine[i].mesh = meshFilter.mesh;
			combine[i].transform = meshFilter.transform.localToWorldMatrix;

			material = meshFilter.gameObject.GetComponent<MeshRenderer>().material;
		}

		// add a child game object to put our new mesh in
		GameObject newGo = new GameObject("merged mesh for material " + material.name);
		newGo.transform.parent = transform;

		MeshFilter newMeshFilter = newGo.AddComponent<MeshFilter>();
		newMeshFilter.mesh = new Mesh();
		newMeshFilter.mesh.CombineMeshes(combine); // combine meshes

		MeshRenderer renderer = newGo.AddComponent<MeshRenderer>();
		renderer.material = material; // set material
		renderer.material.name = material.name;

		// remove old mesh filters and renderers
		for (int i = 0; i < mfs.Count; i++) {
			MeshFilter mf = mfs[i];
			Renderer r = mf.gameObject.GetComponent<Renderer>();
			//Destroy(r);
			//Destroy(mf);
			r.enabled = false;
			//mf.enabled = false;
		}

		Debug.Log(gameObject.name + " " + material.name + " merged " + mfs.Count + " meshes");
	}
}
