using UnityEngine;
using System.Collections.Generic;

public class MatchmakerMenu : UIMenu {
	public bool isOpen;
	public List<MatchmakerMenuDelegate> delegates;

	public void AddDelegate(MatchmakerMenuDelegate del) {
		delegates.Add(del);
	}

	public void RemoveDelegate(MatchmakerMenuDelegate del) {
		delegates.Remove(del);
	}

	public void Open() {
		isOpen = true;

		SetAnchor(MenuAnchor.MiddleCenter);
		UseDefaultBackgroundColor();

		foreach (var del in new List<MatchmakerMenuDelegate>(delegates)) {
			del.MatchmakerMenuOpened();
		}

		Focus();
	}

	public void Close() {
		isOpen = false;
		ConfigureForClosed();
		foreach (var del in new List<MatchmakerMenuDelegate>(delegates)) {
			del.MatchmakerMenuClosed();
		}
	}

	void ConfigureForClosed() {
		this.SetAnchor(MenuAnchor.TopCenter);
		selectsOnShow = false;
		backgroundColor = Color.clear;
	}

	// MonoBehaviour

	public override void Awake() {
		base.Awake();
		delegates = new List<MatchmakerMenuDelegate>();
		ConfigureForClosed();
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
