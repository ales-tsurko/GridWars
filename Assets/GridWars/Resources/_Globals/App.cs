using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.Analytics;
/*
 *  This is a singleton. Access like this:
 * 
 *  App.shared.ResourcePathForUnitType(typeof(Tank))
 * 
 */

public class App : MonoBehaviour, AppStateOwner {
	//test github push

	private static App _shared;
	public AssemblyCSharp.TimerCenter timerCenter;
	public int timeCounter = 0;
	public AssemblyCSharp.StepCache stepCache;
	private List <GameObject> _destroyQueue;
	public bool debug = false;
	public List <Soundtrack> soundtracks;
	public Prefs prefs;
	public AppState state { get; set; }
	public UIMenu menu;
	public NotificationCenter notificationCenter;

	public Matchmaker matchmaker;
	public Network network;
	public Battlefield battlefield;
	public CameraController cameraController;
	public PlayerInputs inputs; //used outside of games
	public Account account;
	public ExceptionReporter exceptionReporter;
	public bool isExiting;

	EnvConfig _config;
	public EnvConfig config {
		get {
			if (_config == null) {
				_config = new EnvController().config;
				//Debug.Log(_config.name);
			}
			return _config;
		}
	}


	public string version {
		get {
			return Resources.Load<TextAsset>("version").text;
		}
	}

	private bool _isProcessingDestroyQueue = false;

	public bool testEndOfGameMode {
		get {
			return config.testEndOfGameMode;
		}
	}

	public static App shared {
		get {
			if (_shared == null) {
				GameObject go = new GameObject();
				go.name = "App";
				_shared = go.AddComponent<App>();
			}
			return _shared;
		}
	}

	// --- MonoBehaviour -------------------
    public EnvController envController;
	public void Awake() {
		timerCenter = new AssemblyCSharp.TimerCenter();

	}

	public void Start() {
		exceptionReporter = new ExceptionReporter();
		if (config.name == "Release") {
			Application.logMessageReceived += HandleException;
		}

		debug = false;
		Application.runInBackground = true;
		Profiler.maxNumberOfSamplesPerFrame = 1048576; //Unity bug

		notificationCenter = new NotificationCenter();

		menu = UI.Menu();
		notificationCenter.NewObservation()
			.SetNotificationName(UIMenu.UIMenuShowedNotification)
			.SetSender(menu)
			.SetAction(MenuDidShow)
			.Add();

		prefs = new Prefs();
		prefs.Load();

        SetupResolution();
		stepCache = new AssemblyCSharp.StepCache();
		_destroyQueue = new List<GameObject>();
		soundtracks = new List<Soundtrack>();

		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;

		Wreckage.SetupLayerCollisions();
		Projectile.SetupLayerCollisions();

		account = new Account();
        account.LogEvent("AppStarted");

		matchmaker = GameObject.Find("Matchmaker").GetComponent<Matchmaker>();
		matchmaker.Setup();
		matchmaker.menu.Hide();

		network = new GameObject().AddComponent<Network>();
		network.gameObject.name = "Network";

		battlefield = GameObject.Find("Battlefield").GetComponent<Battlefield>();
		battlefield.AddPlayers();

		cameraController = GameObject.FindObjectOfType<CameraController>();
		cameraController.enabled = true;

		inputs = new PlayerInputs();
		inputs.AddControllerBindings();
		inputs.AddLocalPlayer1KeyBindings();

		var mainMenuState = new MainMenuState();
		mainMenuState.owner = this;
		this.state = mainMenuState;
		mainMenuState.EnterFrom(null);
	}

	void HandleException(string message, string stackTrace, LogType type) {
		if (type == LogType.Exception || type == LogType.Error) {
			exceptionReporter.ReportException(message, stackTrace);
		}
	}

	void MenuDidShow(Notification n) {
		//Debug.Log("MenuDidShow");
		matchmaker.menu.Close();
	}

    void SetupResolution() {
		if (new List<Resolution>(Screen.resolutions).Contains(prefs.resolution)) {
			Screen.SetResolution(prefs.resolution.width, prefs.resolution.height, true); // last arg is bool for fullscreen
		}
		else {
			var resolution = Screen.resolutions[Screen.resolutions.Length - 1];
			Screen.SetResolution(resolution.width, resolution.height, true);
			prefs.resolution = resolution;
		}

		QualitySettings.antiAliasing = prefs.antialiasingLevel;
    }

	public void FixedUpdate() {
		timerCenter.Step();
		stepCache.Step();
		timeCounter++;
	}

	void Update() {
        if (state != null) {
            state.Update();
        }
		ShowHideCursor();
	}

	float mouseMoveTime = 0f;
	float mouseIdleTime = 3f;
	void ShowHideCursor() {
		if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
			mouseMoveTime = Time.time;
			Cursor.visible = true;
		}
		if (Time.time - mouseMoveTime > mouseIdleTime) {
			Cursor.visible = false;
		}
	}

	// --- Menu --------------------

	public void ResetMenu() {
		/*
		if (menu != null) {
			menu.Destroy();
		}
		menu = UI.Menu();
		*/
		menu.Reset();
		menu.Hide();
	}

	// --- Finding Paths --------------------
	// should really have the Types themselve know how to find themselves but
	// class methods don't have access to the type on which they were called -
	// they only know the type in which they are declared

	public string ResourcePathForUnitType(System.Type type) {
		List <string> pathComponents = new List<string>();

		while (type != typeof(GameUnit)) {
			pathComponents.Add(type.Name);
			type = type.BaseType;
		}

		pathComponents.Add("GameUnit"); // add GameUnit
		pathComponents.Reverse();
		return string.Join("/", pathComponents.ToArray());
	}

	public string PrefabPathForUnitType(System.Type type) {
		string path = ResourcePathForUnitType(type);
		return path + "/Prefabs/" + type.Name;
	}

	public string SoundPathForUnitType(System.Type type) {
		string path = ResourcePathForUnitType(type);
		string soundPath = path + "/Sounds/";
		return soundPath;
	}

	public AudioClip SoundNamedForUnitType(string name, System.Type type) {
		string path = ResourcePathForUnitType(type);
		string soundPath = path + "/Sounds/" + name;
		return Resources.Load<AudioClip>(soundPath);
	}

	// --- global audio ----

	AudioSource _audioSource;
	protected AudioSource audioSource {
		get {
			if (_audioSource == null) {
				_audioSource = gameObject.AddComponent<AudioSource>();
				_audioSource.loop = false;
				_audioSource.spatialize = false;
			}
			return _audioSource;
		}
	}
		
	public void PlayOneShot(AudioClip clip, float volume) {
		audioSource.PlayOneShot(clip, volume);
	}

	public void PlayAppSoundNamed(string soundName) {
		PlayAppSoundNamedAtVolume(soundName, 1f);
	}

	public void PlayAppSoundNamedAtVolume(string soundName, float v) {
		string soundPath = "Sounds/" + soundName;
		AudioClip clip = Resources.Load<AudioClip>(soundPath);
		PlayOneShot(clip, v);
	}

	// --- Destroying Objects -----------

	void LateUpdate() {
		ProcessDestroyQueue();
	}


	public void AddToDestroyQueue(GameObject obj) {
		//ThrowIfQueuedToDestroyOrAlreadyDestroyed(obj);

		if(!_destroyQueue.Contains(obj)) {
			_destroyQueue.Add(obj);
		}
	}


	public void ProcessDestroyQueue() {
		if (_isProcessingDestroyQueue) {
			throw new System.InvalidOperationException("circular call of ProcessDestroyQueue");
		}

		_isProcessingDestroyQueue = true;

		foreach(GameObject obj in _destroyQueue) {
			if (obj == null) { 
				//throw new System.InvalidOperationException("found already destroyed gameobject in destroy queue");
				print("WARNING: found already destroyed gameobject in destroy queue");
			} else {
				Destroy(obj);
			}
		}

		_destroyQueue.Clear();
		_isProcessingDestroyQueue = false;
	}

	public void ThrowIfQueuedToDestroyOrAlreadyDestroyed(GameObject obj) {
		if (App.shared.HasQueuedToDestroy(obj)) {
			throw new System.InvalidOperationException("has queued to destroy");
		}

		if (obj == null) { // && !ReferenceEquals(obj, null)) {
			throw new System.InvalidOperationException("null game object");
		}
	}

	public bool HasQueuedToDestroy(GameObject obj) {
		return _destroyQueue.Contains(obj);
	}

	public void ImmediateDestory(GameObject obj) {
		Destroy(obj);
	}

	public void Log(object message, object context) {
		var messageString = "null";
		if (message != null) {
			messageString = message.ToString();
		}

		if (context == null) {
			Log(messageString);
		}
		else {
			if (context.inheritsFrom(typeof(UnityEngine.Object))) {
				Log(context.GetType() + "." + (context as UnityEngine.Object).GetInstanceID() + ": " + messageString);
			}
			else {
				Log(context.GetType() + ": " + messageString);
			}
		}
	}

	public void Log(object message) {
		if (debug) {
			Debug.Log("@" + Time.frameCount + ": " + message.ToString());
		}
	}

	public Soundtrack SoundtrackNamed(string trackName) {
		var tracks = soundtracks.Where(t => t.trackName == trackName).ToList(); 
		if (tracks.Count() > 0) {
			return tracks[0];
		}
		var track = gameObject.AddComponent<Soundtrack>();
		track.SetTrackName(trackName);
		soundtracks.Add(track);
		return track;
	}

	// --- load GameObject ---

	private Dictionary<string, GameObject> loadedGos = null;

	public GameObject LoadGameObject(string k) {

		if (loadedGos == null) {
			loadedGos = new Dictionary<string, GameObject>();
		}

		if (loadedGos.ContainsKey(k)) {
			return loadedGos[k];
		}
			
		GameObject v = Resources.Load<GameObject>(k);
		loadedGos[k] = v;

		return v;
	}

	// --- load AudioClip ---

	private Dictionary<string, AudioClip> loadedClips = null;

	public AudioClip LoadAudioClip(string k) {

		if (loadedClips == null) {
			loadedClips = new Dictionary<string, AudioClip>();
		}

		if (loadedClips.ContainsKey(k)) {
			return loadedClips[k];
		}

		AudioClip v = Resources.Load<AudioClip>(k);
		loadedClips[k] = v;

		return v;
	}

	// --- load Material ---


	private Dictionary<string, Material> loadedMaterials = null;

	public Material LoadMaterial(string k) {

		if (loadedMaterials == null) {
			loadedMaterials = new Dictionary<string, Material>();
		}

		if (loadedMaterials.ContainsKey(k)) {
			return loadedMaterials[k];
		}

		Material v = Resources.Load<Material>(k);
		loadedMaterials[k] = v;

		return v;
	}

	void OnApplicationQuit() {
		isExiting = true;
	}
}




/*
public class ATest : MonoBehaviour {
	public static void ClassTest() {
		print("type = " + MethodBase.GetCurrentMethod().ReflectedType.GetType().Name);
	}
}

public class BTest : ATest {
}
*/