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

}

public static class UIElementExtension {
    public static UIButton SetData (this UIButton element, string _data){
        element.itemData = _data;
        return element;
    }
}
