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

	/*
	public Spell() {
	}
	*/


	virtual public void ServerInit () {
		startTime = Time.time;
		Debug.Log("Applying " + this.ClassName() + " to " + gameUnit.ClassName());
	}

	virtual public void ServerStop () {
		gameUnit.RemoveSpell(this);
		Debug.Log("Removing " + this.ClassName() + " from " + gameUnit.ClassName());
	}

	virtual public void ServerFixedUpdate () {
		if (LifeSpan() != 0f && Age() > LifeSpan()) {
			ServerStop();
		}
	}

	float Age() {
		return Time.time - startTime;
	}



}
