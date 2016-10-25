using UnityEngine;
using System.Collections;

public class PostGameSubState : AppState {
	public PostGameState postGameState {
		get {
			return owner as PostGameState;
		}
	}

	public virtual void ReceivedRematchRequest() {
	}

	public virtual void ReceivedAcceptRematch() {
	}

	public virtual void Disconnected() {
	}
}
