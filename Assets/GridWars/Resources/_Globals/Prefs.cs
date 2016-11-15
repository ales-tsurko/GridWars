using UnityEngine;
using System.Collections;

public class Prefs {
	public static string PrefsKeyIconsVisibleChangedNotification = "PrefsKeyIconsVisibleChangedNotification";

	public bool keyIconsVisible {
		get {
            return GetBool("keyIconsVisible");
		}

		set {
			SetBool("keyIconsVisible", value);
			App.shared.notificationCenter.NewNotification()
				.SetName(PrefsKeyIconsVisibleChangedNotification)
				.SetSender(this)
				.Post();
		}
	}

	public string screenName {
		get {
			return PlayerPrefs.GetString("screenName");
		}

		set {
			PlayerPrefs.SetString("screenName", value);
		}
	}

	public string email {
		get {
			return PlayerPrefs.GetString("email");
		}

		set {
			PlayerPrefs.SetString("email", value);
		}
	}

	public string accessToken {
		get {
			return PlayerPrefs.GetString("accessToken");
		}

		set {
			PlayerPrefs.SetString("accessToken", value);
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
   
    public int camPosition {
        get {
            return PlayerPrefs.GetInt("camPosition", 0);
        }

        set {
            PlayerPrefs.SetInt("camPosition", value);
        }
    }

    public static string GetKeyMappings () {
        return PlayerPrefs.GetString("keyMappings", "empty");
    }

    public static void SetKeyMappings (string s) {
        PlayerPrefs.SetString("keyMappings", s);

    }

	bool GetBool(string name) {
		return PlayerPrefs.GetInt(name) == 1;
	}

	void SetBool(string name, bool value) {
		PlayerPrefs.SetInt(name, value ? 1 : 0);
	}
}
