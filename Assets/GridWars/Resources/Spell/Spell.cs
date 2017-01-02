using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class Spell {

	public GameUnit gameUnit;
	//public string name;
	public PlayerAction playerAction;

	public float startTime = 0f;
	public float factor = 1.3f;
	public string startSoundName;

	virtual public float Cost() {
		return 5f;
	}

	virtual public float LifeSpan() {
		return 10f;
	}

	public float Age() {
		return Time.time - startTime;
	}

	public float TimeLeft() {
		float dt = LifeSpan() - Age();
		if (dt < 0) {
			dt = 0f;
		}
		return dt;
	}

	/*
	public Spell() {
	}
	*/


	virtual public void ServerAndClientInit () {
		startTime = Time.time;
		//Debug.Log("Applying " + this.ClassName() + " to " + gameUnit.ClassName());
		App.shared.PlayAppSoundNamedAtVolume(startSoundName, 1f);
	}


	virtual public void ServerAndClientFixedUpdate () {
		if (LifeSpan() != 0f && Age() > LifeSpan()) {
			ServerAndClientStop();
		}
	}

	virtual public void ServerAndClientStop () {
		gameUnit.RemoveSpell(this);
		//Debug.Log("Removing " + this.ClassName() + " from " + gameUnit.ClassName());
	}


}
