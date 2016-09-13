using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class UIMenu : UIElement {
	Image image;

	public List<UIMenuItem> items = new List<UIMenuItem> ();
	public float spacing;

	public Color backgroundColor {
		get {
			return image.color;
		}

		set {
			image.color = value;
		}
	}

	public void Init() {
		image = gameObject.AddComponent<Image>();
		image.color = Color.black;

		RectTransform t = GetComponent<RectTransform>();
		t.anchorMin = new Vector2(0, 0);
		t.anchorMax = new Vector2(1, 1);
		t.offsetMin = new Vector2(0, 0);
		t.offsetMax = new Vector2(0, 0);
	}

	public void AddItem (UIMenuItem _item){
		RectTransform _t = GetComponent<RectTransform> ();
		RectTransform _i = _item.GetComponent<RectTransform> ();
		_i.SetParent (_t);
		items.Add (_item);
		_item.Show ();
		OrderMenu ();
	}

	public void OrderMenu (float _spacing = 0){
		float h = GetComponent<RectTransform> ().sizeDelta.y;
		if (_spacing <= 0) {
			_spacing = items [0].GetComponent<RectTransform> ().sizeDelta.y * items.Count / items.Count * 1.1f;
		}
		SetSize (items [0].GetComponent<RectTransform> ().sizeDelta.x, items [0].GetComponent<RectTransform> ().sizeDelta.y * items.Count);
		spacing = _spacing;
		for (int i = 0; i < items.Count; i++){
			items [i].GetComponent<RectTransform> ().localPosition = new Vector2 (0, (-i * spacing) + (h * .5f));
		}
	}

	public void Reset () {
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
		items = new List<UIMenuItem> ();
	}

	public override Text SetText (string s, bool allcaps = false, float offset = 10f, UIFont _font = UI.DEFAULTFONT) {
		Text textObj = null;
		RectTransform _t = GetComponent<RectTransform> ();
		textObj = GetComponentInChildren<Text> ();	
		if (textObj == null) {
			textObj = UI.CreateTextObj (_t).GetComponent<Text>();
		}
		textObj.GetComponent<RectTransform> ().localPosition = new Vector2 (0, (GetComponent<RectTransform> ().sizeDelta.y * .5f / 2) - offset);
		if (allcaps) {
			s = s.ToUpper ();
		}
		textObj.text = s;
		gameObject.name = s;
		return textObj;
	}
}
