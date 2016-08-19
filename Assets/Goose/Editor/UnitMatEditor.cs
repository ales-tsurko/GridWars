using UnityEngine;
using System.Collections;
using UnityEditor;
public class UnitMatEditor : EditorWindow {

	[MenuItem ("Units/Material Applicator")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		UnitMatEditor window = (UnitMatEditor)EditorWindow.GetWindow<UnitMatEditor> (typeof(SceneView));
		window.Show();
	}

	Material mat;
	GameObject go;

	void OnGUI () {
		GUILayout.Label ("Apply Material to all children", EditorStyles.boldLabel);
		mat = (Material)EditorGUILayout.ObjectField(mat, typeof(Material));
		go = (GameObject)EditorGUILayout.ObjectField (go, typeof(GameObject));
		if (GUILayout.Button ("Apply")) {
			Apply ();
		}
	}

	void Apply () {
		Renderer[] rends = go.GetComponentsInChildren<Renderer> ();
		foreach (Renderer rend in rends) {
			if (rend.GetComponent<ParticleSystem> ()) {
				continue;
			}
			rend.material = mat;
		}
		this.Close ();
	}


}
