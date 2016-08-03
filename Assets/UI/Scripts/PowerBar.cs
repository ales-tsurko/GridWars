using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PowerBar : MonoBehaviour {
	
	public Text text;
	public Slider slider;
	public Image barColor;
	public float minPower = 0f;
	public float maxPower = 100f;
	public float power = 0f;
	RectTransform _t;


	void Start () {
		_t = GetComponent<RectTransform> ();
		_t.SetParent (GameUI.GetUIParent());
	}

	public void SetPosition (float left, float right, float top, float bottom){
		_t = GetComponent<RectTransform> ();
		_t.SetParent (GameUI.GetUIParent());
		_t.offsetMin = new Vector2(left, bottom);
		_t.offsetMax = new Vector2 (-right, -top);
	}

	public void SetColor (Color _color){
		barColor.color = _color;
	}

	public void SetPosition (PowerBarPlacement _placement){
		switch (_placement) {
		case PowerBarPlacement.Left:
			SetPosition (-8f, 1700f, 20f, 20f);
			break;
		case PowerBarPlacement.Right:
			SetPosition (1700f, -8f, 20f, 20f);
			break;
		}
	}

	void Update() {
		power = Mathf.Clamp(power, minPower, maxPower);
		slider.minValue = minPower;
		slider.maxValue = maxPower;
		slider.value = power;
	}

	public static PowerBar New(){
		GameObject _bar = (GameObject)Instantiate (Resources.Load<GameObject> ("PowerBar"));
		return _bar.GetComponent<PowerBar> ();
	}

	public static PowerBar New(PowerBarPlacement _placement, Color? _color = null){
		PowerBar _bar = New ();
		_bar.SetPosition (_placement);
		_bar.SetColor (_color ?? Color.white);
		return _bar;
	}
}

public enum PowerBarPlacement {Left, Right}
