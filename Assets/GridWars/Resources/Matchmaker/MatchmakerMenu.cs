using UnityEngine;
using System.Collections;

public class MatchmakerMenu : UIMenu {
	public bool isOpen;
	public MatchmakerMenuDelegate theDelegate;

	public void Open() {
		isOpen = true;

		SetAnchor(MenuAnchor.MiddleCenter);
		this.backgroundColor = new Color(0, 0, 0, 1);
		selectsOnShow = true;

		theDelegate.MatchmakerMenuOpened();
	}

	public void Close() {
		isOpen = false;
		ConfigureForClosed();
		theDelegate.MatchmakerMenuClosed();
	}

	void ConfigureForClosed() {
		this.SetAnchor(MenuAnchor.TopCenter);
		this.backgroundColor = new Color(0, 0, 0, 0);
		selectsOnShow = false;
	}

	// MonoBehaviour

	public override void Start() {
		base.Start();

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
