using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;

public class Flattener : MonoBehaviour {

	void Awake() {
		foreach (MeshFilter mf in gameObject.GetComponentsInChildren<MeshFilter>() )
		{
			mf.mesh.uv = null;
			mf.mesh = OutlineMesh(mf.mesh);
		}
	}

	public Mesh OutlineMesh(Mesh inMesh) {

		Mesh outMesh = new Mesh();

		List <int> newTriangles = new List<int>();
		List <Vector3> newVertices = new List<Vector3>();
		//List <Vector2> newUV = new List<Vector2>();

		for (int i = 0; i < inMesh.triangles.Length; i += 3) {

			// for each triangle, we add up to 3 quads
			// for each line, see 

			int vi0 = inMesh.triangles[i + 0];
			int vi1 = inMesh.triangles[i + 1];
			int vi2 = inMesh.triangles[i + 2];

			Vector3 v1 = inMesh.vertices[vi0];
			Vector3 v2 = inMesh.vertices[vi1];
			Vector3 v3 = inMesh.vertices[vi2];

			// add 3 vertices
			int vIndex = newVertices.Count;
			newVertices.Add(v1);
			newVertices.Add(v2);
			newVertices.Add(v3);

			// add 1 triangle - as 3 vert indexes
			newTriangles.Add(vIndex + 2);
			newTriangles.Add(vIndex + 1);
			newTriangles.Add(vIndex + 0);
		}

		outMesh.vertices  = newVertices.ToArray();
		outMesh.triangles = newTriangles.ToArray();

		Debug.Log("in: triangles: " + inMesh.triangles.Length/3 + " uv: " + inMesh.uv.Length + " vertices: " + inMesh.vertices.Length);
		Debug.Log("out: triangles: " + outMesh.triangles.Length/3 + " uv: " + outMesh.uv.Length + " vertices: " + outMesh.vertices.Length);
		Debug.Assert(inMesh.triangles.Length == outMesh.triangles.Length);

		outMesh.RecalculateNormals();
		return outMesh;
	}
}