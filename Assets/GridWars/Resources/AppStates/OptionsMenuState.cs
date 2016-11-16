using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class OptionsMenuState : AppState {

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
    }

    void ShowGraphicsOptions(){
        TransitionTo(new GraphicsOptionsState());
    }

    void GoBackToMain(){
        TransitionTo(new MainMenuState());
    }
}
