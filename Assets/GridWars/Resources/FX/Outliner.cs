using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;


// replaces all meshes in the game object the script is attached to 
// with a wireframe EXCLUDING all non-edge lines.

public class Outliner : MonoBehaviour {

	public float lineWidth = 0.0001f;
	public float pushOutWidth = 0.01f;
	public Material lineMaterial;
	public bool replaceMesh = true;

	void Awake() {
		enabled = false;

		if (!enabled) {
			return;
		}

		List <MeshFilter> existingMeshFilters = new List <MeshFilter>();

		foreach (MeshFilter mf in gameObject.GetComponentsInChildren<MeshFilter>()) {
			existingMeshFilters.Add(mf);
		}

		foreach (MeshFilter mf in existingMeshFilters) {
			Mesh newMesh = OutlineMesh(mf.mesh);
			newMesh.name = "wireframe mesh";

			if (replaceMesh == false) {

				// add a new game object child to hold the wireframe mesh
				GameObject go = new GameObject("wireframe mesh");

				MeshFilter newMeshFilter = go.AddComponent<MeshFilter>();
				newMeshFilter.mesh = newMesh;
				go.transform.parent = mf.gameObject.transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;

				MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
				meshRenderer.material = lineMaterial;
			} else {
				mf.mesh = newMesh;
			}
		}
	}

	public Mesh FlattenMesh(Mesh inMesh) {
		Mesh outMesh = new Mesh();
		List <int> newTriangles = new List<int>();
		List <Vector3> newVertices = new List<Vector3>();

		for (int i = 0; i < inMesh.triangles.Length; i += 3) {

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

		//Debug.Log("in:  triangles: " + inMesh.triangles.Length/3  + " uv: " + inMesh.uv.Length  + " vertices: " + inMesh.vertices.Length);
		//Debug.Log("out: triangles: " + outMesh.triangles.Length/3 + " uv: " + outMesh.uv.Length + " vertices: " + outMesh.vertices.Length);

		Debug.Assert(inMesh.triangles.Length == outMesh.triangles.Length);

		outMesh.RecalculateNormals();
		return outMesh;
	}

	public List<int[]> LinesInMesh(Mesh inMesh) {
		List<int[]> lines = new List<int[]>();

		for (int i = 0; i < inMesh.triangles.Length; i += 3) {
			// add 3 lines for each triangle

			int v1 = inMesh.triangles[i + 0];
			int v2 = inMesh.triangles[i + 1];
			int v3 = inMesh.triangles[i + 2];
			
			lines.Add( new int[] { v1, v2 });
			lines.Add( new int[] { v2, v3 });
			lines.Add( new int[] { v3, v1 });
		}

		return lines;
	}
		
	public List<int[]> EdgeLinesInMesh(Mesh inMesh) {
		List <int[]> allLines = LinesInMesh(inMesh);
		List <int[]> edgeLines = new List<int[]>();

		//print("allLines: " + allLines.Count);

		for (int i = 0; i < allLines.Count; i ++) {
			int[] line1 = allLines[i];
			bool isEdge = true;

			//print("line1: " + line1[0] + ", " + line1[1] + " -------------");

			for (int j = i + 1; j < allLines.Count; j ++) {
				int[] line2 = allLines[j];

				// match the line in either direction

				if (line1.Equals(line2)) {
					if (inMesh.normals[line1[0]] == inMesh.normals[line2[0]]) {
						isEdge = false;
						allLines.RemoveAt(j);
						break;
					}
				}

				if ((line1[0] == line2[1]) && (line1[1] == line2[0])) {
					if (inMesh.normals[line1[0]] == inMesh.normals[line2[1]]) {
						isEdge = false;
						allLines.RemoveAt(j);
						break;
					}
				}
			}

			if (isEdge) {
				edgeLines.Add(line1);
			} 
		}

		return edgeLines;
	}

	public Mesh OutlineMesh(Mesh inMesh) {
		
		Mesh outMesh = new Mesh();
		List <int[]> edgeLines = EdgeLinesInMesh(inMesh);

		List <int> newTriangles = new List<int>();
		List <Vector3> newVertices = new List<Vector3>();

		//print("edgeLines: " + edgeLines.Count);

		foreach(int[] line in edgeLines) {
			Vector3 normal = inMesh.normals[line[0]];
			Vector3 p1 = inMesh.vertices[line[0]];
			Vector3 p2 = inMesh.vertices[line[1]];
			Debug.Assert(line[0] != line[1]);

			List <Vector3> verts = TriangleVerticesForLineAndNormal(p1, p2, normal);
			// returns 6 verts for 2 triangles
			for (int i = 0; i < verts.Count; i++) {
				newTriangles.Add(newVertices.Count);
				newVertices.Add(verts[i]);
			}
		}

		outMesh.vertices  = newVertices.ToArray();
		outMesh.triangles = newTriangles.ToArray();

		//Debug.Log("out: triangles: " + outMesh.triangles.Length/3 + " uv: " + outMesh.uv.Length + " vertices: " + outMesh.vertices.Length);

		outMesh.RecalculateNormals();
		return outMesh;
	}




	List <Vector3> TriangleVerticesForLineAndNormal(Vector3 p1, Vector3 p2, Vector3 normal) {

		p1 += normal * pushOutWidth;
		p2 += normal * pushOutWidth;

		Vector3 forward = (p2 - p1).normalized;
		Vector3 right = Vector3.Cross(forward, normal);
		right *= lineWidth;

		float r = 0f; //lineWidth;

		Vector3 v1 = p1 + right + forward*r; // top right
		Vector3 v2 = p1 - right + forward*r; // top left
		Vector3 v3 = p2 + right - forward*r; // bottom right
		Vector3 v4 = p2 - right - forward*r; // bottom left

		List <Vector3> verts = new List <Vector3>();

		verts.Add(v2);
		verts.Add(v1);
		verts.Add(v3);

		verts.Add(v2);
		verts.Add(v3);
		verts.Add(v4);

		return verts;
	}

}