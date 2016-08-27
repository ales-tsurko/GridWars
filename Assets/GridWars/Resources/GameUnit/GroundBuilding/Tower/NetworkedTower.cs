using UnityEngine;
using System.Collections;

public class NetworkedTower : NetworkedGameUnit {
	public override void SlaveStart() {
		shouldDestroyColliderOnClient = false;
		base.SlaveStart();
	}

	public override void OnEvent(AttemptQueueUnitEvent evnt) {
		base.OnEvent(evnt);

		if (BoltNetwork.isServer) {
			tower.AttemptQueueUnit();
		}
	}

	Tower tower {
		get{
			return GetComponent<Tower>();
		}
	}
}
