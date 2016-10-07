using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIPopup : UIButton {

	public static UIPopup Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(Resources.Load<GameObject>(UI.BUTTONPREFAB));
		UI.AssignToCanvas(go);
		Destroy(go.GetComponent<UIButton>());
		UIPopup popup = go.AddComponent<UIPopup>();
		return popup;
	}

	void Start () {
	
	}
	
	void Update () {
	
	}
}
