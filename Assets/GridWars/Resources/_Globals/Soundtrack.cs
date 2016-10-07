using UnityEngine;
using System.Collections;

public class Soundtrack : MonoBehaviour {
	public float targetVolume = 1f;
	public float maxVolume = 0.5f;
	public string trackName;
	AssemblyCSharp.Timer fadeTimer = null;
	AudioClip clip;

	public void Play() {
		if (audioSource.isPlaying == false) {
			audioSource.PlayOneShot(clip, maxVolume);
			audioSource.time = 0;
		}
	}

	public string ResourcePath() {
		return  "Sounds/" + trackName;
	}

	public void SetTrackName(string aTrackName) {
		trackName = aTrackName;
		clip = Resources.Load<AudioClip>(ResourcePath());
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

	public void SetVolume(float v) {
		audioSource.volume = maxVolume * v;
		if (Mathf.Approximately(audioSource.volume, 0f)) {
			audioSource.Stop();
		}
	}

	public float AdjustedVolume() {
		return audioSource.volume / maxVolume;
	}

	public void FixedUpdate() {
		if (fadeTimer != null) {
			SetVolume(fadeTimer.RatioDone());
		}

		float v = AdjustedVolume();
		if (Mathf.Approximately(targetVolume, v) == false) {
			v += (targetVolume - v) * 0.05f;
			if (Mathf.Abs(v - targetVolume) < 0.001) {
				v = targetVolume;
			}
			SetVolume(v);
		}
	}

	public void FadeOut() {
		if (fadeTimer == null) {
			fadeTimer = App.shared.timerCenter.NewTimer().SetTimeout(3.0f).SetTarget(this).SetMethod("DoneFade").Start();
		}

	}

	public void DoneFade() {
		audioSource.Stop();
	}

}
