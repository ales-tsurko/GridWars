using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class Keys {

    public const string CHANGECAM = "Change Camera View";
    public const string CONCEDE = "Concede Match";
    public const string TOGGLEKEYS = "Toggle Display of Hotkeys";
    public const string JOYSTICKONE = "Joystick Button 1";
    public static bool JoystickAttached (int player) {
        #if !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX
        PlayerIndex singlePlayer = (PlayerIndex)0;
        GamePadState testState = GamePad.GetState(singlePlayer);
        return testState.IsConnected;
        #endif
        return false;
    }

    /*
     foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
         {
                          //save for later
         }
         */

    public static KeyCode GetKey (this string _string){
        KeyCode code;
        keyData.TryGetValue(_string, out code);
        return code;
    }
    public static KeyCode GetButton (this string _string){
        KeyCode code;
        joyData.TryGetValue(_string, out code);
        return code;
    }

    public static bool Pressed (this string _string){
        return (Input.GetKeyDown(_string.GetKey()) || Input.GetKeyDown(_string.GetButton()));
    }

	public static Dictionary<string, KeyCode> keyData = new Dictionary<string, KeyCode> () {

		{ "Tank1", KeyCode.F },
		{ "Tank2", KeyCode.R },
		{ "Chopper1", KeyCode.J },
		{ "Chopper2", KeyCode.U },
		{ "Tanker1", KeyCode.D },
		{ "Tanker2", KeyCode.E },
		{ "MobileSAM1", KeyCode.K },
		{ "MobileSAM2", KeyCode.I },
        { CONCEDE, KeyCode.Escape},
        { TOGGLEKEYS, KeyCode.H},
        { CHANGECAM, KeyCode.C}

	};

    public static Dictionary<string, KeyCode> joyData = new Dictionary<string, KeyCode> () {

        { "Tank1", KeyCode.Joystick1Button0 },
        { "Tank2", KeyCode.Joystick2Button0 },
        { "Chopper1", KeyCode.Joystick1Button1 },
        { "Chopper2", KeyCode.Joystick2Button1 },
        { "Tanker1", KeyCode.Joystick1Button2 },
        { "Tanker2", KeyCode.Joystick2Button2 },
        { "MobileSAM1", KeyCode.Joystick1Button3 },
        { "MobileSAM2", KeyCode.Joystick2Button3 },
        { CONCEDE, KeyCode.Joystick2Button4},
        { TOGGLEKEYS, KeyCode.Joystick2Button5},
        { CHANGECAM, KeyCode.Joystick2Button6}

    };
}


