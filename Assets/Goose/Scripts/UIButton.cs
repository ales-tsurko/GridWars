using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InControl;

using UnityEngine.Assertions;

[System.Serializable]
public class UIButton : UIElement {

	public float startTime = 0;
	public float charactersPerSecond = 10f;
	public bool doesType = false;
	public bool wasActivatedByMouse;
	public bool useRainbowStyle = false;
	public float rainbowCyclePeriod = 2f;


	private bool finishedTyping = false;

	public static UIButton Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(App.shared.LoadGameObject(UI.BUTTONPREFAB));
		UI.AssignToCanvas(go);
		UIButton button = go.GetComponent<UIButton>();
		return button;
	}
		
	UnityEvent method;
    string baseText;

	public Text textComponent {
		get {
			return GetComponentInChildren<Text>();
		}
	}

	string _text;
	string _textSuffix = "";
    public virtual string text {
        get {
			return _text;
        }

        set {
			_text = value;
			if (_text == null) {
				_text = "";
			}


			bool shouldType = doesType && (!value.Contains("<") && !value.Contains("\n"));

			textComponent.text = value.ToUpper();
			SizeToFit();

			if (shouldType) {
				textComponent.text = "";
			}
			else {
				UpdateDisplayedText();
			}
        }
    }

	void UpdateDisplayedText() {
		textComponent.text = displayedText;
		SizeToFit();
	}

	string displayedText {
		get {
			return (_text + _textSuffix).ToUpper();
		}
	}

	public UIButton SetText(string text) {
		this.text = text;
		return this;
	}

	public bool isBackItem;

	public UIButton SetIsBackItem(bool isBackItem) {
		this.isBackItem = isBackItem;
		return this;
	}

	PlayerAction _playerAction;
	public PlayerAction playerAction {
		get {
			return _playerAction;
		}

		set {
			_playerAction = value;
			_playerAction.Owner.OnLastInputTypeChanged += LastInputTypeChanged;
			UpdateSuffix();
		}
	}

	void LastInputTypeChanged(BindingSourceType type) {
		UpdateSuffix();
	}

    public UIButton SetPlayerAction (PlayerAction _playerAction){
        playerAction = _playerAction;
        return this;
    }

	public RuntimeAnimatorController alertStyleController;
	public RuntimeAnimatorController defaultStyleController;

	public UIButton UseAlertStyle() {
		this.GetComponent<Animator>().runtimeAnimatorController = alertStyleController;
		useRainbowStyle = false;
		return this;
	}

	public UIButton UseDefaultStyle() {
		this.GetComponent<Animator>().runtimeAnimatorController = defaultStyleController;
		useRainbowStyle = false;
		return this;
	}

	public UIButton UseRainbowStyle() {
		this.GetComponent<Animator>().runtimeAnimatorController = null;
		useRainbowStyle = true;
		return this;
	}

	public System.Action action;

	public UIButton SetAction(System.Action action) {
		this.action = action;
		return this;
	}

	public UIButton SetData(object data) {
		this.data = data;
		return this;
	}


	public bool isInteractible {
		get {
			return selectableComponent.interactable;
		}

		set {
			selectableComponent.interactable = value && allowsInteraction;
		}
	}

	bool _allowsInteraction;
	public bool allowsInteraction {
		get {
			return _allowsInteraction;
		}

		set {
			_allowsInteraction = value;
			if (!value) {
				isInteractible = false;
			}
		}
	}

	public Vector2 innerMargins = new Vector2(1, 1); //ratio of font height

	Selectable selectableComponent {
        get {
			return GetComponent<Selectable>();
        }
    }

	Button buttonComponent {
		get {
			return GetComponent<Button>();
		}
	}

    protected RectTransform rectTransform {
        get {
            return GetComponent<RectTransform>();
        }
    }

    [HideInInspector]
    public Vector2 menuSize = Vector2.zero;

	public UIMenu menu;

	public override void Show() {
		base.Show();

		if (menu != null) {
			menu.ApplyLayout();
		}

	}

	public override void Hide() {
		base.Hide();

		if (menu != null) {
			menu.ApplyLayout();
		}
	}

	public void Select() {
		selectableComponent.Select();
	}

	public bool isSelected;

	public void SetMenuIndex(int menuIndex) {
		menu.SetItemIndex(this, menuIndex);
	}

	protected EventTrigger eventTrigger {
		get {
			return GetComponent<EventTrigger>();
		}
	}

    protected virtual void Awake () {
		allowsInteraction = true;
	}

	protected virtual void Start () {
		if (buttonComponent != null) {
			buttonComponent.onClick.AddListener(OnClick);
		}

		var eventTrigger = gameObject.AddComponent<EventTrigger>();

		var entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener(data => OnSelected());
		eventTrigger.triggers.Add(entry);

		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Deselect;
		entry.callback.AddListener(data => OnDeselected());
		eventTrigger.triggers.Add(entry);

		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(data => OnPointerEnter());
		eventTrigger.triggers.Add(entry);

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(Prefs.PrefsChangedNotification)
			.SetAction(PrefsChangedNotification)
			.Add();

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(Tower.TowerUpdatedHotkeyTextNotification)
			.SetAction(TowerUpdatedHotkeyText)
			.Add();

		wasActivatedByMouse = false;
	}

	void OnDestroy() {
		if (playerAction != null) {
			_playerAction.Owner.OnLastInputTypeChanged -= LastInputTypeChanged;
		}

		if (App.shared.notificationCenter != null) {
			App.shared.notificationCenter.RemoveObserver(this);
		}
	}

	void PrefsChangedNotification(Notification notification) {
		if (notification.data as string == "keyIconsVisible") {
			UpdateSuffix();
		}
	}

	bool hasFixedText = false;
	//Hack: Unity garbles text when textMesh.text = releaseAction.HotkeyDescription(); is called in Tower.  Only needs to be fixed once for all text.
	void TowerUpdatedHotkeyText(Notification notification) {
		if (isActiveAndEnabled && !hasFixedText)  {
			hasFixedText = true;
			StartCoroutine(UpdateDisplayedTextAtEndOfFrame());
		}
	}

	IEnumerator UpdateDisplayedTextAtEndOfFrame() {
		textComponent.text = displayedText + "\u2063";
		yield return new WaitForEndOfFrame();
		textComponent.text = displayedText;
	}

	void UpdateSuffix() {
		if (playerAction != null) {
			var previousTextSuffix = _textSuffix;
			var description = playerAction.HotkeyDescription();

			if (description == "" || !App.shared.prefs.keyIconsVisible) {
				_textSuffix = "";
			}
			else {
				_textSuffix = " (" + description + ")";
			}

			if (previousTextSuffix != _textSuffix) {
				if (doesType) {
					textComponent.text = "";
					startTime = 0;
				}
				else {
					UpdateDisplayedText();
				}
			}
		}
	}

	public void Deselect() {
		isSelected = false;
		menu.ItemDeselected(this);
	}

	public virtual void Update () {
		//base.Update();
		if (doesType && !finishedTyping) {

			var fullText = this.displayedText; //perf opt

			if (startTime == 0) {
				startTime = Time.time;
				textComponent.text = "";
				if (fullText.Length < 20) {
					charactersPerSecond = 20f;
				} else {
					charactersPerSecond = fullText.Length / 1f;
				}
			}

			int n = (int)((Time.time - startTime) * charactersPerSecond);
			n = Mathf.Clamp(n, 0, fullText.Length);
			if (n < fullText.Length) {
				string typed = fullText.Substring(0, n).ToUpper();
				//string remaining = fullText.Substring(n, fullText.Length - n).ReplacedNonWhiteSpaceWithSpaces();
				string remaining = new string(' ', fullText.Length - n);
				textComponent.text = typed + remaining;
			} else {
				textComponent.text = fullText.ToUpper();
				finishedTyping = true;
			}

			SizeToFit();
		}

		if (useRainbowStyle) {
			/*
			Color[] colors = new Color[] {
				Color.red,
				Color.black.ToOrange(),
				Color.yellow,
				Color.green,
				Color.blue,
				Color.black.ToIndigo(),
				Color.black.ToViolet()

				//new Color().ToTronTerminalBlue(),
				//new Color().ToP3Amber()
			};
			*/


			// picked these colors to attempt to avoid a double pulse effect
			Color[] colors = new Color[] {
				new Color(1.00F, 0.50F, 0.00F, 1F), 
				new Color(0.50F, 1.00F, 0.00F, 1F), 
				new Color(0.00F, 1.00F, 0.50F, 1F), 
				new Color(0.00F, 0.50F, 1.00F, 1F), 
				new Color(0.50F, 0.00F, 1.00F, 1F), 
				new Color(1.00F, 0.00F, 0.50F, 1F), 
			};
				
			float progress = (Time.time % rainbowCyclePeriod) / rainbowCyclePeriod;
			int startColorIndex = (int)Mathf.Floor(progress * colors.Length);
			int endColorIndex = (startColorIndex + 1) % colors.Length;
			var lerpProgress = progress * colors.Length - (float)startColorIndex;

			textComponent.color = Color.Lerp(colors[startColorIndex], colors[endColorIndex], lerpProgress);
		}
	}

	int clickFrame = -1; //fire event only 1x per frame

	public void OnClick() {
		if (action != null && isInteractible && clickFrame != Time.frameCount) {
			wasActivatedByMouse = true;
			clickFrame = Time.frameCount;
			StartCoroutine(ActivateCoroutine());
        }
	}

	IEnumerator ActivateCoroutine() {
		yield return new WaitForEndOfFrame(); // If action selects a new menu item, it won't be activated.
		if (menu != null && menu.soundsEnabled) {
			App.shared.PlayAppSoundNamedAtVolume("MenuItemClicked", .5f);
		}
		action.Invoke();
		wasActivatedByMouse = false;
	}

	public void OnPointerEnter() {
		if (isInteractible) {
			menu.SelectItem(this);
		}
	}

	public void OnSelected() {
		if (menu != null && menu.soundsEnabled) {
			App.shared.PlayAppSoundNamedAtVolume("MenuItemSelected", 0.1f);
		}
		isSelected = true;
	}

	public void OnDeselected() {
		//Debug.Log("OnDeselected: " + this.text);
        //menu.Deselect();
		isSelected = false;
		menu.ItemDeselected(this);
	}

	public virtual void SizeToFit() {
		var w = textComponent.preferredWidth;

		var lineCount = textComponent.text.Split('\n').Length;
		var h = textComponent.font.lineHeight*lineCount;

		rectTransform.sizeDelta = new Vector2(
			w + textComponent.font.fontSize*innerMargins.x*2,
			h + textComponent.font.fontSize*innerMargins.y*2
		);

		if (menu != null) {
			menu.ApplyLayout();
		}
    }

    public void SetMenuSize (Vector2 size){
        menuSize = size;
        rectTransform.sizeDelta = menuSize;
    }

	public UIButton SetFillColor (Color _color, ButtonColorType type = ButtonColorType.Normal){
		ColorBlock c = selectableComponent.colors;
        switch (type) {
            case ButtonColorType.Normal:
                c.normalColor = _color;
                break;
            case ButtonColorType.Hover:
                c.highlightedColor = _color;
                break;
            case ButtonColorType.Pressed:
                c.pressedColor = _color;
                break;
            case ButtonColorType.Disabled:
                c.disabledColor = _color;
                break;
			case ButtonColorType.All:
			SetFillColor(_color, ButtonColorType.Normal);
			SetFillColor(_color, ButtonColorType.Hover);
			SetFillColor(_color, ButtonColorType.Pressed);
			SetFillColor(_color, ButtonColorType.Disabled);
			selectableComponent.image.color = _color;
			break;
        }
		selectableComponent.colors = c;
		return this;
    }

    public void SetBorderColor (Color _color){
        Image i = transform.FindChild("Border").GetComponent<Image>();
        i.color = _color;
    }

	public UIButton SetTextColor (Color _color){
        Text t = transform.FindChild("Text").GetComponent<Text>();
        t.color = _color;
		return this;
    }
}
public enum ButtonColorType { Normal, Pressed, Hover, Disabled, All }