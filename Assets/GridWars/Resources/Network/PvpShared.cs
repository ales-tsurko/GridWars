using UnityEngine;
using System.Collections.Generic;

public class PvpShared : DefaultNetworkDelegate {
	public override List<Player> localPlayers {
		get {
			var list = new List<Player>();
			list.Add(Battlefield.current.PlayerNumbered(1));
			list.Add(Battlefield.current.PlayerNumbered(2));
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
	}
}
