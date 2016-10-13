using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIButtonRemapKey : UIButton {
	public static new UIButtonRemapKey Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(Resources.Load<GameObject>(UI.BUTTONPREFAB));
		UI.AssignToCanvas(go);
		GameObject.Destroy(go.GetComponent<UIButton>());
		var button = go.AddComponent<UIButtonRemapKey>();
		return button;
	}

    public string code;
    public KeyCode keyKey, joyKey;
    public new void OnClick () {
		App.shared.keys.RemapKey(this);
    }
}
