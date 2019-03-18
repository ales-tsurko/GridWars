using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ArcadeWaitForOpponentState: AppState {
    public Player readyPlayer;
    float enterTime;
    float waitTime = 5;

    Player otherPlayer {
        get {
            return readyPlayer == battlefield.player1 ? battlefield.player2 : battlefield.player1;
        }
    }

    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        enterTime = Time.realtimeSinceStartup;
    }

    public override void Update() {
        base.Update();

        float timeRemaining = Mathf.Max(0f, Mathf.Ceil(waitTime - (Time.realtimeSinceStartup - enterTime)));

        readyPlayer.hudText.SetText("WAITING FOR OPPONENT\n\n" + timeRemaining.ToString());

        otherPlayer.hudText.SetText("PRESS ANY BUTTON TO PLAY\n\n" + timeRemaining.ToString());

        if (otherPlayer.arcadeInputs.anyButton.WasPressed) {
            StartTwoPlayer();
        }
        else if (timeRemaining <= 0f) {
            StartSinglePlayer();
        }
    }

    void StartSinglePlayer() {
        readyPlayer.hudText.FadeOut();
        otherPlayer.hudText.FadeOut();

        readyPlayer.isLocal = true;
        readyPlayer.npcModeOn = false;

        otherPlayer.isLocal = false;
        otherPlayer.npcModeOn = true;

        battlefield.player1.isTutorialMode = false;
        battlefield.player2.isTutorialMode = false;
        battlefield.npcLevel = App.shared.prefs.npcLevel;
        App.shared.prefs.npcLevel = Mathf.Max(1, battlefield.npcLevel - 1); //player only gets credit for a win if they finish the game.

        TransitionTo(new ArcadeWaitForBoltState());
    }

    void StartTwoPlayer() {
        readyPlayer.hudText.FadeOut();
        otherPlayer.hudText.FadeOut();

        battlefield.player1.isLocal = true;
        battlefield.player2.isLocal = true;

        battlefield.player2.npcModeOn = false;
        battlefield.player2.npcModeOn = false;

        battlefield.player1.isTutorialMode = false;
        battlefield.player2.isTutorialMode = false;

        /*Analytics.CustomEvent("SharedScreenPvPClicked", new Dictionary<string, object>
            {
                { "playTime", Time.timeSinceLevelLoad }
            });*/
        TransitionTo(new ArcadeWaitForBoltState());
    }
}
