using UnityEngine;
using System.Collections;

public class NetworkObject : Bolt.EntityEventListener {
	//public interface

	public NetworkObjectDelegate networkObjectDelegate {
		get {
			return GetComponent<NetworkObjectDelegate>();
		}
	}

	public virtual void MasterSlaveStart() {
		networkObjectDelegate.MasterSlaveStart();
	}

	public virtual void MasterStart() {
		networkObjectDelegate.MasterStart();
	}

	public virtual void SlaveStart() {
		networkObjectDelegate.SlaveStart();
	}

	public virtual void MasterFixedUpdate() {
		networkObjectDelegate.MasterFixedUpdate();
	}

	public virtual void SlaveFixedUpdate() {
		networkObjectDelegate.SlaveFixedUpdate();
	}

	public virtual void QueuePlayerCommands() {
		networkObjectDelegate.QueuePlayerCommands();
	}

	public virtual void SlaveDied() {
		networkObjectDelegate.SlaveDied();
	}

	//internal interface

	public override void Attached() {
		base.Attached();

		MasterSlaveStart();

		if (BoltNetwork.isServer) {
			MasterStart();
		}

		SlaveStart();
	}

	public override void SimulateOwner() {
		base.SimulateOwner();

		MasterFixedUpdate();
	}

	void Update() {
		if (entity.isControllerOrOwner) {
			QueuePlayerCommands();
		}
	}

	void FixedUpdate() {
		SlaveFixedUpdate();
	}

	public override void Detached() {
		base.Detached();
		SlaveDied();
	}
}
