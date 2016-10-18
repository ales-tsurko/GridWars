using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public class Keys {

    public const string CHANGECAM = "Camera";
    public const string CONCEDE = "Concede";
    public const string TOGGLEKEYS = "Hotkeys";
	public const string FOCUSMENU = "FocusMenu";
    public const string BACK = "Back";

	public Keys() {
		keyDelegateMap = new Dictionary<string, List<KeyDelegate>>();
		keyDownTimeMap = new Dictionary<string, float>();
	}

    private List<KeyData> _keyData;
    public List<KeyData> keyData {
        get {
            if (_keyData == null) {
                LoadKeyMappings();
            }
            return _keyData;
        }
        set {
            _keyData = new List<KeyData>();
            foreach (KeyData k in value) {
                _keyData.Add(k);
            }
        }
    }

    private List<KeyData> _joyData;
    public List<KeyData> joyData {
        get {
            if (_joyData == null) {
                LoadKeyMappings();
            }
            return _joyData;
        }
        set {
            _joyData = new List<KeyData>();
            foreach (KeyData k in value) {
                _joyData.Add(k);
            }
        }
    }
   
    public List<KeyData> defaultKeyData;
    public List<KeyData> defaultJoyData;
        
    private void SetNewKey (string _string, KeyCode _keyCode){
        //in future check if k is already mapped and if so, remove - try to make default;
        //Debug.Log (_string + "  "+_keyCode.ToString());
        foreach (KeyData k in keyData) {
            if (k.code == _string) {
                //Debug.Log("Setting");
                k.key = _keyCode;
            }
        }
    }

    private void SetNewButton (string _string, KeyCode _keyCode){
        //in future check if k is already mapped and if so, remove - try to make default
        foreach (KeyData j in joyData) {
            if (j.code == _string) {
                j.key = _keyCode;
            }
        }
    }

    public void SetNewInput (string s, KeyCode k){
        if (k.ToString().Contains("Joystick")) {
            SetNewButton(s, k);
        } else {
            SetNewKey(s, k);
        }
        SaveKeyMappings();
    }

    public void SaveKeyMappings(){
        KeySaveData save = new KeySaveData();
        save.keyData = keyData;
        save.joyData = joyData;
        string saveString = JsonMapper.ToJson(save);
        Debug.Log("Keys Saved");
        Prefs.SetKeyMappings(saveString);
    }

    public void LoadKeyMappings(bool resetToDefault = false) {
        string s = Prefs.GetKeyMappings();
        if (resetToDefault || s == "empty" || string.IsNullOrEmpty(s) || s == "{}") {
			List<KeyData> _defaultKeyData = new KeyDataDefaults().keyData;
			List<KeyData> _defaultJoyData = new KeyDataDefaults().joyData;
            keyData = _defaultKeyData;
            joyData = _defaultJoyData;
            SaveKeyMappings();
        } else {
            KeySaveData data = JsonMapper.ToObject<KeySaveData>(s);
            keyData = data.keyData;
            joyData = data.joyData;
        }
    }

    UIMenu remapMenu;
    public int currentRemapPlayerNum;

    public void RemapKey(UIButtonRemapKey remapKey){
		App.shared.ResetMenu();
		var indicator = UI.ActivityIndicator("Press a Key or Joystick Button to map to " + remapKey.code);
		var read = indicator.gameObject.AddComponent<ReadRemapKeyInput>();
		read.data = remapKey;
		App.shared.menu.AddItem(indicator);
		App.shared.menu.Show();
    }

    public void SetDefaults() {
        LoadKeyMappings(true);
    }

    public void InitKeyMappings() {
        LoadKeyMappings();
    }

	//KeyDelegate

	Dictionary<string, List<KeyDelegate>> keyDelegateMap;
	Dictionary<string, float> keyDownTimeMap;
	public float longPressDuration = 1f;

	List<KeyDelegate> KeyDelegateList(string keyName) {
		List<KeyDelegate> list;

		if (!keyDelegateMap.ContainsKey(keyName)) {
			list = new List<KeyDelegate>();
			keyDelegateMap[keyName] = list;
		}
		else {
			list = keyDelegateMap[keyName];
		}

		return list;
	}

	public void AddKeyDelegate(string keyName, KeyDelegate keyDelegate) {
		var list = KeyDelegateList(keyName);
		if (!list.Contains(keyDelegate)) {
			//App.shared.Log("AddKeyDelegate: " + keyName, keyDelegate);
			list.Add(keyDelegate);
		}
	}

	public void RemoveKeyDelegate(string keyName, KeyDelegate keyDelegate) {
		//App.shared.Log("RemoveKeyDelegate: " + keyName, keyDelegate);
		KeyDelegateList(keyName).Remove(keyDelegate);
	}

	public void SoftReset() {
		//App.shared.Log("SoftReset", this);
		keyDelegateMap.Clear();
		keyDownTimeMap.Clear();
	}

	public void Update() {
		foreach (var keyName in keyDelegateMap.Keys) {
			if (keyName.KeyDown()) {
				keyDownTimeMap[keyName] = Time.time;
			}

			if (keyDownTimeMap.ContainsKey(keyName)) {
				if (Time.time - keyDownTimeMap[keyName] >= longPressDuration) {
					foreach (var keyDelegate in new List<KeyDelegate>(KeyDelegateList(keyName))) { //copy in case its modified
						keyDelegate.KeyLongPressed();
					}
					keyDownTimeMap.Remove(keyName);
				}
				else if (keyName.KeyUp()) {
					foreach (var keyDelegate in new List<KeyDelegate>(KeyDelegateList(keyName))) { //copy in case its modified
						keyDelegate.KeyPressed();
					}
					keyDownTimeMap.Remove(keyName);
				}
			}
		}
	}

    [System.Serializable]
    public class KeySaveData {
        public List<KeyData> keyData;
        public List<KeyData> joyData;
    }
}

[System.Serializable]
public class KeyData {
    public string code;
    public KeyCode key;
    public string description;
    public int playerNum;
}

public class ReadRemapKeyInput : MonoBehaviour {
    public UIButtonRemapKey data;
    void Update() {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
            if (kcode.ToString().Contains("Mouse") || kcode.ToString().Contains("JoystickB")) {
                continue;
            }
            if (Input.GetKeyDown(kcode)) {
				App.shared.keys.SetNewInput(data.code, kcode);
                App.shared.state.EnterFrom(null);
                Destroy(gameObject);
                return;
            }
        }
    }
}

public static class KeyCodeExtension {
	public static KeyCode GetKeyCode (this string _string){
		foreach (KeyData k in App.shared.keys.keyData) {
			if (k.code == _string) {
				return k.key;
			}
		}
		return new KeyCode();
	}

	public static KeyCode GetButton (this string _string){
		foreach (KeyData j in App.shared.keys.joyData) {
			if (j.code == _string) {
				return j.key;
			}
		}
		return new KeyCode();
	}

	public static bool KeyDown (this string _string){
		return (Input.GetKeyDown(_string.GetKeyCode()) || Input.GetKeyDown(_string.GetButton()));
	}

	public static bool KeyUp (this string _string){
		return (Input.GetKeyUp(_string.GetKeyCode()) || Input.GetKeyUp(_string.GetButton()));
	}
}

public interface KeyDelegate {
	void KeyPressed();
	void KeyLongPressed();
}