using UnityEngine;
using System.Collections;

public class ClusterBombExploder : MonoBehaviour {
	public GameObject explosionEffect;
	public float damage;
	AudioClip explosionSound;
	AudioSource audio;
	void Start () {
		audio = gameObject.AddComponent<AudioSource> ();
		explosionSound = Resources.Load<AudioClip> ("BulletPing");
	}

	void Update () {
		if (transform.position.y <= .1f) {
			Instantiate (explosionEffect, new Vector3(transform.position.x, .1f, transform.position.z), transform.rotation);
			//audio.PlayOneShot (explosionSound);
			Destroy (gameObject, .3f);
		}
	}

	public void OnTriggerEnter (Collider o){
		
		if (o.name.StartsWith ("Cluster")) {
			return;
		}
		if (o.GetComponent<GameUnit> ()) {
			o.GetComponent<GameUnit> ().ApplyDamage (damage);
		}
		Instantiate (explosionEffect, transform.position, transform.rotation);
		//audio.PlayOneShot (explosionSound);
		Destroy (gameObject, 0.3f);
	}

	
}
