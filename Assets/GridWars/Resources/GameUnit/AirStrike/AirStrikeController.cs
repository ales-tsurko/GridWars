using UnityEngine;
using System.Collections;

public class AirStrikeController : MonoBehaviour {

	public GameObject clusterBombPrefab;
	public Transform bomber;
	public float bomberSpeed;
	public Transform target;
	bool startLaunching;
	public int numBombs;
	public float interval;
	int bombsLaunched;
	float nextLaunch;
	public Transform [] launchPositions = new Transform[3];
	bool initd = false;

	//sound
	AudioSource audioSource;
	public string flyBySound;
	AudioClip flyBySoundClip;

	void Awake () {
		initd = false;
		audioSource = GetComponent <AudioSource> ();
		flyBySoundClip = App.shared.LoadAudioClip(flyBySound);
		audioSource.PlayOneShot (flyBySoundClip);
	}
	public void Init (Vector3 _target){
		startLaunching = false;
		bombsLaunched = 0;
		target.position = new Vector3 (_target.x, 15, _target.z) + (transform.forward * -15);
		bomber.position = new Vector3 (_target.x, 15, _target.z) + (transform.forward * -75);
		initd = true;

	}

	public void Update () {
		if (!initd) {
			return;
		}
		bomber.position += bomber.forward * bomberSpeed * Time.deltaTime;
		if (!startLaunching && Vector3.Distance (bomber.position, target.position) < 4f) {
			startLaunching = true;
		}
		if (!startLaunching) {
			return;
		}
		if (bombsLaunched >= numBombs || Time.time < nextLaunch) {
			return;
		}
		nextLaunch = Time.time + interval;
		Launch ();
	}

	void Launch () {
		foreach (Transform launchPos in launchPositions) {
			Instantiate (clusterBombPrefab, launchPos.position + new Vector3 (Random.Range (-3f, 3f), -.5f, Random.Range (-1.5f, 1.5f)), bomber.rotation);
			bombsLaunched++;
		}
	}


}
