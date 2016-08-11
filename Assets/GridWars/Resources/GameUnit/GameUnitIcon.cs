using UnityEngine;
using System.Collections;

//using UnityEditor;

public class GameUnitIcon : MonoBehaviour {
	public void Enable() {
		foreach (var script in gameObject.GetComponentsInChildren<MonoBehaviour>()) {
			script.enabled = false;
		}
		Destroy(GetComponent<Collider>());
		Destroy(GetComponent<Rigidbody>());
		enabled = true;
	}

	// Use this for initialization
	void Start () {
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
