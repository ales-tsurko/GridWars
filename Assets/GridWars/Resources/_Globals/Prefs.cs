using UnityEngine;
using System.Collections;

public class Prefs {
	public static string PrefsKeyIconsVisibleChangedNotification = "PrefsKeyIconsVisibleChangedNotification";
    public static string prefix = "";
	
    public bool keyIconsVisible {
		get {
            return GetBool(prefix + "keyIconsVisible");
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
            return PlayerPrefs.GetString(prefix + "screenName");
		}

		set {
            PlayerPrefs.SetString(prefix + "screenName", value);
		}
	}

	public string accessToken {
		get {
            return PlayerPrefs.GetString(prefix + "accessToken");
		}

		set {
            PlayerPrefs.SetString(prefix + "accessToken", value);
		}
	}

	public SerializedTransform cameraPosition {
		get {
            var json = PlayerPrefs.GetString(prefix + "cameraPosition");
			if (json == null) {
				return null;
			}
			else {
				return JsonUtility.FromJson<SerializedTransform>(json);
			}
		}

		set {
            PlayerPrefs.SetString(prefix + "cameraPosition", JsonUtility.ToJson(value));
		}
	}
   
    public int camPosition {
        get {
            return PlayerPrefs.GetInt(prefix + "camPosition", 0);
        }

        set {
            PlayerPrefs.SetInt(prefix + "camPosition", value);
        }
    }

    public static string GetKeyMappings () {
        return PlayerPrefs.GetString(prefix + "keyMappings", "empty");
    }

    public static void SetKeyMappings (string s) {
        PlayerPrefs.SetString(prefix + "keyMappings", s);

    }

    public Resolution GetResolution (){
        int _width = PlayerPrefs.GetInt(prefix + "ResolutionWidth", 0);
        int _height = PlayerPrefs.GetInt(prefix + "ResolutionHeight", 0);
        return new Resolution(){ height = _height, width = _width };
    }
    public void SetResolution(Resolution res){
        PlayerPrefs.SetInt(prefix + "ResolutionWidth", res.width);
        PlayerPrefs.SetInt(prefix + "ResolutionHeight", res.height);
    }

    public int GetAA(){
        return PlayerPrefs.GetInt(prefix + "Antialiasing", 0);
    }
    public void SetAA(int aa){
        PlayerPrefs.SetInt(prefix + "Antialiasing", aa);
    }

	bool GetBool(string name) {
        return PlayerPrefs.GetInt(prefix + name) == 1;
	}

	void SetBool(string name, bool value) {
        PlayerPrefs.SetInt(prefix + name, value ? 1 : 0);
	}
}
