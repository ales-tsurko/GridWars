using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIElement : MonoBehaviour {

	/// <summary>
	/// Sets the text of the GameButton
	/// </summary>
	/// <param name="s">S.</param>
	public void SetText (string s){
		Text textObj = GetComponentInChildren<Text> ();
		if (textObj == null) {
			Debug.LogError (transform.name + " has no text element");
			return;
		}
		textObj.text = s;
	}

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

	public void SetSize (float x, float y){
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (x, y);
	}
}
