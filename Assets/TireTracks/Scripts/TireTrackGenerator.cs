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
		lastMark = track.AddSkidMark (new Vector3 (_t.position.x, 0.02f, _t.position.z), new Vector3 (0, 1, 0), 1, lastMark);
	}

	void OnDestroy(){
		if (track != null) {
			track.FadeOutAndDie ();
		}
	}
}

public static class TireTrackGeneratorExtension {
	public static Vector3 SurfacePosition (this Transform t, Terrain ter = null){
		if (ter == null) {
			ter = GameObject.FindObjectOfType<Terrain> ();
		}
		if (ter == null) {
			return t.position;
		}
		return new Vector3 (t.position.x, ter.SampleHeight (t.position), t.position.z);
	}
}
