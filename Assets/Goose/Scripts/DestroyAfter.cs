using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float destroyTime = 10f;
	public float endTime;

	void Start () {
		Destroy (gameObject, destroyTime);
		endTime = Time.time + destroyTime - 0.3f;
	}

	public void FixedUpdate() {

		float v = (endTime - Time.time) / destroyTime;
		if (v < 0) { 
			v = 0f;
		}

		//gameObject.SetAlpha(v);
	}
}
