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

	protected List<Bolt.Event> playerCommands;

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
		playerCommands = new List<Bolt.Event>();
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
		} else {
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

	public virtual void QueuePlayerCommands(){}


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
			if (obj.activeInHierarchy) {
				if (!obj.IsDestroyed()) {
					results.Add(obj);
				}
			}
		}
		return results;
	}

	public virtual bool IsFriendOf(GameUnit otherUnit) {
		if (otherUnit == null) {
			return false;
		}

		if (player == null) {
			return false;
		}

		return player.IsFriendOf(otherUnit.player);
	}


	public virtual bool IsEnemyOf(GameUnit otherUnit) {
		if (otherUnit == null) {
			return false;
		}

		if (player == null) {
			return false;
		}

		return player.IsEnemyOf(otherUnit.player);
	}

	public float DistanceToObj(GameObject obj) {
		// please do not change this to sqrMagnitude
		return Vector3.Distance(obj.transform.position, transform.position);
	}

	public virtual List<GameObject> EnemyObjects() {
		/*
		List<GameUnit> results = new List<GameUnit>(FindObjectsOfType<GameUnit>()).FindAll(
			(unit => !unit.isDestroyed) && this.IsEnemyOf(unit) && this.CanTargetUnit(unit)
		);
		*/

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
	}

	public virtual List<GameObject> EnemyObjectsWithWeapons() {
		List<GameObject> objs = EnemyObjects();
		List<GameObject> results = new List<GameObject>();
		foreach (GameObject enemyObj in objs) {
			if (enemyObj.GameUnit().Weapons().Length > 0) {
				results.Add(enemyObj);
			}
		}
		return results;
	}

	/*
	public virtual List<GameObject> EnemyObjectsWithWeaponsThatCanTargetMe() {
		List<GameObject> objs = EnemyObjects();
		List<GameObject> results = new List<GameObject>();
		foreach (GameObject enemyObj in objs) {
			if (enemyObj.GameUnit().CanTargetUnit(this)) {
				results.Add(enemyObj);
			}
		}
		return results;
	}

	// --- targeting -------------------


	public virtual float PriorityToTargetUnit(GameObject obj) {
		GameUnit unit = object.GameUnit();
		float p = 0;

		// these two are taken care of elsewhere
		// higher if we have weapon that can target it
		// higher if closer 

		if (unit) {

			// higher if has any weapon
			if (unit.Weapons().Count > 0) {
				p += 1;
			}

			// higher if has weapon that can target us
			if (unit.CanTargetUnit(this)) {
				p += 1;
			}
		}

		return p;
	}
	*/

	public virtual bool CanTargetUnit(GameUnit unit) {

		if (unit.isTargetable) {
			foreach (Weapon weapon in Weapons()) {
				if (weapon.CanTargetClassOfUnit(unit)) {
					return true;
				}
			}
			//return true;
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

	public void WasFiredOnByWeapon(Weapon weapon) {
		ConsiderTarget(weapon.owner);
	}

	public virtual void ConsiderTarget(GameObject obj) {
		if (!obj.IsDestroyed()) {
			foreach (Weapon weapon in Weapons()) {
				weapon.ConsiderTarget(obj);
			}
		}
	}

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

	bool isQuitting = false;
	void OnApplicationQuit() {
		isQuitting = true;
	}

	void OnDestroy() {
		if (!isQuitting) {
			ShowFxExplosion();
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
