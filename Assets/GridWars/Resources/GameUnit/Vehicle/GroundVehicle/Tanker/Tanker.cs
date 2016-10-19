using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// GW1 speed tanker mass=50 thrust=1025

public class Tanker : GroundVehicle {
	public Explosion prefabBombExplosion;

	public override void Awake() {
		base.Awake();
		powerCostPerLevel = new float[] { 15f, float.MaxValue, float.MaxValue };
	}

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();
		SetupHazardLight();
	}

	private Color darkLightColor;
	private GameObject hazardLight;

	public void SetupHazardLight() {
		gameObject.EachRenderer(r => {
			if (r.gameObject.name.StartsWith("CenterConsole")) {
				hazardLight = r.gameObject;
			}
		});

		darkLightColor = Color.Lerp(Color.yellow, Color.black, 0.8f);
	}

	public override void ServerInit() {
		maxHitPoints = 8f;
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

	public void SetHazardLightColor(Color color) {
		hazardLight.EachMaterial(m => {
			m.color = color;
		});
	}

	public override void ServerAndClientUpdate() {
		base.ServerAndClientUpdate();

		if (target != null) {
			var period = 1f;
			float dist = TargetDistance();
			float r = period * dist / 30f;
			float p = 0.03f + r * r;

			if (p < period) {
				period = p;
			}

			float pitchDist = 35f;
			if (dist < pitchDist) {
				runningAudioSource.pitch = 1f + 2f * (1f - dist / pitchDist);
			}

			float c = (Mathf.Sin(2f * Mathf.PI * Time.time / period) + 1f) / 2f;
			SetHazardLightColor(Color.Lerp(Color.white, darkLightColor, 1f - c * c));
		} else {
			SetHazardLightColor(player.secondaryColor);
		}
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