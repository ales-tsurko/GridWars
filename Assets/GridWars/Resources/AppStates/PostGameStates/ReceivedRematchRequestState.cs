using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class ReceivedRematchRequestState : PostGameSubState {
	public override void EnterFrom(AppState state) {
		openSoundtrackName = "Rematch";
		base.EnterFrom(state);

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("Opponent Requests a Rematch"));
		menu.AddItem(UI.MenuItem("Accept", AcceptRematch));
		menu.AddItem(UI.MenuItem("Decline", postGameState.Leave));
		menu.Show();
	}

	void RejectRematch() {
		postGameState.Leave();
        string oScreenName = "null";
        string oID = "null";
        if (App.shared.account.opponent != null) {
            oScreenName = App.shared.account.screenName;
            oID = App.shared.account.opponent.GetID();
        }
        Analytics.CustomEvent("RematchRejected", new Dictionary<string, object>
            {
                { "platform", Application.platform.ToString() },
                { "id", App.shared.account.GetID() },
                { "screenName", App.shared.account.screenName },
                { "opponentId", oID },
                { "opponentScreenName", oScreenName }
            });
	}

	void AcceptRematch() {
		matchmaker.Send("requestRematch");

		app.ResetMenu();
		menu.AddNewIndicator().SetText("Starting Game");
		menu.AddNewButton().SetText("Leave").SetAction(postGameState.Leave);
		menu.Show();
        string oScreenName = "null";
        string oID = "null";
        if (App.shared.account.opponent != null) {
            oScreenName = App.shared.account.screenName;
            oID = App.shared.account.opponent.GetID();
        }
        Analytics.CustomEvent("RematchAccepted", new Dictionary<string, object>
                {
                    { "platform", Application.platform.ToString() },
                    { "id", App.shared.account.GetID() },
                    { "screenName", App.shared.account.screenName },
                    { "opponentId", oID },
                    { "opponentScreenName", oScreenName }
                });
	}
}
