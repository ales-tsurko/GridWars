using UnityEngine;
using System.Collections.Generic;

public class ReleaseZone : MonoBehaviour {
	public bool isObstructed {
		get {
			return obstructions.Count > 0;
		}
	}

	public void AddObstruction(Collider obstruction) {
		OnTriggerEnter(obstruction);
	}

	public List<Collider> obstructions;

	// Use this for initialization
	void Start () {
		obstructions = new List<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (var obstruction in new List<Collider>(obstructions)) {
			if (obstruction == null) {
				obstructions.Remove(obstruction);
			}
		}
	}

	void OnTriggerEnter(Collider obstruction) {
		if ((obstructions != null) && (obstruction.GetComponent<Vehicle>() != null) && !obstructions.Contains(obstruction)) {
			obstructions.Add(obstruction);
		}
	}

	void OnTriggerExit(Collider obstruction) {
		//Debug.Log("OnTriggerExit: " + obstruction.name);
		obstructions.Remove(obstruction);
	}
}
