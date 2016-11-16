using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class GraphicsOptionsState : AppState {
    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);
        ShowGraphicsOptionsMenu();
    }

    public override void WillExit() {
        base.WillExit();
    }

    void ShowGraphicsOptionsMenu() {
        app.ResetMenu();
        menu.AddItem(UI.MenuItem("Resolution", ShowResolutionMenu));
        menu.AddItem(UI.MenuItem("Antialiasing", ShowAntiAliasingMenu));
        menu.AddItem(UI.MenuItem("Back", GoBackToOptions));
        menu.Show();
    }

    void ShowAntiAliasingMenu() {
        TransitionTo (new AAMenuState ());
    }

    void ShowResolutionMenu(){
        TransitionTo (new ResolutionMenuState ());
    }

    void GoBackToOptions(){
        TransitionTo (new OptionsMenuState ());
    }
}
