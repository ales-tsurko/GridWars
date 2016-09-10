using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KeyboardCanvas : MonoBehaviour {

	// Use this for initialization
	void Start () {
		canvas = GetComponent<Canvas> ();
		canvas.worldCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		keyboardButton = Resources.Load<GameObject> ("UI/KeyboardButton");
		StartCoroutine (Display ());

	}
	Canvas canvas;
	GameObject keyboardButton;
	public Text keyDisplayInfo;
	IEnumerator Display () {
		Tower[] towers = FindObjectsOfType<Tower> ();
		while (towers.Length < 10) {
			towers = FindObjectsOfType<Tower> ();
			yield return null;
		}
		StartCoroutine (KeyDisplay ());
		foreach (Tower t in towers) {
			if (!t.player.isLocal) {
				continue;
			}
			var button = (GameObject)Instantiate (keyboardButton);
			button.transform.SetParent (transform);
			button.transform.position = t.transform.position + (t.transform.forward * 6) + new Vector3 (0, .05f, 0);
			button.transform.localRotation = Quaternion.Euler (new Vector3 (90, -90, 0));
			button.transform.localScale = Vector3.one * .35f;
			button.transform.GetComponentInChildren<Text> ().text = t.attemptQueueUnitKeyCode.ToString ().FormatForKeyboard();
		}
	}
	IEnumerator KeyDisplay () {
		keyDisplayInfo.gameObject.SetActive (true);
		float i = 2;
		while (i > .05f) {
			i -= Time.deltaTime *.9f;
			keyDisplayInfo.color = new Color (1, 1, 1, i);
			yield return null;
		}
		keyDisplayInfo.gameObject.SetActive (false);
	}
	void Update () {
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
