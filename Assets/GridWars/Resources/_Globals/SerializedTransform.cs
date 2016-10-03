using UnityEngine;
using System.Collections;

[System.Serializable]
public class SerializedTransform {
	public Vector3 position;
	public Quaternion rotation;

	public SerializedTransform(Transform t) {
		position = t.position;
		rotation = t.rotation;
	}
}
