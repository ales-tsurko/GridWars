using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class ArcadeMainMenuState : AppState
{
    public PlayerInputs playerInputs;

    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        CameraController cc = Object.FindObjectOfType<CameraController>();
        cc.MainMenuEntered();

        if (QualitySettings.vSyncCount > 0) {
            QualitySettings.vSyncCount = 2;
        }
        else {
            Application.targetFrameRate = 30;
        }

        app.battlefield.SoftReset();
        network.Reset();

        battlefield.isInternetPVP = false;
        battlefield.isAiVsAi = false;
        battlefield.isPvELadder = false;

        battlefield.player1.npcModeOn = false;
        battlefield.player2.npcModeOn = false;

        battlefield.player1.hudText = GameObject.Find("LeftPlayerText").GetComponent<PlayerText>();
        battlefield.player1.hudText.FadeInWithText("PRESS ANY BUTTON TO PLAY");

        battlefield.player2.hudText = GameObject.Find("RightPlayerText").GetComponent<PlayerText>();
        battlefield.player2.hudText.FadeInWithText("PRESS ANY BUTTON TO PLAY");

        battlefield.player1.inputs = null;
        battlefield.player1.arcadeInputs = new ArcadeInputs();
        battlefield.player1.arcadeInputs.isLeftPlayer = true;
        battlefield.player1.arcadeInputs.AddBindings();

        battlefield.player2.inputs = null;
        battlefield.player2.arcadeInputs = new ArcadeInputs();
        battlefield.player2.arcadeInputs.isLeftPlayer = false;
        battlefield.player2.arcadeInputs.AddBindings();

        /*
        if (_needsInitialFadeIn) {
            menu.backgroundColor = Color.black;
            menu.targetBackgroundColor = Color.clear;
            _needsInitialFadeIn = false;
        }
        */

        App.shared.SoundtrackNamed("MenuBackgroundMusic").Play();

        playerInputs = new PlayerInputs();
    }

    public override void Update() {
        base.Update();

        if (battlefield.player1.arcadeInputs.anyButton.WasPressed) {
            var s = new ArcadeWaitForOpponentState();
            s.readyPlayer = battlefield.player1;
            TransitionTo(s);
        }
        else if (battlefield.player2.arcadeInputs.anyButton.WasPressed) {
            var s = new ArcadeWaitForOpponentState();
            s.readyPlayer = battlefield.player2;
            TransitionTo(s);
        }
    }

    public override void WillExit() {
        base.WillExit();

        if (QualitySettings.vSyncCount > 0) {
            QualitySettings.vSyncCount = 1;
        }
        else {
            Application.targetFrameRate = 60;
        }
    }

    void Quit() {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
