using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public static class Keys {

    public const string CHANGECAM = "Change Camera View";
    public const string CONCEDE = "Concede Match";
    public const string TOGGLEKEYS = "Toggle Display of Hotkeys";
    public static List<KeyData> keyData;
    public static List<KeyData> joyData;
   
    private static List<KeyData> _defaultKeyData;
    public static List<KeyData> defaultKeyData {
        get {
            if (_defaultKeyData == null) {
                _defaultKeyData = Resources.Load<KeyDataDefaults>("Keys/Defaults").keyData;
            }
            return _defaultKeyData;
        }
    }

    private static List<KeyData> _defaultJoyData;
    public static List<KeyData> defaultJoyData {
        get {
            if (_defaultJoyData == null) {
                _defaultJoyData = Resources.Load<KeyDataDefaults>("Keys/Defaults").joyData;
            }
            return _defaultJoyData;
        }
    }

    public static KeyCode GetKey (this string _string){
        if (keyData == null) {
            LoadKeyMappings();
        }
        foreach (KeyData k in keyData) {
            if (k.code == _string) {
                return k.key;
            }
        }
        return new KeyCode();
    }

    public static KeyCode GetButton (this string _string){
        if (joyData == null) {
            LoadKeyMappings();
        }
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
        //Debug.Log(saveString);
        Prefs.SetKeyMappings(saveString);
    }

    public static void LoadKeyMappings(bool resetToDefault = false) {
        string s = Prefs.GetKeyMappings();
        if (resetToDefault || s == "empty" || string.IsNullOrEmpty(s) || s == "{}") {
            //Debug.Log(s + "  getting defaults");
            keyData = defaultKeyData;
            joyData = defaultJoyData;
            SaveKeyMappings();
        } else {
            KeySaveData data = JsonMapper.ToObject<KeySaveData>(s);
            keyData = data.keyData;
            joyData = data.joyData;
        }
    }

    static UIMenu remapMenu;
    public static int currentRemapPlayerNum;
    public static void OpenRemapKeys(UIMenuItem item){
        if (item != null) {
            currentRemapPlayerNum = Convert.ToInt32(item.itemData);
        }
        if (remapMenu != null) {
            remapMenu.Reset();
        }
        remapMenu = UI.Menu();
        foreach(KeyData k in keyData) {
            if (k.playerNum != currentRemapPlayerNum) {
                continue;
            }
            remapMenu.AddItem(UI.ButtonPrefabKeyMap(k));
        }
        remapMenu.AddItem(UI.ButtonPrefabKeyMap(null, true, "Reset to Defaults"));
        remapMenu.AddItem(UI.ButtonPrefabKeyMap(null, true, "Close"));
        remapMenu.Show();
    }

    public static void RemapKey(UIButtonRemapKey remapKey){
        UIPopup popup = UI.Popup("Press a Key or Joystick Button to map to \n\n" + remapKey.code);
        ReadRemapKeyInput read = popup.gameObject.AddComponent<ReadRemapKeyInput>();
        read.data = remapKey;
    }

    public static void SetDefaults(UIMenuItem item) {
        LoadKeyMappings(true);
        OpenRemapKeys(null);
    }

    public static void CloseMenu(UIMenuItem item) {
        remapMenu.Reset();
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
                Keys.OpenRemapKeys(null);
                Destroy(gameObject);
                return;
            }
        }
    }
}
