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

	List<Collider> obstructions;

	// Use this for initialization
	void Start () {
		obstructions = new List<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider obstruction) {
		Debug.Log("OnTriggerEnter: " + obstruction.name);
		if (!obstructions.Contains(obstruction)) {
			obstructions.Add(obstruction);
		}
	}

	void OnTriggerExit(Collider obstruction) {
		Debug.Log("OnTriggerExit: " + obstruction.name);
		obstructions.Remove(obstruction);
	}
}
