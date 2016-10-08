using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu()]
public class KeyDataDefaults : ScriptableObject {
    public List<KeyData> keyData = new List<KeyData>() {
		new KeyData(){code = "Chopper1", description = "Player 1 Chopper", key = KeyCode.D, playerNum = 1},
        new KeyData(){code = "Tanker1", description = "Player 1 Tanker", key = KeyCode.F, playerNum = 1},
		new KeyData(){code = "Tank1", description = "Player 1 Tank", key = KeyCode.J, playerNum = 1},
        new KeyData(){code = "MobileSAM1", description = "Player 1 Mobile Sam", key = KeyCode.K, playerNum = 1},
        new KeyData(){code = Keys.CONCEDE, description = "Concede", key = KeyCode.Q, playerNum = 1},
        new KeyData(){code = Keys.TOGGLEKEYS, description = "Toggle Hotkeys", key = KeyCode.H, playerNum = 1},
        new KeyData(){code = Keys.CHANGECAM, description = "Change Camera Angle", key = KeyCode.C, playerNum = 1},
		new KeyData(){code = "MobileSAM2", description = "Player 2 Mobile Sam", key = KeyCode.E, playerNum = 2},
		new KeyData(){code = "Tank2", description = "Player 2 Tank", key = KeyCode.R, playerNum = 2},
		new KeyData(){code = "Tanker2", description = "Player 2 Tanker", key = KeyCode.U, playerNum = 2},
		new KeyData(){code = "Chopper2", description = "Player 2 Chopper", key = KeyCode.I, playerNum = 2},
        new KeyData(){code = "ExitFPS", description = "Exit First Person View", key = KeyCode.X, playerNum = 1}
    };

    public List<KeyData> joyData = new List<KeyData>() {
		new KeyData(){ code = "Chopper1", description = "Player 1 Chopper", key = KeyCode.Joystick2Button0, playerNum = 1 },
		new KeyData(){ code = "Tanker1", description = "Player 1 Tanker", key = KeyCode.Joystick1Button1, playerNum = 1 },
        new KeyData(){ code = "Tank1", description = "Player 1 Tank", key = KeyCode.Joystick1Button2, playerNum = 1 },
        new KeyData(){ code = "MobileSAM1", description = "Player 1 Mobile Sam", key = KeyCode.Joystick1Button3, playerNum = 1 },
        new KeyData(){ code = Keys.CONCEDE, description = "Concede", key = KeyCode.Joystick2Button4, playerNum = 1 },
        new KeyData(){ code = Keys.TOGGLEKEYS, description = "Toggle Hotkeys", key = KeyCode.Joystick2Button5, playerNum = 1 },
        new KeyData(){ code = Keys.CHANGECAM, description = "Change Camera Angle", key = KeyCode.Joystick2Button6, playerNum = 1 },
		new KeyData(){ code = "MobileSAM2", description = "Player 2 Mobile Sam", key = KeyCode.Joystick2Button3, playerNum = 2 },
        new KeyData(){ code = "Tank2", description = "Player 2 Tank", key = KeyCode.Joystick2Button2, playerNum = 2 },
		new KeyData(){ code = "Tanker2", description = "Player 2 Tanker", key = KeyCode.Joystick2Button1, playerNum = 2 },
        new KeyData(){ code = "Chopper2", description = "Player 2 Chopper", key = KeyCode.Joystick2Button0, playerNum = 2 },
        new KeyData(){ code = "ExitFPS", description = "Exit First Person View", key = KeyCode.Joystick1Button13, playerNum = 1}
    };

}
