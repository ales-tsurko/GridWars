using UnityEngine;
using System.Collections;

	/*
	 * Setting this script on an object will
	 * disable it's collisions with the Default layer,
	 * wait until the object comes to rest, then 
	 * cause it to sink into the ground and then be removed from the game.
	 * 
	 * */

public class FadeAway : MonoBehaviour {

	private float fadePeriod;
	private float startTime;

	public void SetFadePeriod(float v) {
		fadePeriod = v;
	}

	public void Start () {
		startTime = Time.time;
	}

	public void FixedUpdate() {

		if (App.shared.timeCounter % 10 == 0) {
			float a = 1f - Mathf.Clamp((Time.time - startTime) / fadePeriod, 0f, 1f);

			gameObject.EachMaterial(mat => { 
				Color c = mat.color;
				c.a = a;
				mat.color = c;
			});
		
			if (Time.time - startTime > fadePeriod) {
				AddToDestroyQueue();
			}
		}
	}
		
	void AddToDestroyQueue() {
		App.shared.AddToDestroyQueue(gameObject);
	}
}
