using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class Waves : MonoBehaviour {

	void Awake() {

		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh mesh = GetComponent<MeshFilter>().mesh;

		MeshFlattener flattener = new AssemblyCSharp.MeshFlattener();
		flattener.inMesh = mf.mesh;
		flattener.process();
		mf.mesh = flattener.outMesh;
	}

	void Update() {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		//Vector3[] normals = mesh.normals;
		int i = 0;
		while (i < vertices.Length) {
			Vector3 v = vertices[i];
			vertices[i] = new Vector3(v.x, 10f * Mathf.Sin((v.x + v.z + Time.time*5f)), v.z);
			i ++;
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}
}
