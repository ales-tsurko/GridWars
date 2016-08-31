using UnityEngine;
using System.Collections;

public class NetworkObject : BetterMonoBehaviour {
	//public interface

	public BoltEntity entity {
		get {
			return GetComponent<BoltEntity>();
		}
	}

	public virtual void MasterInit() {
	}

	public virtual void ClientInit() {
	}

	public virtual void SlaveInit() {
	}

	public virtual void MasterSlaveStart() {
	}

	public virtual void MasterStart() {
	}

	public virtual void ClientStart() {
	}

	public virtual void SlaveStart() {
	}

	public virtual void MasterFixedUpdate() {
	}

	public virtual void SlaveFixedUpdate() {
	}

	public virtual void QueuePlayerCommands() {
	}

	public virtual void SlaveDied() {
	}
}
