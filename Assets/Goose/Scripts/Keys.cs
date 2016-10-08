using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public static class Keys {

    public const string CHANGECAM = "Change Camera View";
    public const string CONCEDE = "Concede Match";
    public const string TOGGLEKEYS = "Toggle Display of Hotkeys";
    public const string EXITFPS = "ExitFPS";
    private static List<KeyData> _keyData;
    public static List<KeyData> keyData {
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

    private static List<KeyData> _joyData;
    public static List<KeyData> joyData {
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
   
    public static List<KeyData> defaultKeyData;
    public static List<KeyData> defaultJoyData;

    public static KeyCode GetKey (this string _string){
        foreach (KeyData k in keyData) {
            if (k.code == _string) {
                return k.key;
            }
        }
        return new KeyCode();
    }

    public static KeyCode GetButton (this string _string){
        foreach (KeyData j in joyData) {
            if (j.code == _string) {
                return j.key;
            }
        }
        return new KeyCode();
    }

    public static bool Pressed (this string _string){
        return (Input.GetKeyDown(_string.GetKey()) || Input.GetKeyDown(_string.GetButton()));
    }
        
    private static void SetNewKey (string _string, KeyCode _keyCode){
        //in future check if k is already mapped and if so, remove - try to make default;
        //Debug.Log (_string + "  "+_keyCode.ToString());
        foreach (KeyData k in keyData) {
            if (k.code == _string) {
                //Debug.Log("Setting");
                k.key = _keyCode;
            }
        }
    }

    private static void SetNewButton (string _string, KeyCode _keyCode){
        //in future check if k is already mapped and if so, remove - try to make default
        foreach (KeyData j in joyData) {
            if (j.code == _string) {
                j.key = _keyCode;
            }
        }
    }

    public static void SetNewInput (string s, KeyCode k){
        if (k.ToString().Contains("Joystick")) {
            SetNewButton(s, k);
        } else {
            SetNewKey(s, k);
        }
        SaveKeyMappings();
    }

    public static void SaveKeyMappings(){
        KeySaveData save = new KeySaveData();
        save.keyData = Keys.keyData;
        save.joyData = Keys.joyData;
        string saveString = JsonMapper.ToJson(save);
        Debug.Log("Keys Saved");
        Prefs.SetKeyMappings(saveString);
    }

    public static void LoadKeyMappings(bool resetToDefault = false) {
        string s = Prefs.GetKeyMappings();
        if (resetToDefault || s == "empty" || string.IsNullOrEmpty(s) || s == "{}") {
            List<KeyData> _defaultKeyData = Resources.Load<KeyDataDefaults>("Keys/Defaults").keyData;
            List<KeyData> _defaultJoyData = Resources.Load<KeyDataDefaults>("Keys/Defaults").joyData;
            keyData = _defaultKeyData;
            joyData = _defaultJoyData;
            SaveKeyMappings();
        } else {
            KeySaveData data = JsonMapper.ToObject<KeySaveData>(s);
            keyData = data.keyData;
            joyData = data.joyData;
        }
    }

    static UIMenu remapMenu;
    public static int currentRemapPlayerNum;

    public static void RemapKey(UIButtonRemapKey remapKey){
		App.shared.ResetMenu();
		var indicator = UI.ActivityIndicator("Press a Key or Joystick Button to map to " + remapKey.code);
		var read = indicator.gameObject.AddComponent<ReadRemapKeyInput>();
		read.data = remapKey;
		App.shared.menu.AddItem(indicator);
		App.shared.menu.Show();
    }

    public static void SetDefaults() {
        LoadKeyMappings(true);
    }

    public static void InitKeyMappings() {
        LoadKeyMappings();
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
                Keys.SetNewInput(data.code, kcode);
                App.shared.state.EnterFrom(null);
                Destroy(gameObject);
                return;
            }
        }
    }
}
