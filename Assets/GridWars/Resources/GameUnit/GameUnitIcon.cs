using UnityEngine;
using System.Collections;

//using UnityEditor;

public class GameUnitIcon : MonoBehaviour {
	public void Enable() {
		// make sure other units don't target the icon!
		GameUnit unit = gameObject.GetComponent<GameUnit>();
		unit.isTargetable = false;

		// disable any unit actions
		DisableScripts();

		// remove physics
		Destroy(GetComponent<Collider>());
		Destroy(GetComponent<Rigidbody>());

		enabled = true;
	}

	void DisableScripts (){
		foreach (var script in GetComponentsInChildren<MonoBehaviour>()) {
			if (script.inheritsFrom(typeof(GameUnit))) {
				script.enabled = false;
			}
			else if (!script.inheritsFrom(typeof(GameUnitIcon))) {
				script.enabled = false;
				Destroy(script);
			}
		}
	}
}
