﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class UIMenu : UIElement {

	public List<UIMenuItem> items = new List<UIMenuItem> ();
	public float spacing;

	public void AddItem (UIMenuItem _item){
		RectTransform _t = GetComponent<RectTransform> ();
		RectTransform _i = _item.GetComponent<RectTransform> ();
		_i.SetParent (_t);
		items.Add (_item);
		OrderMenu ();
	}

	public void OrderMenu (float _spacing = 0){
		float h = GetComponent<RectTransform> ().sizeDelta.y;
		if (_spacing <= 0) {
			_spacing =  h / items.Count;
		}
		spacing = _spacing;
		for (int i = 0; i < items.Count; i++){
			items [i].GetComponent<RectTransform> ().localPosition = new Vector2 (0, (i * h) - (h * .5f));
		}
	}

	public void Reset () {
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
		items = new List<UIMenuItem> ();
	}

	public override void SetText (string s, float offset = 10f) {
		Text textObj = null;
		RectTransform _t = GetComponent<RectTransform> ();
		textObj = GetComponentInChildren<Text> ();	
		if (textObj == null) {
			textObj = UI.CreateTextObj (_t).GetComponent<Text>();
		}
		textObj.GetComponent<RectTransform> ().localPosition = new Vector2 (0, (GetComponent<RectTransform> ().sizeDelta.y * .5f / 2) - offset);
		textObj.text = s;
		gameObject.name = s;
	}
}
