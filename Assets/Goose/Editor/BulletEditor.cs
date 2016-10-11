using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Bullet))]
public class BulletEditor : Editor {
    Transform def, alt;
    bool altDisplayed;
    public void Awake () {
        Bullet bullet = (Bullet)target;
        Transform[] ts = bullet.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) {
            if (t.name == "DefaultStyle") {
                def = t;
            }
            if (t.name == "AltStyle") {
                alt = t;
            }
        }
        if (alt == null) {
            return;
        }
        if (alt.gameObject.activeSelf) {
            altDisplayed = true;
            def.gameObject.SetActive(false);
        }
        else {
            altDisplayed = false;
            def.gameObject.SetActive(true);
        }
    }


    public override void OnInspectorGUI(){
        if (alt != null) {
            EditorGUILayout.LabelField("Use Button to Toggle Default and Alternate Styles");
            if (GUILayout.Button("Toggle Style (Current: " + (altDisplayed ? "Alternate" : "Default") + ")")) {
                altDisplayed = !altDisplayed;
                def.gameObject.SetActive(!altDisplayed);
                alt.gameObject.SetActive(altDisplayed);
            }
        }
        EditorGUILayout.Separator();
        DrawDefaultInspector();
    }
}
