using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpell : Spell {
	GameObject sphere;

	override public float Cost() {
		return 7f;
	}

	override public float LifeSpan() {
		return 6.3f;
	}

	override public void ServerAndClientInit () {
		base.ServerAndClientInit();

		gameUnit.SetArmor(0.9f);

		sphere = GameObject.CreatePrimitive( PrimitiveType.Sphere );
		sphere.transform.localScale = Vector3.one * 4f;
		sphere.transform.position = gameUnit.transform.position;
		sphere.transform.parent = gameUnit.gameObject.transform;

		sphere.GetComponent<Collider>().enabled = false;

		Renderer r = sphere.GetComponent<Renderer>();
		r.material = App.shared.LoadMaterial("Materials/Shield");
		//r.material.color = new Color(1f, 1f, 1f, 0.01f);
		//Debug.Log("r.material.name = " + r.material.name);

		// sphere.SetAlpha(0.01f);
	}

	override public void ServerAndClientFixedUpdate () {
		base.ServerAndClientFixedUpdate();

		float dt = TimeLeft();
		if (dt < 1f) {
			sphere.SetAlpha(0.01f * dt);
		}
	}

	override public void ServerAndClientStop () {
		base.ServerAndClientStop();
		gameUnit.SetArmor(0.0f);
		Object.Destroy(sphere);
	}
}
