using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

/*
 *  This is a singleton. Access like this:
 * 
 *  App.shared.ResourcePathForUnitType(typeof(Tank))
 * 
 */

public class App : MonoBehaviour, AppStateOwner {
	//test github push
    [Range(.01f, 5)]
    public float gameSpeed = 1;
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

	public Matchmaker matchmaker;
	public Network network;
	public Battlefield battlefield;
	public CameraController cameraController;
	public Keys keys;
	public PlayerInputs inputs; //used outside of games
	public Account account;

	public string version {
		get {
			return Resources.Load<TextAsset>("version").text;
		}
	}

	private bool _isProcessingDestroyQueue = false;

	public bool testEndOfGameMode = false;

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

	public void Awake() {
		timerCenter = new AssemblyCSharp.TimerCenter();

	}

	public void Start() {
		Profiler.maxNumberOfSamplesPerFrame = 1048576; //Unity bug

		prefs = new Prefs();

		stepCache = new AssemblyCSharp.StepCache();
		_destroyQueue = new List<GameObject>();
		soundtracks = new List<Soundtrack>();

		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;

		Wreckage.SetupLayerCollisions();
		Projectile.SetupLayerCollisions();

		account = new Account();

		matchmaker = new Matchmaker();

		network = new GameObject().AddComponent<Network>();
		network.gameObject.name = "Network";

		battlefield = GameObject.Find("Battlefield").GetComponent<Battlefield>();

		cameraController = GameObject.FindObjectOfType<CameraController>();
		cameraController.enabled = true;

		keys = new Keys();

		inputs = new PlayerInputs();
		inputs.AddControllerBindings();
		inputs.AddLocalPlayer1KeyBindings();

		menu = UI.Menu();

		//*
		var mainMenuState = new MainMenuState();
		mainMenuState.owner = this;
		this.state = mainMenuState;
		mainMenuState.EnterFrom(null);
		//*/
	}

	public void FixedUpdate() {
		timerCenter.Step();
		stepCache.Step();
		timeCounter++;
	}

	void Update() {
		state.Update();
		keys.Update();
	}

	void OnDestroy() {
		inputs.Destroy();
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
        #if UNITY_EDITOR
        Time.timeScale = Mathf.Clamp(gameSpeed, 0.01f, 5);
        #endif
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