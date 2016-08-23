using UnityEngine;
using System.Collections.Generic;
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
		mat = (Material)EditorGUILayout.ObjectField(mat, typeof(Material), true);
		go = (GameObject)EditorGUILayout.ObjectField (go, typeof(GameObject), true);
		if (GUILayout.Button ("Apply")) {
			Apply ();
		}
	}

	void Apply () {
		go.EachRenderer(r => {
			//Debug.Log(r.gameObject.name);
			var properties = r.GetComponent<GameObjectProperties>();
			if (properties == null || !properties.skipMaterialApplicator) {
				r.material = mat;
				/*
				for (var i = 0; i < r.sharedMaterials.Length; i ++) {
					r.sharedMaterials[i] = mat;
				}
				*/
			}
		});

		this.Close ();
	}


}
