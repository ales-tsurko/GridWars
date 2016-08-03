using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour {

	private static GameUI _instance;
	public static GameUI instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<GameUI> ();
				if (_instance == null) {
					GameObject ui = (GameObject)Instantiate (Resources.Load<GameObject> ("GameUI"));
					ui.name = "GameUI";
					_instance = ui.GetComponent<GameUI> ();
				}
			}
			return _instance;
		}
	}

	public RectTransform topPanel;
	public static RectTransform GetUIParent () {
		return instance.topPanel;
	}

}
