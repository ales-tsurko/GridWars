using UnityEngine;
using System.Collections;

//using UnityEditor;

public class GameUnitIcon : MonoBehaviour {
	public void Enable() {
		// make sure other units don't target the icon!
		GameUnit unit = gameObject.GameUnit();
		unit.isTargetable = false;

		unit.deathExplosionPrefab = null;

		// disable any unit actions
		DisableScripts();

		// remove physics
		Destroy(GetComponent<Collider>());
		Destroy(GetComponent<Rigidbody>());

		enabled = true;
	}

	void DisableScripts (){
		foreach (var script in GetComponentsInChildren<MonoBehaviour>()) {
			if (!script.inheritsFrom(typeof(GameUnitIcon))) {
				script.enabled = false;
				Destroy(script);
			}
		}
	}
}
