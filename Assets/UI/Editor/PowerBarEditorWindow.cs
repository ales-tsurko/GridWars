using UnityEngine;
using System.Collections;
using UnityEditor;
public class PowerBarEditorWindow : EditorWindow {
	PowerBarPlacement placement;
	Color color = Color.white;

	[MenuItem ("UI/Power Bar")]
	static void Init () {
		
		PowerBarEditorWindow window = (PowerBarEditorWindow)EditorWindow.GetWindow (typeof (PowerBarEditorWindow));
		window.Show();
	}

	void OnGUI () {
		GUILayout.Label ("Create New Power Bar", EditorStyles.boldLabel);
		placement = (PowerBarPlacement)EditorGUILayout.EnumPopup ("Placement", placement);
		color = EditorGUILayout.ColorField ("Color", color);

		if (GUILayout.Button ("Create")) {
			PowerBar.New (placement, color);
			this.Close ();
		}
	}
}
