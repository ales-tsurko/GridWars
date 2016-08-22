using UnityEngine;
using System.Collections;

//using UnityEditor;

public class GameUnitIcon : MonoBehaviour {
	public void Enable() {

		// make sure other units don't target the icon!
		GameUnit unit = gameObject.GetComponent<GameUnit>();
		unit.isTargetable = false;

		// disable any unit actions
		DisableScripts(transform);

		// remove physics
		Destroy(GetComponent<Collider>());
		Destroy(GetComponent<Rigidbody>());

		enabled = true;
	}

	void DisableScripts (Transform other){
		foreach (var script in other.GetComponentsInChildren<MonoBehaviour>()) {
			if (script.GetComponent<GameUnit> ()) {
				script.enabled = false;
				continue;
			}
			if (script.GetType () == this.GetType ()) {
				continue;
			}
			Destroy (script);
		}
		foreach (Transform o in other) {
			DisableScripts (o);
		}
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
