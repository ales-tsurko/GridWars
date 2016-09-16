using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class Keys {

    public const string CHANGECAM = "Change Camera View";
    public const string CONCEDE = "Concede Match";
    public const string TOGGLEKEYS = "Toggle Display of Hotkeys";


    public static KeyCode GetKey (this string _string){
        KeyCode code;
        data.TryGetValue(_string, out code);
        return code;
    }
	public static Dictionary<string, KeyCode> data = new Dictionary<string, KeyCode> () {

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
}


