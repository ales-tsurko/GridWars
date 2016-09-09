using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class Keys {
	
	public static Dictionary<string, KeyCode> data = new Dictionary<string, KeyCode> () {

		{ "Tank1", KeyCode.Alpha1 },
		{ "Tank2", KeyCode.Alpha6 },
		{ "Chopper1", KeyCode.Alpha2},
		{ "Chopper2", KeyCode.Alpha7},
		{ "Jeep1", KeyCode.Alpha3},
		{ "Jeep2", KeyCode.Alpha8},
		{ "MobileSAM1", KeyCode.Alpha4},
		{ "MobileSAM2", KeyCode.Alpha9}

	};
}


