using UnityEngine;
using System.Collections;

public class Blockify : MonoBehaviour {

	public int xDivisions;
	public int yDivisions;
	public int zDivisions;

	public Material material = null;

	void Start() {
		Rigidbody rb = gameObject.GetComponent<Rigidbody>();
		BoxCollider b = gameObject.GetComponent<BoxCollider>();
		Vector3 center = b.center;
		Vector3 size = b.size;

		float dx = size.x / (float)xDivisions;
		float dy = size.y / (float)yDivisions;
		float dz = size.z / (float)zDivisions;
		float dMass = rb.mass / (xDivisions * yDivisions * zDivisions);
		Vector3 partSize = new Vector3(dx, dy, dz);


		for (int x = 0; x < xDivisions; x++) {
			for (int y = 0; y < yDivisions; y++) {
				for (int z = 0; z < zDivisions; z++) {

					Vector3 localCenter = center + new Vector3(x * dx - size.x/2f, y * dy - size.y/2f, z * dz - size.z/2f);
					Vector3 partCenter = gameObject.transform.position + localCenter; 

					GameObject part = CreatePart(partCenter, partSize, dMass);


					Rigidbody partRb = part.GetComponent<Rigidbody>();
					Vector3 force = localCenter.normalized * 2000f;
					partRb.AddForce(force);

				}
			}
		}

		Destroy(gameObject);

	}

	GameObject CreatePart(Vector3 center, Vector3 size, float mass) {
		// add cube

		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.layer = gameObject.layer;
		cube.name = "Blockify block";
		cube.transform.localPosition = center;
		cube.EachRenderer(renderer => renderer.material = material);

		// add box collider

		BoxCollider box = cube.GetComponent<BoxCollider>();
		box.center = Vector3.zero;
		box.size = Vector3.one;

		cube.transform.localScale = size;

		// add rigidbody

		Rigidbody parentRb = gameObject.GetComponent<Rigidbody>();

		Rigidbody rb = cube.AddComponent<Rigidbody>();
		rb.mass = mass;
		rb.drag = parentRb.drag;
		rb.angularDrag = parentRb.angularDrag;
		rb.velocity = parentRb.velocity;
		rb.angularVelocity = parentRb.angularVelocity;
		rb.useGravity = true;
		//Rigidbody test = part.GetComponent<Rigidbody>();
		//Debug.Log(test);

		FadeAway w = cube.AddComponent<FadeAway>();
		w.SetFadePeriod(4f);

		/*
		BrightFadeInGeneric fadein = cube.AddComponent<BrightFadeInGeneric>();
		fadein.period = 2f;
		fadein.startColor = Color.yellow;
		fadein.OnEnable();
		*/

		return cube;
	}


}
