using UnityEngine;
using System.Collections;

public class NetworkObject : BetterMonoBehaviour {
	//public interface

	public BoltEntity entity {
		get {
			return GetComponent<BoltEntity>();
		}
	}

	//The following methods are called in the order that they appear

	public virtual void ServerAndClientInit() {
		if (debug) {
			Debug.Log(this + " ServerAndClientInit");
		}
	}

	public virtual void ServerInit() {
		if (debug) {
			Debug.Log(this + " ServerInit");
		}
	}

	public virtual void ClientInit() {
		if (debug) {
			Debug.Log(this + " ClientInit");
		}
	}

	public virtual void ServerJoinedGame() {
		if (debug) {
			Debug.Log(this + " ServerJoinedGame");
		}
	}

	public virtual void ClientJoinedGame() {
		if (debug) {
			Debug.Log(this + " ClientJoinedGame");
		}
	}

	public virtual void ServerAndClientJoinedGame() {
		if (debug) {
			Debug.Log(this + " ServerAndClientJoinedGame");
		}
	}

	public virtual void ServerFixedUpdate() {
		if (debug) {
			Debug.Log(this + " ServerFixedUpdate");
		}
	}

	public virtual void ClientFixedUpdate() {
		if (debug) {
			Debug.Log(this + " ClientFixedUpdate");
		}
	}

	public virtual void ServerAndClientFixedUpdate() {
		if (debug) {
			Debug.Log(this + " ServerAndClientFixedUpdate");
		}
	}

	public virtual void ServerUpdate() {
		if (debug) {
			Debug.Log(this + " ServerUpdate");
		}
	}

	public virtual void ClientUpdate() {
		if (debug) {
			Debug.Log(this + " ClientUpdate");
		}
	}

	public virtual void ServerAndClientUpdate() {
		if (debug) {
			Debug.Log(this + " ServerAndClientUpdate");
		}
	}

	public virtual void ServerLeftGame() {
		if (debug) {
			Debug.Log(this + " ServerLeftGame");
		}
	}

	public virtual void ServerAndClientLeftGame() {
		if (debug) {
			Debug.Log(this + " ServerAndClientLeftGame");
		}
	}

	public virtual void ClientLeftGame() {
		if (debug) {
			Debug.Log(this + " ClientLeftGame");
		}
	}

	// protected

	protected void DidLeaveGame() {
		ServerAndClientLeftGame();
		if (BoltNetwork.isServer) {
			ServerLeftGame();
		}
		else {
			ClientLeftGame();
		}
	}

	//MonoBehaviour

	/*
	protected virtual void FixedUpdate() {
		throw new System.Exception("FixedUpdate shouldn't be used in NetworkObjects");
	}

	protected virtual void Update() {
		throw new System.Exception("Update shouldn't be used in NetworkObjects");
	}
	*/

	//internal 

	bool debug = false;
}
