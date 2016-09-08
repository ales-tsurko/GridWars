using UnityEngine;
using System.Collections;

public class PvpShared : DefaultNetworkDelegate {
	public override void Start() {
		base.Start();
		BoltLauncher.StartSinglePlayer();
	}
	
	public override void BoltStartDone() {
		base.BoltStartDone();
		Network.shared.StartGame();
	}
}
