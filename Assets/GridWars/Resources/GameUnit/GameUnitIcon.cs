using UnityEngine;
using System.Collections;

//using UnityEditor;

public class GameUnitIcon : MonoBehaviour {
	// Use this for initialization
	void Start () {
		Destroy(GetComponent<Collider>());
		Destroy(GetComponent<Rigidbody>());
		/*
		 * 1 = opaque
		 * 2 = cutout
		 * 3 = transparent
		gameObject.EachMaterial(m => {
			m.SetFloat("_Mode", 2);
		});
		*/
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.S)) {
			this.enabled = false;
			GetComponent<GameUnit>().enabled = true;
		}
	}
}
