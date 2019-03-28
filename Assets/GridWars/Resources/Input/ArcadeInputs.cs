using InControl;
using UnityEngine;
using System.Collections.Generic;

public class ArcadeInputs : PlayerActionSet {
    public PlayerAction releaseChopper;
    public PlayerAction releaseTanker;
    public PlayerAction releaseTank;
    public PlayerAction releaseMobileSam;
    public PlayerAction anyButton;
    public Player player;

    public ArcadeInputs() {
        releaseChopper = CreatePlayerAction("Chopper");
        releaseTanker = CreatePlayerAction("Tanker");
        releaseTank = CreatePlayerAction("Tank");
        releaseMobileSam = CreatePlayerAction("MobileSAM");
        anyButton = CreatePlayerAction("AnyButton");
    }

    public void AddBindings() {
        if (player.playerNumber == 1) {
            releaseChopper.AddDefaultBinding(InputControlType.Button4);
            releaseTanker.AddDefaultBinding(InputControlType.Button5);
            releaseTank.AddDefaultBinding(InputControlType.Button6);
            releaseMobileSam.AddDefaultBinding(InputControlType.Button7);
            anyButton.AddDefaultBinding(InputControlType.Button4);
            anyButton.AddDefaultBinding(InputControlType.Button5);
            anyButton.AddDefaultBinding(InputControlType.Button6);
            anyButton.AddDefaultBinding(InputControlType.Button7);

            releaseChopper.AddDefaultBinding(Key.Q);
            releaseTanker.AddDefaultBinding(Key.W);
            releaseTank.AddDefaultBinding(Key.E);
            releaseMobileSam.AddDefaultBinding(Key.R);
            anyButton.AddDefaultBinding(Key.Q);
            anyButton.AddDefaultBinding(Key.W);
            anyButton.AddDefaultBinding(Key.E);
            anyButton.AddDefaultBinding(Key.R);
        }
        else {
            releaseChopper.AddDefaultBinding(InputControlType.Button0);
            releaseTanker.AddDefaultBinding(InputControlType.Button1);
            releaseTank.AddDefaultBinding(InputControlType.Button2);
            releaseMobileSam.AddDefaultBinding(InputControlType.Button3);
            anyButton.AddDefaultBinding(InputControlType.Button0);
            anyButton.AddDefaultBinding(InputControlType.Button1);
            anyButton.AddDefaultBinding(InputControlType.Button2);
            anyButton.AddDefaultBinding(InputControlType.Button3);

            releaseChopper.AddDefaultBinding(Key.U);
            releaseTanker.AddDefaultBinding(Key.I);
            releaseTank.AddDefaultBinding(Key.O);
            releaseMobileSam.AddDefaultBinding(Key.P);
            anyButton.AddDefaultBinding(Key.U);
            anyButton.AddDefaultBinding(Key.I);
            anyButton.AddDefaultBinding(Key.O);
            anyButton.AddDefaultBinding(Key.P);
        }
    }
}