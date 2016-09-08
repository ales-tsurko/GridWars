using UnityEngine;
using System.Collections;

public class VsComp : DefaultNetworkDelegate {
	public override void Start() {
		base.Start();

		BoltLauncher.StartSinglePlayer();
	}

	public override void BoltStartDone() {
		base.BoltStartDone();
		Network.shared.StartGame();
		Battlefield.current.PlayerNumbered(2).npcModeOn = true;
	}
}
