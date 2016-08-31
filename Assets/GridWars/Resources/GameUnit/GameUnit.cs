using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameUnit : NetworkObject {
	public float thrust;
	public float rotationThrust;

	public Player player {
		get {
			if (gameUnitState.playerNumber > 0) {
				return Battlefield.current.PlayerNumbered(gameUnitState.playerNumber);
			}
			else {
				return null;
			}
		}

		set {
			if (value == null) {
				gameUnitState.playerNumber = 0;
			}
			else {
				gameUnitState.playerNumber = value.playerNumber;
				value.TakeControlOf(this);
			}
		}
	}

	public int playerNumber {
		get {
			return gameUnitState.playerNumber;
		}
	}
		
	// Damagable
	public float hitPoints {
		get {
			return gameUnitState.hitPoints;
		}

		set {
			gameUnitState.hitPoints = value;
		}
	}

	public bool isAlive {
		get {
			return gameUnitState.isAlive;
		}

		set {
			gameUnitState.isAlive = value;
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

	public void BecomeIcon() {
		isTargetable = false;

		deathExplosionPrefab = null;

		Destroy(GetComponent<Collider>());
		Destroy(GetComponent<Rigidbody>());
		gameObject.DeepRemoveScripts();
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

	protected override void Awake() {
		base.Awake();

		isTargetable = true;
	}

	//Networking

	public bool shouldDestroyColliderOnClient = true;
	protected List<Bolt.Event> playerCommands;

	public IGameUnitState gameUnitState {
		get {
			return entity.GetState<IGameUnitState>();
		}
	}

	public GameUnit Instantiate() {
		return BoltNetwork.Instantiate(gameObject).GetComponent<GameUnit>();
	}

	public static GameUnit Instantiate(System.Type unitType) {
		return Load(unitType).GetComponent<GameUnit>().Instantiate();
	}

	public static T Instantiate<T>() where T: GameUnit {
		return (T) Instantiate(typeof(T));
	}

	public override void MasterInit() {
		base.MasterInit();
		hitPoints = maxHitPoints;
	}

	public void HideAndDisable() {
		//Debug.Log(this + " HideAndDisable");
		SetVisibleAndEnabled(false);
	}

	public void ShowAndEnable() {
		//Debug.Log(this + " ShowAndEnable");
		SetVisibleAndEnabled(true);
	}

	void SetVisibleAndEnabled(bool visibleAndEnabled) {
		foreach (var script in GetComponentsInChildren<MonoBehaviour>()) {
			if (script.GetType() != typeof(BoltEntity)) {
				script.enabled = visibleAndEnabled;
			}
		}

		foreach (var renderer in GetComponentsInChildren<ParticleSystemRenderer>()) {
			renderer.enabled = visibleAndEnabled;
		}

		gameObject.EachRenderer(r => r.enabled = visibleAndEnabled);

		var collider = GetComponent<Collider>();
		if (collider != null) {
			collider.enabled = visibleAndEnabled;
		}

		var rigidBody = GetComponent<Rigidbody>();
		if (rigidBody != null) {
			rigidBody.isKinematic = !visibleAndEnabled;
		}
	}

	public override void MasterSlaveStart() {
		base.MasterSlaveStart();

		gameUnitState.SetTransforms(gameUnitState.transform, transform);

		if (typeof(ITurretedUnitState).IsAssignableFrom(gameUnitState.GetType())) {
			var s = entity.GetState<ITurretedUnitState>();
			//TODO: this won't work for more than 1 weapon
			foreach (var weapon in Weapons()) {
				if (weapon.turretObjX != null) {
					s.SetTransforms(s.turretXTransform, weapon.turretObjX.transform);
				}

				if (weapon.turretObjY != null) {
					s.SetTransforms(s.turretYTransform, weapon.turretObjY.transform);
				}
			}
		}

		playerCommands = new List<Bolt.Event>();
	}

	public override void MasterStart() {
		gameUnitState.isAlive = true;

		base.MasterStart();
	}

	public override void ClientStart() {
		base.ClientStart();
	}

	public override void SlaveStart() {
		base.SlaveStart();
		SetupWeapons();
		SetupSmokeDamage ();
		SetupDeathExplosion ();

		gameObject.CloneMaterials();

		if (player == null) {
			gameObject.Paint(Color.white, "Unit");
		} else {
			player.AddGameObject(gameObject); // hack because player setter is never called
			player.Paint(gameObject);
		}

		PlayBirthSound();
	}

	public float nextThinkTime;

	public bool IsThinkStep() {
		float waitSeconds = (1f / 20f);
		if (Time.time > nextThinkTime) {
			nextThinkTime = Time.time + (waitSeconds * 2f * UnityEngine.Random.value);
			return true;
		}
		/*
		return (App.shared.timeCounter % 20 == 0);
		float chancePerSec = 1f / 40f;
		float secsInLastStep = Time.deltaTime;
		return (UnityEngine.Random.value < chancePerSec * secsInLastStep);
		*/
		return false;
	}

	public virtual void Think() {
		PickTarget();
	}

	public override void MasterFixedUpdate(){
		base.MasterFixedUpdate();
		/*
		if (player == null) {
			print ("SimulateOwner null player on " + this);
		}
		*/

		if (IsThinkStep()) {
			Think();
		}


		foreach (var weapon in Weapons()) {
			if (weapon.isActiveAndEnabled) {
				weapon.SimulateOwner();
			}
		}

		RemoveIfOutOfBounds ();
	}

	public override void SlaveFixedUpdate(){
		base.SlaveFixedUpdate();
	}

	public override void QueuePlayerCommands(){}

	public override void SlaveDied(){
		base.SlaveDied();
		ShowFxExplosion();
	}


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
		return App.shared.stepCache.ActiveGameObjects();
	}

	/*
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
	*/

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
		return player.EnemyObjects();
	}


	public virtual List <GameObject> NonAirEnemyVehicles() {
		var results = new List<GameObject>();

		foreach (GameObject enemy in EnemyObjects()) {
			if ( !enemy.GameUnit().IsOfType(typeof(AirVehicle)) ) {
				results.Add(enemy);
			}
		}
		return results;
	}

	public virtual List <GameObject> EnemyBuildings() {
		var results = new List<GameObject>();

		foreach (GameObject enemy in EnemyObjects()) {
			if (enemy.GameUnit().IsOfType(typeof(GroundBuilding)) ) {
				results.Add(enemy);
			}
		}
		return results;
	}

	/*
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
		if (target != null && target.IsDestroyed()) {
			target = null;
		}

		if (Weapons().Length > 0) {
			Weapon targetingWeapon = HighestPriorityWeaponWithTarget();
			if (targetingWeapon) {
				target = targetingWeapon.target;
			}
		}

		if (target == null) {
			target = DefaultTarget();
		} 
	}

	public virtual GameObject DefaultTarget() {
		return ClosestEnemyObject();
	}
		

	public virtual GameObject ClosestEnemyObject() {
		return ClosestOfObjects(EnemyObjects());
	}

	public virtual GameObject ClosestOfObjects( List<GameObject> objs) {
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
			(y() < -3) || (y() > 150) ||
			(x() > 150) || (x() > 150) ||
			(z() > 150) || (z() > 150) 
		);
	}

	public virtual void RemoveIfOutOfBounds () {
		if (isOutOfBounds() ) {
			OnDead();
		}
	}


	// --- aiming --------------------

	public float YAngleToTarget() {
		var targetPos = target.transform.position;
		Vector3 targetDir = (targetPos - _t.position).normalized;
		float angle = AngleBetweenOnAxis(_t.forward, targetDir, _t.up);
		return angle;
	}

	public float targetDistance() {
		return Vector3.Distance(transform.position, target.transform.position);
	}

	public virtual void OnCollisionEnter(Collision collision) {
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

	public void DestroySelf() {
		if (!isActiveAndEnabled) {
			return;
		}

		gameUnitState.isAlive = false;

		//Debug.Log("App.shared.AddToDestroyQueue(gameObject); " + gameObject);

		if (player) {
			player.RemoveGameObject(gameObject);
		}

		App.shared.AddToDestroyQueue(gameObject);
	}

	public void ActuallyDestroySelf() {
		var timer = App.shared.timerCenter.NewTimer().SetTimeout(3*1f/20); //TODO use frames instead
		timer.action = NetworkDestroySelf;
		timer.Start();
	}

	void NetworkDestroySelf() {
		BoltNetwork.Destroy(gameObject);
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

		if (gameObject.IsDestroyed()) {
			return;
		}

		hitPoints -= damage;
		if (smokeDamage != null) {
			float max = 100f;
			smokeDamage.maxParticles = (int)(max * (1 - (hitPoints / maxHitPoints)));
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
			var unitExplosion = deathExplosionPrefab.GameUnit();
			if (unitExplosion != null) {
				var explosion = deathExplosionPrefab.GetComponent<Explosion>().Instantiate();
				explosion.transform.position = _t.position;
				explosion.transform.rotation = _t.rotation;
			}
		}
	}

	public void ShowFxExplosion() {
		if (deathExplosionPrefab != null) {
			var unitExplosion = deathExplosionPrefab.GameUnit();
			if (unitExplosion == null) {
				var obj = Instantiate(deathExplosionPrefab);
				obj.transform.position = _t.position;
				obj.transform.rotation = _t.rotation;
			}
		}
		
	}
		
	// --- Weapons ------------------------------------------

	Weapon[] _weapons;

	public Weapon[] Weapons() {
		if (_weapons == null) {
			_weapons = GetComponentsInChildren<Weapon>();
		}
		return _weapons;
	}

	public void SetupWeapons() {
		foreach (Weapon weapon in Weapons()) {
			weapon.owner = gameObject;
			weapon.enabled = true;
		}
	}

	public void DeactivateWeapons() {
		foreach (Weapon weapon in Weapons()) {
			weapon.isActive = false;
			weapon.enabled = false;
		}
	}

	// --- AI eval ------------------------------------------

	public float evalPosition(Vector3 pos) {
		// higher to be closer to targetable objects
		// diff weights for targets

		// lower to be too close to obsticles
		return 0f;

	}


}
