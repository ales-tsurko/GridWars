using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float destroyTime = 10f;
	void Start () {
		Destroy (gameObject, destroyTime);
	}

	/*
	public void FixedUpdate() {
		Material m = gameObject.GetComponent<Renderer>().material;
		Color c = m.color;
		c.a *= .99f;
		m.color = c;
	}
	*/
}
