using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class UIButton : UIElement {

	UnityEvent method;
	void Awake () {
		GameObject textObj = new GameObject ();
		var rect = textObj.AddComponent<RectTransform> ();
		rect.SetParent (transform);
		rect.localPosition = Vector3.zero;
		rect.localScale = Vector3.one;
		rect.sizeDelta = GetComponent<RectTransform> ().sizeDelta * .5f;
		Text text = textObj.AddComponent<Text> ();
		text.resizeTextForBestFit = true;
		text.font = Resources.GetBuiltinResource<Font> ("Arial.ttf");
		text.alignment = TextAnchor.MiddleCenter;
	}

	void Start () {
		GetComponent<Button> ().onClick.AddListener (() => OnClick ());
	}
		
	public void OnClick (){
		print ("Click");
		method.Invoke ();
	}

	/// <summary>
	/// Sets the method to call OnClick
	/// </summary>
	/// <param name="action">Method without parens</param>
	public void SetAction (UnityAction action){
		gameObject.name = action.Method.ToString ();
		method = new UnityEvent ();
		method.AddListener (action);
	}
}
