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
            releaseMobileSam.AddDefaultBinding(InputControlType.Action1);
            releaseTanker.AddDefaultBinding(InputControlType.Action2);
            releaseTank.AddDefaultBinding(InputControlType.Action3);
            releaseChopper.AddDefaultBinding(InputControlType.Action4);
            releaseMobileSam.AddDefaultBinding(Key.Q);
            releaseTanker.AddDefaultBinding(Key.W);
            releaseTank.AddDefaultBinding(Key.E);
            releaseChopper.AddDefaultBinding(Key.R);

            anyButton.AddDefaultBinding(InputControlType.Action1);
            anyButton.AddDefaultBinding(InputControlType.Action2);
            anyButton.AddDefaultBinding(InputControlType.Action3);
            anyButton.AddDefaultBinding(InputControlType.Action4);
            anyButton.AddDefaultBinding(Key.Q);
            anyButton.AddDefaultBinding(Key.W);
            anyButton.AddDefaultBinding(Key.E);
            anyButton.AddDefaultBinding(Key.R);
        }
        else {
            releaseMobileSam.AddDefaultBinding(InputControlType.Action5);
            releaseTanker.AddDefaultBinding(InputControlType.Action6);
            releaseTank.AddDefaultBinding(InputControlType.Action7);
            releaseChopper.AddDefaultBinding(InputControlType.Action8);
            releaseMobileSam.AddDefaultBinding(Key.U);
            releaseTanker.AddDefaultBinding(Key.I);
            releaseTank.AddDefaultBinding(Key.O);
            releaseChopper.AddDefaultBinding(Key.P);

            anyButton.AddDefaultBinding(InputControlType.Action5);
            anyButton.AddDefaultBinding(InputControlType.Action6);
            anyButton.AddDefaultBinding(InputControlType.Action7);
            anyButton.AddDefaultBinding(InputControlType.Action8);
            anyButton.AddDefaultBinding(Key.U);
            anyButton.AddDefaultBinding(Key.I);
            anyButton.AddDefaultBinding(Key.O);
            anyButton.AddDefaultBinding(Key.P);
        }
    }
}