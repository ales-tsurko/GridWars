using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// GW1 speed tanker mass=50 thrust=1025

public class Tanker : GroundVehicle {
	public Explosion prefabBombExplosion;

	public float distanceToTarget {
		get {
			return entity.GetState<ITankerState>().distanceToTarget;
		}

		set {
			entity.GetState<ITankerState>().distanceToTarget = value;
		}
	}

	public override void Awake() {
		base.Awake();
		powerCostPerLevel = new float[] { 15f, 20f, float.MaxValue };
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

		if (target == null) {
			distanceToTarget = float.MaxValue;
		}
		else {
			distanceToTarget = TargetDistance();
		}


		if (IsInStandoffRange()) {
			Die();
		}
	}

	public virtual List <GameObject> PossibleDefaultTargets() {
		var results = EnemyBuildingUnits();

		if (results.Count == 0) {
			results = EnemyNonAirUnits();
		}

		if (results.Count == 0) {
			results = EnemyAirUnits();
		}
		return results;
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

			if (otherUnit.IsOfType(typeof(Tower))) {
				Tower tower = (Tower)otherUnit;
				tower.DieWithBlockify();
				tower.Die();
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

		if (distanceToTarget < 1000f) {
			var period = 1f;
			float dist = distanceToTarget;
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

	public override void UpgradeVeterancy() {
		thrust *= 1.2f;
	}

}