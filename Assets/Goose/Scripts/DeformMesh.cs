using UnityEngine;
using System.Collections;

public class DeformMesh : MonoBehaviour {

	Vector3 [] originalMesh;
	public float damageRadius, maxDisplacement, maxVertexFracture;
	public Vector3 localImpactPos;
	public Vector3 impactVelocity;
	Mesh mesh;
	void Start (){
		mesh = GetComponent<MeshFilter> ().mesh;
		originalMesh = mesh.vertices;
		mesh.MarkDynamic ();
		ProcessImpact ();
	}

	void ProcessImpact ()
	{
		
			Vector3 contactPoint = transform.TransformPoint(localImpactPos);

	
		Deform(mesh, originalMesh, transform, contactPoint, impactVelocity);

			
	}

	float Deform (Mesh mesh, Vector3[] originalMesh, Transform localTransform, Vector3 contactPoint, Vector3 contactVelocity)
	{
		Vector3[] vertices = mesh.vertices;
		float sqrRadius = damageRadius * damageRadius;
		float sqrMaxDeform = maxDisplacement * maxDisplacement;

		Vector3 localContactPoint = localTransform.InverseTransformPoint(contactPoint);
		Vector3 localContactForce = contactVelocity;

		float totalDamage = 0.0f;
		int damagedVertices = 0;

		for (int i=0; i<vertices.Length; i++)
		{
			float dist = (localContactPoint - vertices [i]).sqrMagnitude;

			if (dist < sqrRadius)
			{
				Vector3 damage = (localContactForce * (damageRadius - Mathf.Sqrt(dist)) / damageRadius) + Random.onUnitSphere * maxVertexFracture;
				vertices[i] += damage;

				Vector3 deform = vertices[i] - originalMesh[i];

				if (deform.sqrMagnitude > sqrMaxDeform)
					vertices[i] = originalMesh[i] + deform.normalized * maxDisplacement;

				totalDamage += damage.magnitude;
				damagedVertices++;
			}
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return damagedVertices > 0? totalDamage / damagedVertices : 0.0f;
	}
}
