using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using AssemblyCSharp;

public class GameUnit : NetworkObject {
	public float thrust;
	public float rotationThrust;
	public float birthVolume = 1;
	public float deathVolume = 1;
	public Color paintedColor;
	//public bool allowFriendlyFire = true;
	//public AssemblyCSharp.TimerCenter timerCenterForServer; // use these timers to do mutations
	//public AssemblyCSharp.TimerCenter timerCenterForClient; // use these timers for fx

	public AudioClip deathSound;


	[HideInInspector]
	public bool showsUnitExplosion = true;

	public Player player {
		get {
			if (gameUnitState == null) {
				return null;
			}
			else if (gameUnitState.playerNumber > 0) {
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
				if (BoltNetwork.isServer) {
					value.TakeControlOf(this);
				}
			}
		}
	}

	public int playerNumber {
		get {
			return gameUnitState.playerNumber;
		}
	}

	public bool shouldAddToPlayerUnits = true;
		
	// Damagable
	public float hitPoints {
		get {
			return gameUnitState.hitPoints;
		}

		set {
			gameUnitState.hitPoints = value;
		}
	}

	public float maxHitPoints {
		get {
			return gameUnitState.maxHitPoints;
		}

		set {
			gameUnitState.maxHitPoints = value;
		}
	}

	public bool isInGame {
		get {
			return gameUnitState.isInGame;
		}

		set {
			gameUnitState.isInGame = value;
		}
	}


	[HideInInspector]
	public bool isTargetable;
	public bool isRunning = true;

	public GameObject target = null;
	//public float angleToTarget = 0;

	public bool isStaticUnit = false;

	//tower
	public float[] powerCostPerLevel = new float[] { float.MaxValue, float.MaxValue, float.MaxValue };
	public float cooldownSeconds = 1f;
	public float standOffDistance = 20f;
	public Vector3 launchDirection = Vector3.forward;
	public ReleaseZone releaseZone;

	public float PowerCost(int veteranLevel) {
		return powerCostPerLevel[veteranLevel];
	}

	//FX

	public float fadeInPeriod = 0.3f;
	protected bool isPlayerPainted = true;

	public float hpRatio {
		get {
			return hitPoints/maxHitPoints;
		}
	}

	public void BecomeIcon() {
		isTargetable = false;

		deathExplosionPrefab = null;

		leftGameCalled = true; //otherwise NetworkObject will raise an exception

		Destroy(GetComponent<Collider>());
		Destroy(GetComponent<Rigidbody>());
		gameObject.DeepRemoveScripts();
	}

	// -- vet sound ---------

	public AudioClip vet1Sound {
		get {
			return SoundNamed("vet1");
		}
	}

	public AudioClip vet2Sound {
		get {
			return SoundNamed("vet2");
		}
	}


	// --- Running Sound ------------------------------------------

	AudioSource _runningAudioSource;
	protected AudioSource runningAudioSource {
		get {
			if (_runningAudioSource == null) {
				_runningAudioSource = gameObject.AddComponent<AudioSource>();
				_runningAudioSource.loop = true;
				_runningAudioSource.spatialize = true;
				//_runningAudioSource.volume = 0.1f;

				_runningAudioSource.maxDistance = 20f;
				_runningAudioSource.minDistance = 1f;

				//_runningAudioSource.minVolume = 1f;
				_runningAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
			}
			return _runningAudioSource;
		}
	}

	public AudioClip runningSound {
		get {
			return SoundNamed("running");
		}
	}

	protected void PlayRunningSound() {
		if ((runningSound != null) && (!runningAudioSource.isPlaying)) {
			runningAudioSource.clip = runningSound;
			runningAudioSource.Play(0);
		}
	}

	public void SetRunningSoundPitch(float p) {
		if (runningSound != null) {
			runningAudioSource.pitch = p;
		}
	}

	private float lastCameraDistance = 0f;

	public void UpdateSpatialSounds() {
		AudioSource a = _runningAudioSource;
		if (a != null) {
			Vector3 cp = Camera.main.transform.position;
			float d = Vector3.Distance(cp, _t.position);
			float dr = (Mathf.Clamp(d, a.minDistance, a.maxDistance) - a.minDistance) / a.maxDistance;
			float minVolume = 0.04f;
			float maxVolume = 1f;
			a.volume = minVolume + (maxVolume - minVolume) * (1f - dr);
			if (lastCameraDistance != 0f) {
				float v = Mathf.Abs(d - lastCameraDistance);
				float speedOfSound = 10f;
				float p = speedOfSound / Math.Abs(speedOfSound - Mathf.Clamp(v, 0, 1));
				_runningAudioSource.pitch = p;
			}
			lastCameraDistance = d;
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
		
	// Birth

	public AudioClip birthSound {
		get {
			return SoundNamed("birth");
		}
	}
		
	protected void PlayBirthSound() {
		if (birthSound != null) {
			audioSource.PlayOneShot(birthSound, birthVolume);
		}
	}


	protected void PlayDeathSound() {
		if (deathSound != null) {
			App.shared.PlayOneShot(deathSound, deathVolume);
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
		return App.shared.LoadAudioClip(soundPath);
		//return Resources.Load<AudioClip>(soundPath);
	}

	public void PlaySoundNamed(string name, float volume) {
		audioSource.PlayOneShot(SoundNamed(name), volume);
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

	public override void Awake() {
		base.Awake();
		launchDirection = Vector3.forward;
		isTargetable = true;
	}

	// Cache

	public string cacheKey {
		get {
			return entity.prefabId.ToString();
		}
	}

	public GameUnitCache cache {
		get {
			return App.shared.battlefield.gameUnitCache;
		}
	}

	//Networking

	public bool shouldDestroyColliderOnClient = true;
	protected List<Bolt.Event> playerCommands;

	IGameUnitState _gameUnitState;

	public IGameUnitState gameUnitState;

	public virtual GameUnit Instantiate() {
		/*
		var unit = cache.ForKeyPop(cacheKey);
		if (unit == null) {
		*/
			return BoltNetwork.Instantiate(gameObject).GetComponent<GameUnit>();
		/*
		}
		else {
			unit.entity.Freeze(false);
			unit.GetComponent<NetworkedGameUnit>().Attached();
			return unit;
		}
		*/
	}

	public static GameUnit Instantiate(System.Type unitType) {
		return Load(unitType).GetComponent<GameUnit>().Instantiate();
	}

	public static T Instantiate<T>() where T: GameUnit {
		return (T) Instantiate(typeof(T));
	}

	void IsInGameChanged() {
		//App.shared.Log("IsInGameChanged: " + isInGame, this);
		if (!isInGame) {
			DidLeaveGame();
			SetVisibleAndEnabled(false);
		}
	}


	//NetworkObject

	bool serverAndClientJoinedGame = false;

	public int instanceId;

	public override void ServerInit() {
		base.ServerInit();
		//App.shared.Log("ServerInit", this);
		isInGame = true;

		//this is the only way to set a default due to Bolt constraints.
		if (maxHitPoints == 0f) {
			maxHitPoints = 1f;
		}
		hitPoints = maxHitPoints;

		thinkThrottle = new Throttle();
		thinkThrottle.behaviour = this;
		thinkThrottle.period = 25;
	}

	public override void ClientInit() {
		base.ClientInit();
		Destroy(GetComponent<Rigidbody>());

		if (shouldDestroyColliderOnClient) {
			Destroy(GetComponent<Collider>());
		}
	}

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();

		instanceId = GetInstanceID();

		serverAndClientJoinedGame = false;

		gameUnitState.AddCallback("isInGame", IsInGameChanged);
		gameUnitState.AddCallback("veteranLevel", VeteranLevelChanged);

		foreach(var weapon in Weapons()) {
			weapon.ServerAndClientInit();
		}
			
		SetVisibleAndEnabled(false);
	}

	public override void ServerJoinedGame() {
		base.ServerJoinedGame();

		//App.shared.Log("ServerJoinedGame", this);

		SetVisibleAndEnabled(true); //Don't do this in ServerAndClientJoinedGame as some classes need it setup here
		gameUnitState.isInGame = true;

		if (releaseZone != null) {
			releaseZone.hiddenUnit = null;
		}

	}

	public override void ClientJoinedGame() {
		base.ClientJoinedGame();
		SetVisibleAndEnabled(true); //Don't do this in ServerAndClientJoinedGame as some classes need it setup here
	}

	public override void ServerAndClientJoinedGame() {
		base.ServerAndClientJoinedGame();

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

		SetupWeapons();
		SetupSmokeDamage ();
		SetupDeathExplosion ();

		if (player != null) {
			if (shouldAddToPlayerUnits) {
				player.units.Add(this);
			}

			if (isPlayerPainted) {
				//App.shared.Log("player.Paint()", this);
				player.Paint(gameObject);
			}
		}

		PlayBirthSound();
		PlayRunningSound();

		if (fadeInPeriod != 0f) {
			//BrightFadeIn comp = gameObject.AddComponent<BrightFadeIn>();
			BrightFadeInGeneric comp = gameObject.AddComponent<BrightFadeInGeneric>();
			comp.period = fadeInPeriod;
		}

		serverAndClientJoinedGame = true;

		if (veteranLevel > 0) {
			ShowVeteranLevel();
		}
	}

	public override void ServerFixedUpdate(){
		base.ServerFixedUpdate();

		hitPoints += Time.deltaTime * hitPointRegenRate;
		hitPoints = Mathf.Clamp(hitPoints, 0, maxHitPoints);

		if (thinkThrottle.isOff) {
			Think();
		}

		foreach (var weapon in Weapons()) {
			if (weapon.isActiveAndEnabled) {
				weapon.ServerFixedUpdate();
			}
		}
			
		RemoveIfOutOfBounds ();
	}
		
	public override void ClientFixedUpdate(){
		base.ClientFixedUpdate();
	}

	public override void ServerAndClientFixedUpdate() {
		base.ServerAndClientFixedUpdate();
		UpdateSpatialSounds();
	}

	public override void ServerUpdate() {
		base.ServerUpdate();
	}

	public override void ServerAndClientUpdate() {
		base.ServerAndClientUpdate();

		QueuePlayerCommands();

		if (smokeDamage != null) {
			float max = 50f;
			smokeDamage.maxParticles = (int)(max * (1 - (hitPoints / maxHitPoints)));
		}
	}

	public override void ServerLeftGame() {
		base.ServerLeftGame();
	}

	public override void ClientLeftGame() {
		base.ClientLeftGame();
	}

	public override void ServerAndClientLeftGame(){
		base.ServerAndClientLeftGame();
		if (player != null) {
			player.units.Remove(this);

			//Don't explode when units are removed at the end of the game
			if (entity.isAttached && player.isInGame) {
				ShowFxExplosion();
				PlayDeathSound();
			}
		}

		ResetFirstPersonCamera();
	}

	protected override void OnDestroy() {
		base.OnDestroy();

		//App.shared.Log("OnDestroy: " + (destroySelfTimer == null).ToString(), this);

		if (destroySelfTimer != null) {
			destroySelfTimer.Cancel();
		}
	}

	// Thinking

	Throttle thinkThrottle;

	public virtual void Think() {
		PickTarget();
	}


	// -----------------------

	public virtual void QueuePlayerCommands(){}

	public void SetVisibleAndEnabled(bool visibleAndEnabled) {
		//App.shared.Log("SetVisibleAndEnabled(" + visibleAndEnabled + ")", this);
		foreach (var script in GetComponentsInChildren<MonoBehaviour>()) {
			if (script.GetType() != typeof(BoltEntity) && !script.inheritsFrom(typeof(NetworkedGameUnit))) {
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

		return target && (TargetDistance() < standOffDistance);
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

	public bool IsDead() {
		return hitPoints <= 0f;
	}

	// --- veterancy ----------------------------

	public int killCount = 0;
	public int killsPerVeteranLevel = 3;

	public int veteranLevel {
		get {
			if (serverAndClientJoinedGame) {
				return gameUnitState.veteranLevel;
			}
			else {
				return 0;
			}

		}

		set {
			gameUnitState.veteranLevel = value;
		}
	}


	private int maxVeteranLevel = 2;
	public float hitPointRegenRate = 0.3f; // in hp per second

	public void DidKill(GameUnit otherUnit) {
		killCount++;

		if ((killCount % killsPerVeteranLevel) == 0) {
			if (veteranLevel < maxVeteranLevel) {
				UpgradeVeterancy();
			}
		}
	}

	public virtual void UpgradeVeterancy() {
		veteranLevel++;
	}

	void VeteranLevelChanged() {
		if (veteranLevel != 0) {
			ShowVeteranLevel();
		}
	}

	public void ShowVeteranLevel() {
		if (!serverAndClientJoinedGame) {
			return;
		}

		if (veteranLevel == 1) {
			App.shared.PlayOneShot(vet1Sound, 1f);
		} else {
			App.shared.PlayOneShot(vet2Sound, 1f);
		}

		// cycler setup --------------

		// remove brightfade if present
		var brightFade = GetComponent<BrightFadeInGeneric>();
		if (brightFade != null) {
			brightFade.enabled = false;
			Destroy(brightFade);
		}

		// ------------------------------------

		if (veteranLevel == 1) {
			Color darkPrimaryColor = Color.Lerp(player.primaryColor, Color.black, 0.35f);
			PaintPrimaryColor(darkPrimaryColor);
			PaintSecondaryColor(darkPrimaryColor);
		} else if (veteranLevel == 2) {
			PaintPrimaryColor(Color.black);
			PaintSecondaryColor(player.primaryColor);
		}

		ColorCycleInFX cycler = gameObject.GetComponent<ColorCycleInFX>();
		if (cycler != null) {
			cycler.enabled = false;
			Destroy(cycler);
		}
		cycler = gameObject.AddComponent<ColorCycleInFX>();
		cycler.OnEnable();
	}

	// --- Vet Upgrading Stats Helpers ---

	public void AdjustScaleByFactor(float s) {
		gameObject.transform.localScale = new Vector3(s, s, s);
	}

	public void AdjustWeaponsRangeByFactor(float f) {
		foreach (Weapon w in Weapons()) {
			w.range *= f;
			w.rangeMultiplier *= f;
		}
		standOffDistance *= f;
	}

	public void AdjustWeaponsDamageByFactor(float f) {
		foreach (Weapon w in Weapons()) {
			w.damageMultiplier *= f;
		}
	}

	public void AdjustWeaponsFireRateByFactor(float f) {
		foreach (Weapon w in Weapons()) {
			w.reloadTimeInSeconds *= 1f/f;
		}
	}

	public void AdjustMaxHitpointsByFactor(float f) {
		maxHitPoints *= f;
		hitPoints += maxHitPoints * 0.5f; // supcom does 0.25
		hitPoints = Mathf.Clamp(hitPoints, 0f, maxHitPoints);
	}

	public void AdjustHitPointGenByFactor(float f) {
		hitPointRegenRate *= f;
	}
	// --- Painting ----

	public void PaintPrimaryColor(Color c) {
		gameObject.EachRenderer(r => {
			if (r.material.name.StartsWith("PrimaryColor")) {
				r.material.color = c;
			}
		});
	}

	public void PaintSecondaryColor(Color c) {
		gameObject.EachRenderer(r => {
			if (r.material.name.StartsWith("SecondaryColor")) {
				r.material.color = c;
			}
		});
	}

	// -------------------------------------

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

	public float RatioOfDistanceToEnemyFortress() {
		float fd = player.fortress.DistanceToEnemyFortress();
		return DistanceToEnemyFortress() / fd; 
	}

	public float DistanceToEnemyFortress() {
		foreach (var enemyPlayer in player.enemyPlayers) {
			return Vector3.Distance(enemyPlayer.fortress.transform.position, transform.position);
		}

		return -1; 
	}

	public float DistanceToObj(GameObject obj) {
		// please do not change this to sqrMagnitude
		return Vector3.Distance(obj.transform.position, transform.position);
	}

	public virtual List<GameObject> EnemyObjects() {
		return player.EnemyObjects();
	}

	public virtual List <GameUnit> EnemyUnits() {
		var results = new List<GameUnit>();

		foreach (GameObject enemy in EnemyObjects()) {
			results.Add(enemy.GameUnit());
		}
		return results;
	}

	public virtual List <GameObject> EnemyBuildingUnits() {
		var results = new List<GameObject>();

		foreach (GameObject enemy in EnemyObjects()) {
			GameUnit gu = enemy.GameUnit();
			if ( gu.IsOfType(typeof(GroundBuilding))) {
				results.Add(enemy);
			}
		}
		return results;
	}


	public virtual List <GameObject> EnemyNonAirUnits() {
		var results = new List<GameObject>();

		foreach (GameObject enemy in EnemyObjects()) {
			GameUnit gu = enemy.GameUnit();
			if ( !gu.IsOfType(typeof(AirVehicle)) 
				&& !gu.IsOfType(typeof(Projectile))) {
				results.Add(enemy);
			}
		}
		return results;
	}

	public virtual List <GameObject> EnemyAirUnits() {
		var results = new List<GameObject>();

		foreach (GameObject enemy in EnemyObjects()) {
			GameUnit gu = enemy.GameUnit();
			if ( gu.IsOfType(typeof(AirVehicle)) ) {
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
			/*
			if (target == null) {
				target = Camera.main.gameObject;
			}
			*/
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
			Die();
		}
	}


	// --- aiming --------------------

	public float YAngleToTarget() {
		var targetPos = target.transform.position;
		Vector3 targetDir = (targetPos - _t.position).normalized;
		float angle = AngleBetweenOnAxis(_t.forward, targetDir, _t.up);
		return angle;
	}

	public float TargetDistance() {
		return Vector3.Distance(transform.position, target.transform.position);
	}

	public virtual void OnCollisionEnter(Collision collision) {
	}

	// --- icons --------------------

	public virtual void OnDrawGizmos() {
		if (target != null) {
			Gizmos.color = Color.green;
			//Gizmos.DrawLine (_t.position, target.transform.position);
		}
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
		if (!isInGame) {
			return;
		}

		if (gameObject.IsDestroyed()) {
			return;
		}

		hitPoints -= damage;
		if (hitPoints <= 0) {
			Die();
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
		string s = App.shared.ResourcePathForUnitType(GetType()) + "/Prefabs/DeathExplosion";
		deathExplosionPrefab = App.shared.LoadGameObject(s);
	}


	// --- Death ------------------------------------------

	public virtual void Die() { // should only be called on Server.
		if (!BoltNetwork.isServer) {
			Debug.LogWarning(this + " Die called on client!");
			return;
		}

		if (isInGame) {
			//App.shared.Log("Die", this);

			//Debug.Log("App.shared.AddToDestroyQueue(gameObject); " + gameObject);

			RemoveFromGame();
		}
	}

	Timer destroySelfTimer;
	public virtual void RemoveFromGame() {
		if (!isInGame) {
			return;
		}

		destroySelfTimer = App.shared.timerCenter.NewTimer();
		destroySelfTimer.timeout = 6*1f/20; //wait 6 network updates to be sure client gets updated
		destroySelfTimer.action = DestroySelf;
		destroySelfTimer.Start();

		//App.shared.Log("RemoveFromGame", this);

		isInGame = false;
	}

	public virtual void DestroySelf() {
		//Debug.Log("Add to Cache");
		/*
		if (cache.ForKeyPush(cacheKey, this)) {
			gameUnitState.RemoveAllCallbacks();
			entity.Freeze(true);
		}
		else {
		*/
			//App.shared.Log("DestroySelf", this);
			BoltNetwork.Destroy(gameObject);

			foreach (var comp in gameObject.GetComponents<AudioSource>())
			{
				Destroy(comp);
			}
		//}
	}

	void ResetFirstPersonCamera() {
		Camera cam = _t.GetComponentInChildren<Camera>();
		if (cam) {
			App.shared.cameraController.ResetCamera();
		}
	}

	public virtual GameObject ShowFxExplosion() {
		if (showsUnitExplosion && deathExplosionPrefab != null) {
			var unitExplosion = deathExplosionPrefab.GameUnit();
			if (unitExplosion == null) {
				var obj = Instantiate(deathExplosionPrefab);
				obj.transform.position = _t.position;
				obj.transform.rotation = _t.rotation;
				return obj;
			}
		}
		return null;
	}
		

	// --- Weapons ------------------------------------------

	Weapon[] _weapons;

	public Weapon[] Weapons() {
		if (_weapons == null) {
			_weapons = GetComponentsInChildren<Weapon>();
		}
		return _weapons;
	}

	virtual public bool HasWeapons() {
		return Weapons().Length > 0;
	}

	public void SetupWeapons() {
		foreach (Weapon weapon in Weapons()) {
			weapon.owner = gameObject;
			weapon.enabled = true;
			weapon.player = player;
			//weapon.allowFriendlyFire = allowFriendlyFire;
		}
	}

	public void SetAllowFriendlyFire(bool v) {
		foreach (Weapon weapon in Weapons()) {
			weapon.allowFriendlyFire = v;
		}
	}

	public void DeactivateWeapons() {
		foreach (Weapon weapon in Weapons()) {
			weapon.isActive = false;
			weapon.enabled = false;
		}
	}

	// --- AI eval ------------------------------------------

	/*
	public float evalPosition(Vector3 pos) {
		// higher to be closer to targetable objects
		// diff weights for targets

		// lower to be too close to obsticles
		return 0f;
	}
	*/


	public Vector3 ExpectedPositionAfterTime(float leadTime) {
		Vector3 pos = target.GameUnit().ColliderCenter();
		Rigidbody rb = target.GetComponent<Rigidbody>();

		if (rb) {
			return pos + rb.velocity * leadTime;
		}

		return pos;
	}


	// -------------------------------------


	public virtual List<System.Type> CountersTypes() {
		List<System.Type> counters = new List<System.Type>();
		return counters;
	}
}
