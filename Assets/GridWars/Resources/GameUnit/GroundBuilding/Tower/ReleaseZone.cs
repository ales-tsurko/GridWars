using UnityEngine;
using System.Collections.Generic;

public class ReleaseZone : MonoBehaviour {
	bool _isObstructed;

	public bool isObstructed {
		get {

			if (_isObstructed) {
				return true;
			}

			var myCollider = GetComponent<Collider>();

			foreach (var vehicle in App.shared.stepCache.AllVehicleUnits()) {
				BoxCollider otherCollider = vehicle.BoxCollider();

				if (myCollider.bounds.Intersects(otherCollider.bounds)) {
					return true;
				}
			}
			return false;

			//return obstructions.Count > 0;
		}

		set {
			_isObstructed = value;
		}
	}

	public void AddObstruction(Collider obstruction) {
		//OnTriggerEnter(obstruction);
	}

	/*
	List<Collider> obstructions;

	// Use this for initialization
	void Start () {
		obstructions = new List<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider obstruction) {
		if (!obstructions.Contains(obstruction)) {
			obstructions.Add(obstruction);
		}
	}

	void OnTriggerExit(Collider obstruction) {
		//Debug.Log("OnTriggerExit: " + obstruction.name);
		obstructions.Remove(obstruction);
	}
	*/
}
