using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class OptionsMenuState : AppState {
   
    private static bool _needsInitialFadeIn = true;

    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);
        ShowOptionsMenu();
    }

    public override void WillExit() {
        base.WillExit();
    }

    void ShowOptionsMenu() {
        app.ResetMenu();
        menu.AddItem(UI.MenuItem("Graphics", ShowGraphicsOptions));
        menu.AddItem(UI.MenuItem("Back", GoBackToMain));
        menu.Show();

        if (_needsInitialFadeIn) {
            menu.backgroundColor = Color.black;
            menu.targetBackgroundColor = Color.clear;
            _needsInitialFadeIn = false;
        }
    }

    void ShowGraphicsOptions(){
        TransitionTo(new GraphicsOptionsState());
    }

    void GoBackToMain(){
        TransitionTo(new MainMenuState());
    }
}
