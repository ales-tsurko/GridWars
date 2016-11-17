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
            return GetString("screenName");
		}

		set {
            SetString("screenName", value);
		}
	}

	public string accessToken {
		get {
            return GetString("accessToken");
		}

		set {
            SetString("accessToken", value);
		}
	}

	public bool hasPlayedTutorial {
		get {
			return GetBool("hasPlayedTutorial");
		}

		set {
			SetBool("hasPlayedTutorial", value);
		}
	}

	public float npcHandicap {
		get {
			if (HasPref("npcHandicap")) {
				return GetFloat("npcHandicap");
			}
			else {
				return 1.0f;
			}
		}

		set {
			SetFloat("npcHandicap", value);
		}
	}
   
    public int camPosition {
        get {
            return GetInt("camPosition");
        }

        set {
            SetInt("camPosition", value);
        }
    }

    public Resolution GetResolution (){
		return new Resolution(){ height = GetInt("ResolutionWidth"), width = GetInt("ResolutionHeight") };
    }

    public void SetResolution(Resolution res){
        SetInt("ResolutionWidth", res.width);
        SetInt("ResolutionHeight", res.height);
    }

    public int GetAA(){
        return GetInt("Antialiasing");
    }

    public void SetAA(int aa){
        SetInt("Antialiasing", aa);
    }

	string PrefixedKey(string key) {
		return App.shared.config.prefsPrefix + "/" + key;
	}

	bool HasPref(string key) {
		return PlayerPrefs.HasKey(PrefixedKey(key));
	}

	string GetString(string key) {
		return PlayerPrefs.GetString(PrefixedKey(key));
	}

	void SetString(string key, string value) {
		PlayerPrefs.SetString(PrefixedKey(key), value);
		PlayerPrefs.Save();
	}

	int GetInt(string key) {
		return PlayerPrefs.GetInt(PrefixedKey(key));
	}

	void SetInt(string key, int value) {
		PlayerPrefs.SetInt(PrefixedKey(key), value);
		PlayerPrefs.Save();
	}

	float GetFloat(string key) {
		return PlayerPrefs.GetFloat(PrefixedKey(key));
	}

	void SetFloat(string key, float value) {
		PlayerPrefs.SetFloat(PrefixedKey(key), value);
		PlayerPrefs.Save();
	}

	bool GetBool(string key) {
		return PlayerPrefs.GetInt(PrefixedKey(key)) == 1;
	}

	void SetBool(string key, bool value) {
		PlayerPrefs.SetInt(PrefixedKey(key), value ? 1 : 0);
		PlayerPrefs.Save();
	}
}
