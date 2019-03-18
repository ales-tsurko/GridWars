using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class ArcadePlayingGameState : AppState {
    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        Object.FindObjectOfType<CameraController>().GameStarted();

        battlefield.canCheckGameOver = false;

        if (app.battlefield.isPvsAI()) {
            app.battlefield.localPlayer1.hudText.fadeDuration = 5f;
            app.battlefield.localPlayer1.hudText.FadeOutWithText("Level " + battlefield.npcLevel + " (NPC POWER " + string.Format("{0:F2}", battlefield.localPlayer1.opponent.npcHandicap) + ")");
        }

        App.shared.SoundtrackNamed("MenuBackgroundMusic").FadeOut();

        App.shared.PlayAppSoundNamed("GameStart");

        battlefield.SoftReset();

        app.StartCoroutine(StartGameAfterBattlefieldEmpty());
        app.account.LogEvent("PlayingGame");
    }

    IEnumerator StartGameAfterBattlefieldEmpty() {
        while (BoltNetwork.isServer && !battlefield.isEmpty) {
            yield return null;
        }

        battlefield.StartGame();
    }

    public override void WillExit() {
        base.WillExit();
    }

    public override void Update() {
        base.Update();

        //TODO: what about a tie?
        if (BoltNetwork.isServer && battlefield.canCheckGameOver &&
            battlefield.GameOver()) {
            Player winner = battlefield.livingPlayers[0];
            EndGame(winner);

        }
    }

    public void RestartGame() {
        TransitionTo(new ArcadePlayingGameState());
    }

    public void EndGame(Player victor) {
        GameEnded(victor);
    }

    public void GameEnded(Player victor) {
        if (battlefield.isAiVsAi) {
            TransitionTo(new PlayingGameState());
        }
        else {
            var state = new ArcadePostGameState();
            state.victoriousPlayer = victor;
            TransitionTo(state);
        }
    }

    public void Leave() {
        TransitionTo(new MainMenuState());
    }

    // In Game Menu
    /*
    void AddLevelMenu() {
        levelMenu = UI.Menu();

        levelMenu.anchor = MenuAnchor.TopRight;
        levelMenu.AddNewText().text = "Level " + battlefield.npcLevel + " (NPC POWER " + string.Format("{0:F2}", battlefield.localPlayer1.opponent.npcHandicap) + ")";

        levelMenu.Show();
    }
    */
}
