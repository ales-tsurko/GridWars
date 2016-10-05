using UnityEngine;
using System.Collections;

public class OptionsMenuState : AppState {

    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        app.ResetMenu();
        menu.AddItem(UI.MenuItem("Remap Player 1 Keys", RemapKeys1));
        menu.AddItem(UI.MenuItem("Remap Player 2 Keys", RemapKeys2));
        menu.AddItem(UI.MenuItem("Back", ShowMainMenu));
        menu.SetBackground(Color.black, 1);
        menu.Show();
    }

    void RemapKeys1() {
        TransitionTo(new RemapMenuState() { currentRemapPlayerNum = 1 });
    }
    void RemapKeys2() {
        TransitionTo(new RemapMenuState() { currentRemapPlayerNum = 2 });
    }

    void ShowMainMenu() {
        TransitionTo(new MainMenuState());
    }
}
