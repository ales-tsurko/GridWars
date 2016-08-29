using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public static class UI {

	const string DIR = "UI/";
	const string BUTTON = DIR + "Button";
	const string CANVAS = "Canvas";
	const string SKINDIR = DIR + "Skins/";


	public static UIButton RoundButton (string skin = "Default") {
		skin += "/";
		GameObject obj = new GameObject ();
		UIButton go = obj.AddComponent<UIButton> ();
		go.transform.SetParent (MainCanvas ().transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.GetComponent<Image> ().overrideSprite = Resources.Load<Sprite> (SKINDIR + skin + "ButtonRound");
		return go;
	}
		
	/// <summary>
	/// Returns the Canvas or creates one if null
	/// </summary>
	/// <returns>The canvas.</returns>
	public static Canvas MainCanvas () {
		GameObject go = GameObject.Find (CANVAS);
		Canvas canvas;
		if (go == null) {
			go = new GameObject ();
			canvas = go.AddComponent<Canvas> ();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.name = CANVAS;
			var scaler = canvas.gameObject.AddComponent<CanvasScaler> ();
			var raycaster = canvas.gameObject.AddComponent<GraphicRaycaster> ();
			return canvas;
		} else {
			return go.GetComponent<Canvas> ();
		}
	}
}
