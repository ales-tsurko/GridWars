﻿using UnityEngine;
using System.Collections.Generic;
using InControl;

public class BindInputsToPlayersState : AppState {
	Player player;

	bool firstPlayerUsedKeyboard;
	InputDevice assignedDevice;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		firstPlayerUsedKeyboard = false;

		player = app.battlefield.localPlayer1;

		ResetMenu();
	}

	void ResetMenu() {
		app.ResetMenu();
		//TODO describe the controller button.
		menu.AddItem(UI.ActivityIndicator("<color=#" + ColorUtility.ToHtmlStringRGB(player.primaryColor.WithV(1f)) + ">" + player.description + "</color>\n\n" + "Press return or click continue to use keyboard and mouse.\n\nPress any button to use controller."));
		//menu.AddItem(UI.ActivityIndicator(player.description + "\n\n" + "Press return or click continue to use keyboard and mouse.\n\nPress any button to use controller."));
		menu.AddItem(UI.MenuItem("Continue", Continue));
		menu.AddItem(UI.MenuItem("Cancel", Cancel), true);
		menu.Show();
	}

	// Update

	/*
	List<InputDevice>attachedDevices {
		get {
			return new List<InputDevice>(InputManager.Devices).FindAll(d => d.IsAttached);
		}
	}

	bool isDeviceAttached {
		get {
			return attachedDevices.Count > 0;
		}
	}
	*/

	void Continue() {
		if (InputManager.ActiveDevice.Action1.WasPressed && InputManager.ActiveDevice != assignedDevice) {
			assignedDevice = InputManager.ActiveDevice;
			player.inputs = new PlayerInputs();
			player.inputs.LastInputType = BindingSourceType.DeviceBindingSource;
			player.inputs.Device = InputManager.ActiveDevice;
			player.inputs.AddControllerBindings();

			if (player.isLocalPlayer1) {
				NextPlayer();
			}
			else {
				NextState();
			}
		}
		else if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Return)) {
			player.inputs = new PlayerInputs();
			player.inputs.LastInputType = BindingSourceType.KeyBindingSource;

			if (player.isLocalPlayer1) {
				firstPlayerUsedKeyboard = true;
				player.inputs.AddLocalPlayer1KeyBindings();
				NextPlayer();
			}
			else {
				if (firstPlayerUsedKeyboard) {
					player.inputs.AddLocalPlayer2KeyBindings();
				}
				else {
					player.inputs.AddLocalPlayer1KeyBindings();
				}

				NextState();
			}
		}
	}

	void NextState() {
		TransitionTo(new WaitForBoltState());
	}

	void Cancel() {
		TransitionTo(new MainMenuState());
	}

	void NextPlayer() {
		player = app.battlefield.localPlayer2;
		ResetMenu();
	}
}
