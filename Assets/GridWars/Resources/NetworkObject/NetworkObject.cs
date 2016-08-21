using UnityEngine;
using System.Collections;

public interface NetworkObjectDelegate {
	void MasterFixedUpdate();
	void NetworkStart();
}

public class NetworkObject : Bolt.EntityBehaviour {
	public NetworkObjectDelegate del {
		get {
			return GetComponent<GameUnit>();
		}
	}

	public override void SimulateOwner() {
		base.SimulateOwner();
		del.MasterFixedUpdate();
	}

	public override void Attached() {
		base.Attached();
		del.NetworkStart();
	}
}
