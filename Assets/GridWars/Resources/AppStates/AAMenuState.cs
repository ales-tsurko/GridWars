using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class AAMenuState : AppState {
    private static bool _needsInitialFadeIn = true;

    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);
        
    }

    public override void WillExit() {
        base.WillExit();
    }

    void ShowOptionsMenu() {
        app.ResetMenu();
        menu.AddItem(UI.MenuItem("Resolution", ShowResolutionMenu));
        menu.AddItem(UI.MenuItem("Antialiasing", ShowAntiAliasingMenu));
        menu.AddItem(UI.MenuItem("Back", GoBackToOptions));
        menu.Show();

        if (_needsInitialFadeIn) {
            menu.backgroundColor = Color.black;
            menu.targetBackgroundColor = Color.clear;
            _needsInitialFadeIn = false;
        }
    }

    void ShowAntiAliasingMenu() {
        TransitionTo (new AAMenuState ());
    }

    void ShowResolutionMenu(){
        TransitionTo (new ResolutionMenuState ());
    }

    void GoBackToOptions(){
        TransitionTo(new GraphicsOptionsState());
    }
}
