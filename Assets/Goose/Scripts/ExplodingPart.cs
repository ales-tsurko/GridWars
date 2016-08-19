using UnityEngine;
using System.Collections;

public class ExplodingPart : MonoBehaviour {

	public float force;
	public Vector3 dir;
	public float torque;
	void Start () {
		transform.parent = null;
		GetComponent<Rigidbody> ().velocity = (transform.TransformDirection(dir) + new Vector3 (Random.Range (-1f, 1f), Random.Range (.1f, .2f), Random.Range (-1f, 1f))) * force;
		if (torque > 0) {
			GetComponent<Rigidbody> ().AddRelativeTorque (new Vector3 (0, 0, Random.Range (.5f, 1.5f)) * ((Random.Range (0, 2) * 2) - 1) * torque, ForceMode.Impulse);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
