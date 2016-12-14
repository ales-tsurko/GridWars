using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChatView : MonoBehaviour {
	public static string UIChatViewSubmittedNotification = "UIChatViewEditingEndedNotification";
	public static string UIChatViewReceivedFocusNotification = "UIChatViewReceivedFocusNotification";
	public static string UIChatViewLostFocusNotification = "UIChatViewLostFocusNotification";
	public static string UIChatViewShowedNotification = "UIChatViewShowedNotification";
	public static string UIChatViewHidNotification = "UIChatViewHidNotification";
	public static string UIChatViewCreatedNotification = "UIChatViewCreatedNotification";
	public static string UIChatViewDestroyedNotifcation = "UIChatViewDestroyedNotifcation";

	public static UIChatView Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(App.shared.LoadGameObject("UI/ChatView/UIChatView"));
		UI.AssignToCanvas(go);
		UIChatView chatView = go.GetComponent<UIChatView>();
		chatView.Hide();
		return chatView;
	}

	Coroutine applyLayoutCoroutine;
	List<Text> messageTextComponents = new List<Text>();

	public GameObject messageViewPrefab;
	public GameObject messagesContainer;
	public InputField inputField;

	public bool hasFocus;
	public bool isShown {
		get {
			return gameObject.activeInHierarchy;
		}
	}

	public string messageText {
		get {
			return inputField.text.Trim();
		}
	}

	public void AddMessage(string message) {
		var messageView = Instantiate(messageViewPrefab);
		var textComponent = messageView.GetComponent<Text>();
		messageView.transform.SetParent(messagesContainer.transform, false);

		textComponent.text = message;
		var textTransform = textComponent.GetComponent<RectTransform>();
		textTransform.offsetMin = new Vector2(13f, 0f);
		textTransform.offsetMax = new Vector2(-13f, 0f);

		messageTextComponents.Add(textComponent);

		if (applyLayoutCoroutine == null) {
			applyLayoutCoroutine = StartCoroutine(ApplyLayoutCoroutine());
		}
	}

	public void Hide() {
		if (isShown) {
			gameObject.SetActive(false);
			App.shared.PlayAppSoundNamed("ChatOpen");
			App.shared.notificationCenter.NewNotification()
				.SetName(UIChatViewHidNotification)
				.SetSender(this)
				.Post();
		}
	}

	public void Show() {
		if (!isShown) {
			App.shared.PlayAppSoundNamed("ChatClose");
			gameObject.SetActive(true);

			App.shared.notificationCenter.NewNotification()
				.SetName(UIChatViewShowedNotification)
				.SetSender(this)
				.Post();
		}

		var transform = GetComponent<RectTransform>();
		transform.offsetMin = new Vector2(0f, 0f);
		transform.offsetMax = new Vector2(0f, 0f);
		transform.sizeDelta = new Vector2(384f, transform.sizeDelta.y);
	}

	public void Destroy() {
		Hide();
		App.shared.notificationCenter.NewNotification()
			.SetName(UIChatViewDestroyedNotifcation)
			.SetSender(this)
			.Post();
		Destroy(gameObject);
	}

	public void Focus() {
		inputField.Select();
	}

	public void LoseFocus() {
		if (hasFocus) {
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		}
	}

	public void ClearInput() {
		inputField.text = "";
	}

	public void InputSubmitted() {
		App.shared.notificationCenter.NewNotification()
			.SetName(UIChatViewSubmittedNotification)
			.SetSender(this)
			.Post();
	}

	public void InputReceivedFocus() {
		hasFocus = true;
		App.shared.notificationCenter.NewNotification()
			.SetName(UIChatViewReceivedFocusNotification)
			.SetSender(this)
			.Post();
	}

	public void InputLostFocus() {
		hasFocus = false;
		App.shared.notificationCenter.NewNotification()
			.SetName(UIChatViewLostFocusNotification)
			.SetSender(this)
			.Post();
	}

	IEnumerator ApplyLayoutCoroutine() {
		yield return new WaitForEndOfFrame();
		ApplyLayout();
	}

	void ApplyLayout() {
		float y = 0f;
		float fontSize = (float)messageViewPrefab.GetComponent<Text>().fontSize;

		foreach (var text in messageTextComponents) {
			var textTransform = text.GetComponent<RectTransform>();
			textTransform.anchoredPosition = new Vector2(0f, y);
			y -= (textTransform.sizeDelta.y + fontSize);
		}

		var t = messagesContainer.GetComponent<RectTransform>();
		var height = Mathf.Abs(y) - fontSize;

		t.anchoredPosition = new Vector2(t.anchoredPosition.x, height);
		t.sizeDelta = new Vector2(t.sizeDelta.x, height);

		applyLayoutCoroutine = null;
	}

	void Update() {
		if (hasFocus) {
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
				InputSubmitted();
			}
			else if (Input.GetKeyDown(KeyCode.Escape)) {
				LoseFocus();
			}
		}
	}

	void Awake() {
		App.shared.notificationCenter.NewNotification()
			.SetName(UIChatViewCreatedNotification)
			.SetSender(this)
			.Post();
	}
}
