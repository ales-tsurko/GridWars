using InControl;
using UnityEngine;
using System.Collections.Generic;

public class ArcadeInputs : PlayerActionSet {
    public PlayerAction releaseChopper;
    public PlayerAction releaseTanker;
    public PlayerAction releaseTank;
    public PlayerAction releaseMobileSam;
    public PlayerAction anyButton;
    public bool isLeftPlayer;

    public ArcadeInputs() {
        releaseChopper = CreatePlayerAction("Chopper");
        releaseTanker = CreatePlayerAction("Tanker");
        releaseTank = CreatePlayerAction("Tank");
        releaseMobileSam = CreatePlayerAction("MobileSAM");
        anyButton = CreatePlayerAction("AnyButton");
    }

    public void AddBindings() {
        if (isLeftPlayer) {
            releaseMobileSam.AddDefaultBinding(InputControlType.Button4);
            releaseTanker.AddDefaultBinding(InputControlType.Button5);
            releaseTank.AddDefaultBinding(InputControlType.Button6);
            releaseChopper.AddDefaultBinding(InputControlType.Button7);
            releaseMobileSam.AddDefaultBinding(Key.Q);
            releaseTanker.AddDefaultBinding(Key.W);
            releaseTank.AddDefaultBinding(Key.E);
            releaseChopper.AddDefaultBinding(Key.R);

            anyButton.AddDefaultBinding(InputControlType.Button4);
            anyButton.AddDefaultBinding(InputControlType.Button5);
            anyButton.AddDefaultBinding(InputControlType.Button6);
            anyButton.AddDefaultBinding(InputControlType.Button7);
            anyButton.AddDefaultBinding(Key.Q);
            anyButton.AddDefaultBinding(Key.W);
            anyButton.AddDefaultBinding(Key.E);
            anyButton.AddDefaultBinding(Key.R);
        }
        else {
            releaseMobileSam.AddDefaultBinding(InputControlType.Button0);
            releaseTanker.AddDefaultBinding(InputControlType.Button1);
            releaseTank.AddDefaultBinding(InputControlType.Button2);
            releaseChopper.AddDefaultBinding(InputControlType.Button3);
            releaseMobileSam.AddDefaultBinding(Key.U);
            releaseTanker.AddDefaultBinding(Key.I);
            releaseTank.AddDefaultBinding(Key.O);
            releaseChopper.AddDefaultBinding(Key.P);

            anyButton.AddDefaultBinding(InputControlType.Button0);
            anyButton.AddDefaultBinding(InputControlType.Button1);
            anyButton.AddDefaultBinding(InputControlType.Button2);
            anyButton.AddDefaultBinding(InputControlType.Button3);
            anyButton.AddDefaultBinding(Key.U);
            anyButton.AddDefaultBinding(Key.I);
            anyButton.AddDefaultBinding(Key.O);
            anyButton.AddDefaultBinding(Key.P);
        }
    }
}