using UnityEngine;
using System.Collections;

public class RemapMenuState : AppState {
    public int currentRemapPlayerNum;
    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);
		App.shared.keys.InitKeyMappings();
        app.ResetMenu();
		foreach(KeyData k in App.shared.keys.keyData) {
            if (k.playerNum != currentRemapPlayerNum) {
                continue;
            }
            menu.AddItem(UI.ButtonPrefabKeyMap(k));
        }
        menu.AddItem(UI.ButtonPrefabKeyMap(null, true, "Reset to Defaults", ResetToDefaults));
        menu.AddItem(UI.ButtonPrefabKeyMap(null, true, "Back", CloseMenu));
        menu.Show();
    }

    void ResetToDefaults() {
		App.shared.keys.SetDefaults();
        TransitionTo(new RemapMenuState(){ currentRemapPlayerNum = this.currentRemapPlayerNum });
    }

    void CloseMenu() {
        TransitionTo(new OptionsMenuState());
    }
}
