using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class UIButton : UIMenuItem {

	UnityEvent method;
	void Awake () {
		if (GetComponentInChildren<Text> () == null) {
			UI.CreateTextObj (GetComponent<RectTransform>(), UIFont.Army);
		}
	}

	void Start () {
		GetComponent<Button> ().onClick.AddListener (() => OnClick ());
	}
		
	public void OnClick (){
		action.Invoke (this);
	}

}
