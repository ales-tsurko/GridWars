using UnityEngine;
using System.Collections;

public class Soundtrack : MonoBehaviour {
	public float targetVolume = 1f;
	public float maxVolume = 0.5f;
	public string trackName;
	AudioClip clip;

	public void Play() {
		if (audioSource.isPlaying == false) {
			audioSource.loop = true;
			audioSource.volume = 0f;
			targetVolume = maxVolume;
			audioSource.clip = clip;
			audioSource.Play();
			audioSource.time = 0;
		}
	}

	public string ResourcePath() {
		return  "Sounds/" + trackName;
	}

	public void SetTrackName(string aTrackName) {
		trackName = aTrackName;
		clip = App.shared.LoadAudioClip(ResourcePath());
	}

	AudioSource _audioSource;
	protected AudioSource audioSource {
		get {
			if (_audioSource == null) {
				_audioSource = gameObject.AddComponent<AudioSource>();
			}
			return _audioSource;
		}
	}

	public void SetTargetVolume(float v) {
		targetVolume = v;
	}
		
	private void StopIfZeroVolume() {
		if (audioSource.volume < 0.01f) {
			audioSource.Stop();
		}
	}

	public void FixedUpdate() {
		if (audioSource.isPlaying == true) {
			audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, 0.05f);
			StopIfZeroVolume();
		}
	}

	public void FadeOut() {
		targetVolume = 0f;
	}

	public void DoneFade() {
		audioSource.Stop();
	}

}
