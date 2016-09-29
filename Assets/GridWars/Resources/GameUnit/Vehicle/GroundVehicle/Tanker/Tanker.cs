using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tanker : GroundVehicle {
	public Explosion prefabBombExplosion;
		
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

		var period = 1;
		var min = 0.25f;
		var max = 0.75f;
		//var value = Mathf.Sin(2*Mathf.PI*Time.time/period);

		var x = Time.time;


		//var y = 1 - 2*Mathf.Abs(Mathf.Round(x/period) - x/period); //triangle sawtooth
		var y = Mathf.Sin(2*Mathf.PI*x/period); //sin

		var intensity = y*(max - min) + min;

		var color = player.primaryColor*Mathf.LinearToGammaSpace(intensity);

		gameObject.EachMaterial(m => {
			if (m.name.StartsWith(Player.primaryColorMaterialName)) {
				//Debug.Log(color);
				m.SetColor("_EmissionColor", color);
			}
		});
	}

	override public bool HasWeapons() {
		return true;
	}

}