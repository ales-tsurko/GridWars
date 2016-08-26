using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameUnit : BetterMonoBehaviour, NetworkObjectDelegate {
	public float thrust;
	public float rotationThrust;
	bool isAlive = true;

	GameUnitDelegate gameUnitDelegate {
		get {
			return GetComponent<GameUnitDelegate>();
		}
	}

	Player _player;
	public Player player {
		get {
			return gameUnitState.player;
		}

		set {
			gameUnitState.player = value;
		}
	}
		
	// Damagable
	float _hitPoints;
	public float hitPoints {
		get {
			return gameUnitState.hitPoints;
		}

		set {
			gameUnitState.hitPoints = value;
		}
	}

	public float maxHitPoints;


	[HideInInspector]
	public bool isTargetable;
	public bool isRunning = true;

	public GameObject target = null;
	//public float angleToTarget = 0;

	public bool isStaticUnit = false;

	//tower
	public float powerCost = 4f;
	public float cooldownSeconds = 1f;
	public float standOffDistance = 20f;
	public KeyCode[] buildKeyCodeForPlayers = new KeyCode[2];

	public float hpRatio {
		get {
			return hitPoints/maxHitPoints;
		}
	}


	// --- Sounds ------------------------------------------

	AudioSource _audioSource;
	protected AudioSource audioSource {
		get {
			if (_audioSource == null) {
				_audioSource = gameObject.AddComponent<AudioSource>();
			}
			return _audioSource;
		}
	}
		
	public AudioClip birthSound {
		get {
			return SoundNamed("birth");
		}
	}
		
	protected void PlayBirthSound() {
		if (birthSound != null) {
			audioSource.PlayOneShot(birthSound);
		}
	}

	// ----------------------------------------------

	public GameObject deathExplosionPrefab;

	public static T Load<T>() where T: GameUnit {
		return (T) Load(typeof(T));
	}

	// --- Finding Resources --------------------------------------

	public string ResourcePath() {
		System.Type type = GetType();
		List <string> pathComponents = new List<string>();

		while (type != typeof(GameUnit)) {
			pathComponents.Add(type.Name);
			type = type.BaseType;
		}
		pathComponents.Add("GameUnit");
		pathComponents.Reverse();
		return string.Join("/", pathComponents.ToArray());
	}

	public string PrefabPath() {
		string path = ResourcePath();
		return path + "/Prefabs/" + GetType().Name;
	}

	public AudioClip SoundNamed(string name) {
		string path = ResourcePath();
		string soundPath = path + "/Sounds/" + name;
		return Resources.Load<AudioClip>(soundPath);
	}

	// ------------------------------------------------------------

	public static GameUnit Load(System.Type type) {
		var prefabPath = App.shared.PrefabPathForUnitType(type);
		GameObject obj = (GameObject) Resources.Load(prefabPath);

		if (obj == null) {
			print("missing prefabPath " + prefabPath);
			throw new System.Exception("missing prefabPath " + prefabPath);
		} else {
			//print("found prefabPath " + prefabPath);
		}

		return (GameUnit) obj.GetComponent(type);
	}

	// --- MonoBehaviour --------------------------------------------

	//Awake might be called multiple times for the same loaded prefab.
	//Make sure that its idempotent.
	protected override void Awake() {
		base.Awake();

		isTargetable = true;

		if (gameUnitDelegate == null) {
			if (boltEntity == null) {
				gameObject.AddComponent<StandaloneGameUnit>();
			}
			else {
				gameObject.AddComponent<NetworkedGameUnit>();
			}
		}
	}

	//TODO: Remove after all subclasses implement NetworkObjectDelegate
	public virtual void Start() {
	}

	//TODO: Remove after all subclasses implement NetworkObjectDelegate
	public virtual void FixedUpdate() {}

	//Networking

	GameUnitState _gameUnitState;
	public GameUnitState gameUnitState {
		get {
			return _gameUnitState;
		}

		set {
			if (value != null) {
				value.gameUnit = this;
			}
			_gameUnitState = value;
		}
	}

	public BoltEntity boltEntity {
		get {
			return GetComponent<BoltEntity>();
		}
	}

	public GameUnit Instantiate() {
		Awake();
		var unit = gameUnitDelegate.InstantiateGameUnit();
		gameUnitState = null;
		return unit;
	}

	public virtual void MasterSlaveStart() {
	}

	public virtual void MasterStart() {
	}

	public virtual void SlaveStart() {
		SetupWeapons();
		SetupSmokeDamage ();
		SetupDeathExplosion ();

		gameObject.CloneMaterials();

		if (player == null) {
			gameObject.Paint(Color.white, "Unit");
		}
		else {
			player.Paint(gameObject);
		}

		PlayBirthSound();
	}

	public virtual void MasterFixedUpdate(){
		/*
		if (player == null) {
			print ("SimulateOwner null player on " + this);
		}
		*/

		foreach (var weapon in Weapons()) {
			if (weapon.isActiveAndEnabled) {
				weapon.SimulateOwner();
			}
		}

		RemoveIfOutOfBounds ();
	}

	public virtual void SlaveFixedUpdate(){}


	// -----------------------

	public virtual Rigidbody rigidBody() {
		if (body == null) {
			body = GetComponent<Rigidbody> ();
		}
		return body;
	}
	Rigidbody body;

	// -----------------------

	public bool IsInStandoffRange() {
		if (standOffDistance == -1) {
			return true;
		}

		return target && (targetDistance() < standOffDistance);
	}
		
	public virtual List<GameObject> activeGameObjects() {
		GameObject[] objs = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		var results = new List<GameObject>();
		foreach (GameObject obj in objs) {
			if (obj.activeInHierarchy && !obj.IsDestroyed()) {
				results.Add(obj);
			}
		}
		return results;
	}

	public virtual bool IsEnemyOf(GameUnit otherUnit) {
		if (otherUnit == null) {
			return false;
		}

		if (player == null) {
			//print ("null player " + this);
			return false;
		}

		if (otherUnit == null) {
			//print ("null otherUnit " + otherUnit);
			return false;
		}

		if (otherUnit.player == null) {
			//print ("null otherUnit.player " + otherUnit.player);
			return false;
		}

		return (player.playerNumber != otherUnit.player.playerNumber);
		//return (player != null) && (otherUnit.player != null) && (player != otherUnit.player);
	}

	public float DistanceToObj(GameObject obj) {
		// please do not change this to sqrMagnitude
		return Vector3.Distance(obj.transform.position, transform.position);
	}

	public virtual List<GameObject> EnemyObjects() {

		List<GameUnit> gameUnits = new List<GameUnit>(FindObjectsOfType<GameUnit>()).FindAll(unit => !unit.isDestroyed);
		var results = new List<GameObject>();
		foreach (GameUnit gameUnit in gameUnits) {
			if (gameUnit.gameObject.IsDestroyed() == false) {
				if (gameUnit.player && (gameUnit.player != player) && CanTargetUnit(gameUnit)) {
					results.Add(gameUnit.gameObject);
				}
			}
		}
		return results;

		/*var objs = activeGameObjects();
		foreach (GameObject obj in objs) {
			GameUnit unit = obj.GetComponent<GameUnit> ();
			//if (obj.tag.Contains("Player") && !obj.tag.Equals(this.tag)) {
			if ((obj.tag != null) && (unit != null && IsEnemyOf(unit))) {
				results.Add(obj);
			}
		}

		return results;*/
	}

	// --- targeting -------------------

	public virtual bool CanTargetUnit(GameUnit unit) {

		if (unit.isTargetable) {
			return true;
		}

		return false;
	}

	public virtual Weapon HighestPriorityWeaponWithTarget() {
		Weapon[] weapons = Weapons();

		Weapon result = null;
		int highestPriority = -1;
		foreach (Weapon weapon in weapons) {
			if (weapon.target && highestPriority < weapon.priority) {
				result = weapon;
				highestPriority = weapon.priority;
			}
		}

		return result;
	}

	public virtual void PickTarget() {
		Weapon targetingWeapon = HighestPriorityWeaponWithTarget ();

		if (target && target.IsDestroyed()) {
			target = null;
		}

		if (targetingWeapon) {
			GameObject newTarget = targetingWeapon.target;
			//GameObject newTarget = ClosestEnemyObject ();

			if (target != newTarget) {
				target = newTarget;
				//UpdatedTarget();
			}
		}
	}

	public virtual GameObject ClosestEnemyObject() {
		var objs = EnemyObjects();
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = _t.position;
		foreach (GameObject obj in objs) {
			Vector3 diff = obj.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = obj;
				distance = curDistance;
			}
		}
		return closest;
	}


	// -----------------------

	public virtual bool isOutOfBounds () {
		return (
			(y() < -3) || (y() > 50) ||
			(x() > 50) || (x() > 50) ||
			(z() > 50) || (z() > 50) 
		);
	}

	public virtual void RemoveIfOutOfBounds () {
		if (isOutOfBounds() ) {
			Destroy (gameObject);
		}
	}


	// --- aiming --------------------

	public float AngleToTarget() {
		var targetPos = target.transform.position;

		Vector3 targetDir = (targetPos - _t.position).normalized;
		float angle = AngleBetweenOnAxis(_t.forward, targetDir, _t.up);
		return angle;
	}

	public float targetDistance() {
		return Vector3.Distance(transform.position, target.transform.position);
	}

	void OnCollisionEnter(Collision collision) {
		/*
		if (collision.collider.name == "BattlefieldPlane") {
			return;
		}

		GameUnit otherUnit = collision.gameObject.GetComponent<GameUnit> ();

		//print(this.player.playerNumber + " collision " + otherUnit.player.playerNumber);

		if (IsEnemyOf (otherUnit)) {
			//print(this.player.playerNumber + " collision " + otherUnit.player.playerNumber);
			//Destroy (gameObject);
		}

		foreach (ContactPoint contact in collision.contacts) {
			Debug.DrawRay (contact.point, contact.normal, Color.white);
		}
		*/

		/*
		if (collision.relativeVelocity.magnitude > 2) {
			//audio.Play ();
			//print("collision");
			Destroy (gameObject);
		}
		*/
	}

	// --- icons --------------------

	void OnDrawGizmos() {
		if (target != null) {
			Gizmos.color = Color.green;
			//Gizmos.DrawLine (_t.position, target.transform.position);
		}
	}

	bool _isDestroyed = false;
	public bool isDestroyed {
		get {
			return _isDestroyed || gameObject.IsDestroyed();
		}
	}

	public void DestroySelf() {
		_isDestroyed = true;
		gameUnitDelegate.DestroySelf();
	}

	// --- Damage ------------------------------------------

	public virtual void ApplyDamage(float damage) {
		if (!isAlive) {
			return;
		}

		if (isDestroyed) {
			return;
		}

		hitPoints -= damage;
		if (smokeDamage !=null) {
			smokeDamage.maxParticles = Mathf.Clamp (1000 - (int)((hitPoints / maxHitPoints) * 1000), 250, 1000);
		}
		if (hitPoints <= 0) {
			OnDead();
		}
	}

	// --- Damage Smoke ------------------------------------------

	//Particles for displaying damage amount to units
	ParticleSystem smokeDamage;
	void SetupSmokeDamage () {
		Transform smokeDamageT = _t.FindChild ("SmokeDamage");
		if (smokeDamageT != null) {
			smokeDamage = smokeDamageT.GetComponentInChildren<ParticleSystem> ();
			smokeDamage.maxParticles = 0;
			smokeDamage.simulationSpace = ParticleSystemSimulationSpace.World;
		}
	}

	void SetupDeathExplosion () {
		deathExplosionPrefab = Resources.Load<GameObject> (App.shared.ResourcePathForUnitType (GetType ()) + "/Prefabs/DeathExplosion");
	}


	// --- Death ------------------------------------------

	public virtual void OnDead() {
		if (isAlive) {
			isAlive = false;
			Camera cam = _t.GetComponentInChildren<Camera>();
			if (cam) {
				cam.transform.parent = null;
				FindObjectOfType<CameraController>().SendMessage("ResetCamera", SendMessageOptions.DontRequireReceiver);
			}
			ShowUnitExplosion();
			DestroySelf();
		}
	}

	void OnDestroy() {
		ShowFxExplosion();
	}

	void ShowUnitExplosion() {
		if (deathExplosionPrefab != null) {
			var unitExplosion = deathExplosionPrefab.GetComponent<GameUnit>();
			if (unitExplosion != null) {
				var state = new GameUnitState();
				state.prefabGameUnit = deathExplosionPrefab.GetComponent<Explosion>();
				state.transform = _t;
				state.InstantiateGameUnit();
			}
		}
	}

	void ShowFxExplosion() {
		if (deathExplosionPrefab != null) {
			var unitExplosion = deathExplosionPrefab.GetComponent<GameUnit>();
			if (unitExplosion == null) {
				var obj = Instantiate(deathExplosionPrefab);
				obj.transform.position = _t.position;
				obj.transform.rotation = _t.rotation;
			}
		}
		
	}
		
	// --- Weapons ------------------------------------------

	public Weapon[] Weapons() {
		Weapon[] weapons = GetComponentsInChildren<Weapon>();
		return weapons;
	}

	public void SetupWeapons() {
		Weapon[] weapons = Weapons();

		foreach (Weapon weapon in weapons) {
			weapon.owner = gameObject;
			weapon.enabled = true;
		}
	}

	public void DeactivateWeapons() {
		Weapon[] weapons = Weapons();

		foreach (Weapon weapon in weapons) {
			weapon.isActive = false;
			weapon.enabled = false;
		}
	}
		
}
