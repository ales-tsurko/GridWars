using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu()]
public class KeyDataDefaults : ScriptableObject {
    public List<KeyData> keyData = new List<KeyData>() {
		new KeyData(){ code = "Chopper1", description = "Chopper", key = KeyCode.D, playerNum = 1 },
        new KeyData(){ code = "Tanker1", description = "Tanker", key = KeyCode.F, playerNum = 1 },
		new KeyData(){ code = "Tank1", description = "Tank", key = KeyCode.J, playerNum = 1 },
        new KeyData(){ code = "MobileSAM1", description = "Mobile Sam", key = KeyCode.K, playerNum = 1 },
		new KeyData(){ code = Keys.CONCEDE + "1", description = "Concede", key = KeyCode.Q, playerNum = 1 },
		new KeyData(){ code = Keys.TOGGLEKEYS + "1", description = "Toggle Hotkeys", key = KeyCode.H, playerNum = 1 },
		new KeyData(){ code = Keys.CHANGECAM + "1", description = "Change Camera Angle", key = KeyCode.C, playerNum = 1 },
		new KeyData(){ code = Keys.FOCUSMENU + "1", description = "Focus Menu", key = KeyCode.Escape, playerNum = 1 },
		new KeyData(){ code = "Exit", description = "Exit First Person View", key = KeyCode.Escape, playerNum = 1 },

		new KeyData(){ code = "MobileSAM2", description = "Mobile Sam", key = KeyCode.I, playerNum = 2 },
		new KeyData(){ code = "Tank2", description = "Tank", key = KeyCode.U, playerNum = 2 },
		new KeyData(){ code = "Tanker2", description = "Tanker", key = KeyCode.R, playerNum = 2 },
		new KeyData(){ code = "Chopper2", description = "Chopper", key = KeyCode.E, playerNum = 2 },
		new KeyData(){ code = Keys.CONCEDE + "2", description = "Concede", key = KeyCode.P, playerNum = 2 },
		new KeyData(){ code = Keys.TOGGLEKEYS + "2", description = "Toggle Hotkeys", key = KeyCode.G, playerNum = 2 },
		new KeyData(){ code = Keys.CHANGECAM + "2", description = "Change Camera Angle", key = KeyCode.B, playerNum = 2 },
		new KeyData(){ code = Keys.FOCUSMENU + "2", description = "Focus Menu", key = KeyCode.Delete, playerNum = 2 },
    };

    public List<KeyData> joyData = new List<KeyData>() {
		new KeyData(){ code = "Chopper1", description = "Chopper", key = KeyCode.Joystick1Button3, playerNum = 1 },
		new KeyData(){ code = "Tanker1", description = "Tanker", key = KeyCode.Joystick1Button2, playerNum = 1 },
        new KeyData(){ code = "Tank1", description = "Tank", key = KeyCode.Joystick1Button0, playerNum = 1 },
        new KeyData(){ code = "MobileSAM1", description = "Mobile Sam", key = KeyCode.Joystick1Button1, playerNum = 1 },
		new KeyData(){ code = Keys.CONCEDE + "1", description = "Concede", key = KeyCode.Joystick1Button6, playerNum = 1},
		new KeyData(){ code = Keys.TOGGLEKEYS + "1", description = "Toggle Hotkeys", key = KeyCode.Joystick1Button7, playerNum = 1},
		new KeyData(){ code = Keys.CHANGECAM + "1", description = "Change Camera Angle", key = KeyCode.Joystick1Button5, playerNum = 1},
		new KeyData(){ code = Keys.FOCUSMENU + "1", description = "Focus Menu", key = KeyCode.Joystick1Button9, playerNum = 1},
		new KeyData(){ code = "Exit", description = "Exit First Person View", key = KeyCode.Joystick1Button9, playerNum = 1 },

		new KeyData(){ code = "Chopper2", description = "Chopper", key = KeyCode.Joystick2Button3, playerNum = 2 },
		new KeyData(){ code = "Tanker2", description = "Tanker", key = KeyCode.Joystick2Button2, playerNum = 2 },
		new KeyData(){ code = "Tank2", description = "Tank", key = KeyCode.Joystick2Button0, playerNum = 2 },
		new KeyData(){ code = "MobileSAM2", description = "Mobile Sam", key = KeyCode.Joystick2Button1, playerNum = 2 },
		new KeyData(){ code = Keys.CONCEDE + "2", description = "Concede", key = KeyCode.Joystick2Button6, playerNum = 2 },
		new KeyData(){ code = Keys.TOGGLEKEYS + "2", description = "Toggle Hotkeys", key = KeyCode.Joystick2Button7, playerNum = 2 },
		new KeyData(){ code = Keys.CHANGECAM + "2", description = "Change Camera Angle", key = KeyCode.Joystick2Button5, playerNum = 2 },
		new KeyData(){ code = Keys.FOCUSMENU + "2", description = "Focus Menu", key = KeyCode.Joystick2Button9, playerNum = 2},
    };

}
