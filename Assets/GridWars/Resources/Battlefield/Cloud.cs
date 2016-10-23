using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {
	float speed;
	public bool isForwardOnly;
	public Material material;

	void Start () {
		PickSpeed();

		gameObject.EachRenderer(r => r.receiveShadows = false);

		if (material) {
			gameObject.EachRenderer(r => r.material = material);
		}
	}

	void PickSpeed () {
		speed = UnityEngine.Random.Range(20f, 30f) / 120f;

		int r = (int)Mathf.Floor(UnityEngine.Random.value * 4);

		if (r == 0) {
			gameObject.SetRotY(gameObject.RotY() + 0f);
		} else if (r == 1) {
			gameObject.SetRotY(gameObject.RotY() + 90f);
		} else if (r == 2) {
			gameObject.SetRotY(gameObject.RotY() - 90f);
		} else if (r == 3) {
			gameObject.SetRotY(gameObject.RotY() + 180f);
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
		transform.localPosition += transform.forward * speed;

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
