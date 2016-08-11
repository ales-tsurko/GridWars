using UnityEngine;
using System.Collections;

public class TireTrackGenerator : MonoBehaviour {
	int lastMark;
	Transform _t;
	TireTracks track;
	void Start (){
		GameObject go = new GameObject ();
		track = go.AddComponent<TireTracks> ();
		track.transform.position = Vector3.zero;
		track.name = gameObject.GetInstanceID () + "Track";
		_t = transform;
	}
	void Update(){
		lastMark = track.AddSkidMark (_t.position, new Vector3 (0, 1, 0), 1, lastMark);
	}

	void OnDestroy(){
		track.FadeOutAndDie ();
	}
}
