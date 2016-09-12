using UnityEngine;
using System.Collections;

public class GroundEffect : MonoBehaviour {
	public float threshold;

	void Update(){
		if (transform.parent.position.y > threshold) {
			gameObject.SetActive (false);
		} else {
			transform.localPosition = new Vector3 (0, -transform.parent.position.y + .05f, 0);
		}
	}
}
