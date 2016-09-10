using UnityEngine;
using System.Collections.Generic;

public class VsComp : DefaultNetworkDelegate {
	public override List<Player> localPlayers {
		get {
			var list = new List<Player>();
			list.Add(Battlefield.current.PlayerNumbered(1));
			return list;
		}
	}

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
