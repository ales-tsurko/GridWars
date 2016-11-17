using UnityEngine;
using System.Collections;

[System.Serializable]
public class Prefs {
	public static string PrefsChangedNotification = "PrefsChangedNotification";

	public Prefs() {
		_keyIconsVisible = true;
		_screenName = "";
		_accessToken = "";
		_hasPlayedTutorial = false;
		_npcHandicap = 1f;
		_cameraPosition = "MainBackView";

		//will be corrected in App.SetupResolution
		_resolutionHeight = -1;
		_resolutionWidth = -1;
		_antialiasingLevel = 4;
	}

	public bool _keyIconsVisible;
	public bool keyIconsVisible {
		get {
			return _keyIconsVisible;
		}

		set {
			_keyIconsVisible = value;
			PostNotification("keyIconsVisible");
			Save();
		}
	}

	public string _screenName;
	public string screenName {
		get {
			return _screenName;
		}

		set {
			_screenName = value;
			PostNotification("screenName");
			Save();
		}
	}

	public string _accessToken;
	public string accessToken {
		get {
			return _accessToken;
		}

		set {
			_accessToken = value;
			PostNotification("accessToken");
			Save();
		}
	}

	public bool _hasPlayedTutorial;
	public bool hasPlayedTutorial {
		get {
			return _hasPlayedTutorial;
		}

		set {
			_hasPlayedTutorial = value;
			PostNotification("hasPlayedTutorial");
			Save();
		}
	}

	public float _npcHandicap;
	public float npcHandicap {
		get {
			return _npcHandicap;
		}

		set {
			_npcHandicap = value;
			PostNotification("npcHandicap");
			Save();
		}
	}

	public string _cameraPosition;
	public string cameraPosition {
		get {
			return _cameraPosition;
		}

		set {
			_cameraPosition = value;
			PostNotification("cameraPosition");
			Save();
		}
	}

	public int _resolutionWidth;
	public int resolutionWidth {
		get {
			return _resolutionWidth;
		}

		set {
			_resolutionWidth = value;
			PostNotification("resolutionWidth");
			Save();
		}
	}

	public int _resolutionHeight;
	public int resolutionHeight {
		get {
			return _resolutionHeight;
		}

		set {
			_resolutionHeight = value;
			PostNotification("resolutionHeight");
			Save();
		}
	}

	public int _antialiasingLevel;
	public int antialiasingLevel {
		get {
			return _antialiasingLevel;
		}

		set {
			_antialiasingLevel = value;
			PostNotification("antialiasingLevel");
			Save();
		}
	}

	public Resolution resolution {
		get {
			return new Resolution(){ height = resolutionHeight, width = resolutionWidth };
		}

		set {
			resolutionWidth = value.width;
			resolutionHeight = value.height;
		}
	}


	void PostNotification(string name) {
		if (App.shared.notificationCenter == null) {
			return;
		}

		App.shared.notificationCenter.NewNotification()
			.SetName(PrefsChangedNotification)
			.SetData(name)
			.SetSender(this)
			.Post();
	}

	string environmentName {
		get {
			if (Application.isEditor) {
				return "Editor";
			}
			else if (Debug.isDebugBuild) {
				return "Debug";
			}
			else {
				return "Release";
			}
		}
	}

	string envPath {
		get {
			return System.IO.Path.Combine(
				System.IO.Path.Combine(Application.persistentDataPath, "Environment"),
				environmentName
			);
		}
	}

	string path {
		get {
			return System.IO.Path.Combine(envPath, "Prefs.json");
		}
	}

	public void Load() {
		if (System.IO.File.Exists(path)) {
			JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), this);
		}
	}

	public void Save() {
		if (!System.IO.Directory.Exists(envPath)) {
			System.IO.Directory.CreateDirectory(envPath);
		}

		System.IO.File.WriteAllText(path, JsonUtility.ToJson(this));
	}
}