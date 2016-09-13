using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KeyboardCanvas : MonoBehaviour {
    private static KeyboardCanvas _instance;
    public static KeyboardCanvas instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<KeyboardCanvas>();
            }
            return _instance;
        }
    }
	// Use this for initialization
	void Start () {
		canvas = GetComponent<Canvas> ();
		canvas.worldCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		
		StartCoroutine (Display ());

	}
	Canvas canvas;
	
	public Text keyDisplayInfo;
	IEnumerator Display() {
		Tower[] towers = FindObjectsOfType<Tower> ();
		while (towers.Length < 10) {
			towers = FindObjectsOfType<Tower> ();
			yield return null;
		}
		StartCoroutine (KeyDisplay ());
		foreach (Tower t in towers) {
			
		}
	}
	IEnumerator KeyDisplay() {
		keyDisplayInfo.gameObject.SetActive (true);
		float i = 2;
		while (i > .05f) {
			i -= Time.deltaTime *.9f;
			keyDisplayInfo.color = new Color (1, 1, 1, i);
			yield return null;
		}
		keyDisplayInfo.gameObject.SetActive (false);
	}
	void Update() {
		if (Input.GetKeyDown (KeyCode.K)) {
			keyDisplayInfo.gameObject.SetActive (false);
			canvas.enabled = !canvas.isActiveAndEnabled;
		}
	}
}

public static class KeyboardCanvasExtension {
	public static string FormatForKeyboard (this string s){
		if (s.StartsWith ("Alpha")) {
			return s.Remove (0, 5);
		}
		return s;
	}
}
