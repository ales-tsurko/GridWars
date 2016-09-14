using UnityEngine;
using System.Collections;

public class Prefs {
	public bool keyIconsVisible {
		get {
			return GetBool("keyIconsVisible");
		}

		set {
			SetBool("keyIconsVisible", value);
		}
	}

	public SerializedTransform cameraPosition {
		get {
			var json = PlayerPrefs.GetString("cameraPosition");
			if (json == null) {
				return null;
			}
			else {
				return JsonUtility.FromJson<SerializedTransform>(json);
			}
		}

		set {
			PlayerPrefs.SetString("cameraPosition", JsonUtility.ToJson(value));
		}
	}

	public Prefs() {
		keyIconsVisible = true; //TODO: the UI should control this.
	}

	bool GetBool(string name) {
		return PlayerPrefs.GetInt(name) == 1;
	}

	void SetBool(string name, bool value) {
		PlayerPrefs.SetInt(name, value ? 1 : 0);
	}
}
