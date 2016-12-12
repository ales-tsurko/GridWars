using UnityEngine;
using System.Collections.Generic;

public class MatchmakerMenu : UIMenu {
	public static string MatchmakerMenuOpenedNotification = "MatchmakerMenuOpenedNotification";
	public static string MatchmakerMenuClosedNotification = "MatchmakerMenuClosedNotification";

	public bool isOpen;
	public List<MatchmakerMenuDelegate> delegates;

	public void AddDelegate(MatchmakerMenuDelegate del) {
		delegates.Add(del);
	}

	public void RemoveDelegate(MatchmakerMenuDelegate del) {
		delegates.Remove(del);
	}

	public void Open() {
		if (!isOpen) {
			isOpen = true;
			Focus();
			App.shared.notificationCenter.NewNotification()
				.SetName(MatchmakerMenuOpenedNotification)
				.SetSender(this)
				.Post();
			foreach (var del in new List<MatchmakerMenuDelegate>(delegates)) {
				del.MatchmakerMenuOpened();
			}
		}
	}

	public void Close() {
		if (isOpen) {
			isOpen = false;
			App.shared.notificationCenter.NewNotification()
				.SetName(MatchmakerMenuClosedNotification)
				.SetSender(this)
				.Post();
			foreach (var del in new List<MatchmakerMenuDelegate>(delegates)) {
				del.MatchmakerMenuClosed();
			}
		}
	}

	// MonoBehaviour

	public override void Awake() {
		base.Awake();
		delegates = new List<MatchmakerMenuDelegate>();
		selectsOnShow = false;
	}

	protected override void Update() {
		base.Update();

		this.transform.SetAsLastSibling();
	}
}

public interface MatchmakerMenuDelegate {
	void MatchmakerMenuOpened();
	void MatchmakerMenuClosed();
}
