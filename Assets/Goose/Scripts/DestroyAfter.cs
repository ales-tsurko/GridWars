using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float destroyTime = 10f;
	void Start () {
		Destroy (gameObject, destroyTime);
	}
}
