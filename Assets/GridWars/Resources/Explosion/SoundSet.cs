using UnityEngine;
using System.Collections;

public class SoundSet : MonoBehaviour {
	public AudioClip[] sounds;
	public int[] frequencies;
	public int maxPlays = 4;
	public float minInterval = 0.1f;
	public float meanInterval = 1f; //every 1s
	public float playbackDuration = 1.0f;


	public AudioClip randomClip {
		get {
			var total = 0;
			foreach (var freq in frequencies) {
				total += freq;
			}
			var num = Random.Range(0, total);
			total = 0;
			for (var i = 0; i < frequencies.Length; i ++) {
				total += frequencies[i];
				if (num <= total) {
					return sounds[i];
				}
			}

			return sounds[sounds.Length - 1];
		}
	}

	float startTime = float.MinValue;
	float nextPlayTime = 0f;
	int plays = 0;

	float PoissonRandom() {
		return meanInterval*Mathf.Log(1f - Random.value + 1f);
	}

	void Start() {
		PlayWithRandomness();
	}
		
	void Update() {
		/*
		Debug.Log(Time.time);
		Debug.Log(startTime + playbackDuration);
		Debug.Log(nextPlayTime);
		*/
		if ((Time.time < startTime + playbackDuration) && (plays < maxPlays) && (Time.time >= nextPlayTime)) {
			Play();
		}
	}

	void PlayWithRandomness() {
		startTime = Time.time;
		plays = 0;
		Play();
	}

	void Play() {
		GetComponent<AudioSource>().PlayOneShot(randomClip);
		var nextTime = Mathf.Max(PoissonRandom(), 0.1f);
		//Debug.Log(nextTime);
		nextPlayTime = Time.time + nextTime;
		plays ++;
	}
}
