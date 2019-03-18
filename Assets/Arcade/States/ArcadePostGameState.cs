using UnityEngine;
using System.Collections;

public class ArcadePostGameState : AppState, AppStateOwner {
    public Player victoriousPlayer;
    float enterTime;
    float countdownTime = 10f;

    //AppState

    public AppState state { get; set; }

    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        enterTime = Time.realtimeSinceStartup;

        Object.FindObjectOfType<CameraController>().GameEnded();

        battlefield.localPlayer1.hudText.fadeDuration = 1f;
        battlefield.localPlayer1.hudText.FadeIn();

        if (battlefield.localPlayers.Count == 1) {
            if (battlefield.localPlayer1 == victoriousPlayer) {
                app.PlayAppSoundNamed("Victory");
            }
            else {
                app.PlayAppSoundNamedAtVolume("Defeat", 0.5f);
            }
        }
        else {
            battlefield.localPlayer2.hudText.fadeDuration = 1f;
            battlefield.localPlayer2.hudText.FadeIn();
            app.PlayAppSoundNamed("Victory");
        }

        if (victoriousPlayer.npcModeOn) {
            battlefield.npcLevel = Mathf.Max(1, battlefield.npcLevel - 1);
        }
        else {
            battlefield.npcLevel++;
        }

        App.shared.prefs.npcLevel = battlefield.npcLevel;
    }

    public override void Update() {
        base.Update();

        var winnerText = "VICTORY!";
        var loserText = "DEFEAT!";


        if (victoriousPlayer.fortress.TowersNetDamageRatio() >= 1f) {
            winnerText = "FLAWLESS VICTORY!";
            loserText = "CRUSHING DEFEAT!";
        }

        float timeRemaining = Mathf.Max(0f, Mathf.Ceil(enterTime + countdownTime - Time.realtimeSinceStartup));
        string rematchText = "\n\nPRESS ANY BUTTON TO REMATCH\n\n" + timeRemaining;

        if (battlefield.localPlayers.Count == 1) {
            if (battlefield.localPlayer1 == victoriousPlayer) {
                battlefield.localPlayer1.hudText.SetText(winnerText + rematchText);
            }
            else {
                battlefield.localPlayer1.hudText.SetText(loserText + rematchText);
            }
        }
        else {
            if (battlefield.localPlayer1 == victoriousPlayer) {
                battlefield.localPlayer1.hudText.SetText(winnerText + rematchText);
                battlefield.localPlayer2.hudText.SetText(loserText + rematchText);
            }
            else {
                battlefield.localPlayer1.hudText.SetText(loserText + rematchText);
                battlefield.localPlayer2.hudText.SetText(winnerText + rematchText);
            }
        }

        if (battlefield.player1.arcadeInputs.anyButton.WasPressed || battlefield.player2.arcadeInputs.anyButton.WasPressed) {
            battlefield.localPlayer1.hudText.FadeOut();
            if (battlefield.localPlayers.Count == 2) {
                battlefield.localPlayer2.hudText.FadeOut();
            }

            TransitionTo(new ArcadePlayingGameState());
        }
        else if (timeRemaining <= 0f) {
            TransitionTo(new ArcadeMainMenuState());
        }
    }
}