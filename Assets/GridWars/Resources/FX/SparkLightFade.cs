using UnityEngine;
using System.Collections;

public class SparkLightFade : MonoBehaviour {
	public ParticleSystem sparks;
	public float sparkLifetimeRatio = 4f;

	float initialRange;
	float startTime;

	Light impactLight {
		get {
			return GetComponent<Light>();
		}
	}

	float normalizedLifeTime {
		get {
			return (Time.time - startTime)/(sparkLifetimeRatio*sparks.main.duration);
		}
	}

	// Use this for initialization
	void Start () {
		initialRange = impactLight.range;
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		impactLight.color = sparks.colorOverLifetime.color.Evaluate(sparks.time);
		impactLight.range = initialRange*(1 - normalizedLifeTime);
		if (normalizedLifeTime >= 1.0) {
			gameObject.SetActive(false);
		}
	}
}
