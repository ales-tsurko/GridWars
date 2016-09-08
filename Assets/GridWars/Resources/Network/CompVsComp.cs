using UnityEngine;
using System.Collections;

public class CompVsComp : DefaultNetworkDelegate {
	public override void Start() {
		base.Start();

		BoltLauncher.StartSinglePlayer();
	}

	public override void BoltStartDone() {
		base.BoltStartDone();
		Network.shared.StartGame();
		Battlefield.current.PlayerNumbered(1).npcModeOn = true;
		Battlefield.current.PlayerNumbered(2).npcModeOn = true;


	}
}