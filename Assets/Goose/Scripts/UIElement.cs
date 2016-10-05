using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIElement : MonoBehaviour {
    public string itemData;
	public bool isHidden {
		get {
			return !gameObject.activeInHierarchy;
		}
	}

	public bool matchesNeighborSize = true;

	bool _isOutlined;
	public bool isOutlined {
		get {
			return _isOutlined;
		}

		set {
			_isOutlined = value;
			foreach (var imageComponent in gameObject.GetComponentsInChildren<Image>()) {
				imageComponent.enabled = value;
			}
		}
	}

	/// <summary>
	/// Sets the text of the GameButton
	/// </summary>
	/// <param name="s">S.</param>
	public virtual Text SetText (string s, bool allcaps = false, float offset = 0, UIFont _font = UI.DEFAULTFONT){
		Text textObj = null;
		RectTransform _t = GetComponent<RectTransform> ();
		textObj = GetComponentInChildren<Text> ();	
		if (textObj == null) {
			textObj = UI.CreateTextObj (_t).GetComponent<Text>();
		}
		if (allcaps) {
			s = s.ToUpper ();
		}
		textObj.text = s;
		textObj.font = UI.GetFont (_font);
		return textObj;
	}

	public System.Object data;
	public System.Action action;
	/// <summary>
	/// Sets the position of the Button
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SetPosition (float x, float y) {
		float w = Screen.width;
		float h = Screen.height;
		transform.localPosition = new Vector3 (x * w / 2f, h * y / 2f, 0);
	}

    public void Destroy() {
        Destroy(gameObject);
    }

	public void SetSize (float x, float y, bool preserveAspect = true){
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (x, y);
		SetImageAspect (preserveAspect);
	}

	public virtual void Show () {
		gameObject.SetActive (true);
	}

	public void Hide () {
		gameObject.SetActive (false);
	}

	public void SetImageAspect (bool b){ 
		if (GetComponent<Image> () == null) {
			return;
		}
		GetComponent<Image> ().preserveAspect = b;
	}
	public void SetImageType (Image.Type type) {
		if (GetComponent<Image> () == null) {
			return;
		}
		GetComponent<Image> ().type = type;
	}

   

	/// <summary>
	/// Sets the method to call OnClick
	/// </summary>
	/// <param name="action">Method without parens</param>
	
    public void SetAction (System.Action _action){
        action = _action;
    }

}

public static class UIElementExtension {
    public static UIMenuItem SetData (this UIMenuItem element, string _data){
        element.itemData = _data;
        return element;
    }
}
