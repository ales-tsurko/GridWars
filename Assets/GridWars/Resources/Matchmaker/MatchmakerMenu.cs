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
		if (!isOpen) {
			isOpen = true;
			foreach (var del in new List<MatchmakerMenuDelegate>(delegates)) {
				del.MatchmakerMenuOpened();
			}
			Focus();
		}
	}

	public void Close() {
		if (isOpen) {
			isOpen = false;
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
