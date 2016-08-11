using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ClusterBomb : MonoBehaviour {

	public List<Transform> bombs = new List<Transform>();
	bool fire;
	float nextFire;
	public float interval;
	bool destroyed;
	void Start () {
		fire = false;
		destroyed = false;
	}

	void Update () {
		if (destroyed) {
			return;
		}
		if (!fire && transform.position.y <= 8) {
			fire = true;
		}
		if (!fire || Time.time < nextFire) {
			return;
		}
		if (bombs.Count == 0) {
			//Instantiate small explosion
			destroyed = true;
			Destroy (gameObject, 1.0f);
			return;
		}
		nextFire = Time.time + interval;
		Fire ();

	}

	void Fire(){
		int thisBombNum = Random.Range (0, bombs.Count);
		Transform thisBomb = bombs [thisBombNum];
		if (thisBomb == null) {
			return;
		}
		thisBomb.parent = null;
		thisBomb.GetComponent<MeshRenderer> ().enabled = true;
		thisBomb.GetComponent<Rigidbody> ().velocity = thisBomb.forward * 1f;
		bombs.RemoveAt (thisBombNum);
	}
}
