using UnityEngine;
using System.Collections;

public class DeformMesh : MonoBehaviour {
	public bool pollAltitude = false;
	public Vector2 altitudeThreshold;
	Vector3 [] originalMesh;
	public float damageRadius, maxDisplacement, maxVertexFracture;
	public float randomDamageRadius, randomMaxDisplacement;
	public Vector3 randomMin, randomMax;
	public Vector3 localImpactPos;
	public Vector3 impactVelocity;
	public Vector3 randomImpactPos;
	Mesh mesh;
	void Start (){
		if (pollAltitude) {
			float alt = transform.position.y;
			if (alt < altitudeThreshold.x || alt > altitudeThreshold.y) {
				this.enabled = false;
				return;
			}
		}
		mesh = GetComponent<MeshFilter> ().mesh;
		originalMesh = mesh.vertices;
		mesh.MarkDynamic ();
		ProcessImpact ();
		maxDisplacement += Random.Range (-randomMaxDisplacement, randomMaxDisplacement);
		impactVelocity += new Vector3 (Random.Range (randomMin.x, randomMax.x), Random.Range (randomMin.y, randomMax.y), Random.Range (randomMin.z, randomMax.z));
		localImpactPos += new Vector3 (Random.Range (-randomImpactPos.x, randomImpactPos.x), Random.Range (-randomImpactPos.y, randomImpactPos.y), Random.Range (-randomImpactPos.z, randomImpactPos.z));
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
