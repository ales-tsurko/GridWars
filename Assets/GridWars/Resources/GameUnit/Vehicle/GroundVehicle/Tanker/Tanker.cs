using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// GW1 speed tanker mass=50 thrust=1025

public class Tanker : GroundVehicle {
	public Explosion prefabBombExplosion;
	GameObject centerConsole;

	public override void Awake() {
		base.Awake();
		powerCostPerLevel = new float[] { 15f, float.MaxValue, float.MaxValue };

	}

	/*
	public GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren(typeof(GameObject));
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
		return null;
	}
	*/

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();
		Transform t = transform.Find("CenterConsole");
		if (t != null) {
			centerConsole = t.gameObject;
		}
	}

	public override void ServerInit() {
		maxHitPoints = 13f;
		base.ServerInit();
	}

	public override void ServerFixedUpdate() {
		base.ServerFixedUpdate();

		if (IsInStandoffRange()) {
			Die();
		}
	}

	public virtual List <GameObject> PossibleDefaultTargets() {
		return NonAirEnemyVehicles();
	}

	public override GameObject DefaultTarget() {
		return ClosestOfObjects(PossibleDefaultTargets());
	}

	public void BlowUp() {
		var boom = prefabBombExplosion.GetComponent<BigBoom>().Instantiate();
		boom.player = player;
		boom.transform.position = transform.position;
		boom.transform.rotation = transform.rotation;
	}

	public override void Die() {
		base.Die();
		BlowUp();
	}

	public override void OnCollisionEnter(Collision collision) {
		base.OnCollisionEnter(collision);
		if (!enabled) {
			return;
		}

		GameUnit otherUnit = collision.gameObject.GetComponent<GameUnit> ();

		if (otherUnit != null && !IsFriendOf(otherUnit)) {
			if (!otherUnit.IsOfType(typeof(Projectile))) {
				Die();
			}
		}
	}

	public override void ServerAndClientUpdate() {
		base.ServerAndClientUpdate();

		var period = 1f;
		//var min = 0.25f;
		//var max = 0.75f;
		//var value = Mathf.Sin(2*Mathf.PI*Time.time/period);
		var x = Time.time;

		float c = (Mathf.Sin(2f * Mathf.PI * x / period) + 1f)/2f;

		if (centerConsole != null) {
			centerConsole.EachMaterial(m => {
				m.color = new Color(c, c, c);
			});
		}
		/*

		//var y = 1 - 2*Mathf.Abs(Mathf.Round(x/period) - x/period); //triangle sawtooth
		var y = Mathf.Sin(2f * Mathf.PI * x / period); 

		var intensity = y*(max - min) + min;
		
		var c = player.primaryColor * Mathf.LinearToGammaSpace(intensity);
		centerConsole.EachMaterial(m => {
			if (m.name.StartsWith(Player.primaryColorMaterialName)) {
				//m.SetColor("_EmissionColor", c);
			}
		});
		*/
	}

	override public bool HasWeapons() {
		return true;
	}

	public override List<System.Type> CountersTypes() {
		List<System.Type> counters = base.CountersTypes();
		counters.Add(typeof(MobileSAM));
		counters.Add(typeof(Tank));
		counters.Add(typeof(Tanker));
		counters.Add(typeof(Chopper));
		return counters;
	}

}