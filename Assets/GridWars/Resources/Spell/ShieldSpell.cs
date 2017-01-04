using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpell : Spell {
	GameObject sphere;

	override public float Cost() {
		return 20f;
	}

	override public float LifeSpan() {
		if (gameUnit.GetType() == typeof(Tanker)) {
			return 3.7f;
		}
		return 7f;
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
	}

	override public void ServerAndClientFixedUpdate () {
		base.ServerAndClientFixedUpdate();

		float dt = TimeLeft();
		if (dt < 1f) {
			//sphere.SetAlpha(0.01f * dt);
			Renderer r = sphere.GetComponent<Renderer>();
			Color c = r.material.color;
			float f = 0.95f;
			c.r *= f;
			c.g *= f;
			c.b *= f;
			c.a *= f;
			r.material.color = c;
		}
	}

	override public void ServerAndClientStop () {
		base.ServerAndClientStop();
		gameUnit.SetArmor(0.0f);
		Object.Destroy(sphere);
	}
}
