using UnityEngine;
using System.Collections;

public class TireTrackGenerator : MonoBehaviour {
	int lastMark;
	Transform _t;
	void Start (){
		_t = transform;
	}
	void Update(){
		lastMark = TireTracks.instance.AddSkidMark (_t.position, new Vector3 (0, 1, 0), 1, lastMark);
	}
}
