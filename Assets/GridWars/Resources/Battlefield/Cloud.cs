using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {
	Vector3 speed;

	void Start () {
		PickSpeed();
	}

	void PickSpeed () {
		bool flip = UnityEngine.Random.value < 0.5f;
		float sign = UnityEngine.Random.value < 0.5f ? -1f : 1f;
		float s = sign * UnityEngine.Random.Range(20f, 30f) / 60f;

		if (flip) {
			speed = new Vector3(s, 0, 0);
		} else {
			speed = new Vector3(0, 0, s);
		}
	}

	Bounds ParentBounds() {
		return transform.parent.gameObject.BoxBounds();
	}

	public float RandZ() {
		Bounds b = ParentBounds();
		return UnityEngine.Random.Range(-b.extents.z, b.extents.z);
	}
		
	public float RandX() {
		Bounds b = ParentBounds();
		return UnityEngine.Random.Range(-b.extents.x, b.extents.x);
	}

	void Update () {
		transform.localPosition += speed;

		Vector3 p = transform.localPosition;
		Bounds b = ParentBounds();
		float d = 0.1f;


		// x

		if (p.x > b.extents.x) {
			p.x = -b.extents.x + d;
			p.z = RandZ();
		}
			
		if (p.x < -b.extents.x) {
			p.x = b.extents.x - d;
			p.z = RandZ();
		}


		// y

		if (p.y > b.extents.y) {
			p.y = -b.extents.y + d;
		}

		if (p.y < -b.extents.y) {
			p.y = b.extents.y - d;
		}


		// z

		if (p.z > b.extents.z) {
			p.z = -b.extents.z + d;
			p.x = RandX();
		}

		if (p.z < -b.extents.z) {
			p.z = b.extents.z - d;
			p.x = RandX();
		}

		transform.localPosition = p;

	}
}
