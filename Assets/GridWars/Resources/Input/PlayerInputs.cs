using InControl;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInputs : PlayerActionSet {
	public PlayerAction releaseChopper;
	public PlayerAction releaseTanker;
	public PlayerAction releaseTank;
	public PlayerAction releaseLightTank;
	public PlayerAction releaseMobileSam;
	public PlayerAction releaseBomber;
	public PlayerAction releaseGunship;

	public PlayerAction castSpell1;
	public PlayerAction castSpell2;
	public PlayerAction castSpell3;
	public PlayerAction castSpell4;

	public PlayerAction toggleMenu;
	public PlayerAction concede;
	public PlayerAction toggleHotkeys;
	public PlayerAction nextCamera;
	public PlayerAction firstPersonCamera;
	public PlayerAction focusMessenger;
	public PlayerAction upItem;
	public PlayerAction downItem;
	public PlayerAction leftItem;
	public PlayerAction rightItem;
	public PlayerAction selectItem;
	public PlayerAction goBack;
	public PlayerAction continueTutorial;

	PlayerAction lookLeft;
	PlayerAction lookRight;
	PlayerAction lookUp;
	PlayerAction lookDown;
	public PlayerTwoAxisAction look;
    public PlayerAction enterFPS;
    public PlayerAction exitFPS;
    public PlayerAction unitNext;
    public PlayerAction unitPrev;

	/*
	InputDevice _device;
	public InputDevice device {
		get {
			return _device;
		}

		set {
			_device = value;
			foreach(var action in Actions) {
				action.Device = _device;
			}
		}
	}
	*/

	public PlayerInputs() {
		releaseChopper = CreatePlayerAction("Chopper");
		releaseTanker = CreatePlayerAction("Tanker");
		releaseTank = CreatePlayerAction("Tank");
		releaseLightTank = CreatePlayerAction("LightTank");
		releaseMobileSam = CreatePlayerAction("MobileSAM");
		releaseBomber = CreatePlayerAction("Bomber");
		releaseGunship = CreatePlayerAction("Gunship");

		castSpell1 = CreatePlayerAction("CastSpell1");
		castSpell2 = CreatePlayerAction("CastSpell2");
		castSpell3 = CreatePlayerAction("CastSpell3");
		castSpell4 = CreatePlayerAction("CastSpell4");

		toggleMenu = CreatePlayerAction("Toggle Menu");
		concede = CreatePlayerAction("Concede");
		toggleHotkeys = CreatePlayerAction("Hotkeys");

		nextCamera = CreatePlayerAction("Next Camera");

		firstPersonCamera = CreatePlayerAction("Unit Camera");

		focusMessenger = CreatePlayerAction("Focus Messenger");

		upItem = CreatePlayerAction("Up Item");
		downItem = CreatePlayerAction("Down Item");
		leftItem = CreatePlayerAction("Left Item");
		rightItem = CreatePlayerAction("Right Item");
		selectItem = CreatePlayerAction("Select Item");
		goBack = CreatePlayerAction("Go Back");

		continueTutorial = CreatePlayerAction("Continue Tutorial");

		lookLeft = CreatePlayerAction("Look Left");
		lookRight = CreatePlayerAction("Look Right");
		lookUp = CreatePlayerAction("Look Up");
		lookDown = CreatePlayerAction("Look Down");
		look = CreateTwoAxisPlayerAction(lookLeft, lookRight, lookDown, lookUp);
        enterFPS = CreatePlayerAction("Enter FPS Mode");
        exitFPS = CreatePlayerAction("Exit FPS Mode");
        unitNext = CreatePlayerAction("Next Unit");
        unitPrev = CreatePlayerAction("Previous Unit");
	}

	public void AddControllerBindings() {
		releaseMobileSam.AddDefaultBinding(InputControlType.Action1);
		releaseTanker.AddDefaultBinding(InputControlType.Action2);
		releaseTank.AddDefaultBinding(InputControlType.Action3);
		releaseChopper.AddDefaultBinding(InputControlType.Action4);
		releaseLightTank.AddDefaultBinding(InputControlType.Action5);

		toggleMenu.AddDefaultBinding(InputControlType.Command);

        //concede.AddDefaultBinding(InputControlType.LeftTrigger);
        //toggleHotkeys.AddDefaultBinding(InputControlType.RightTrigger);

        //nextCamera.AddDefaultBinding(InputControlType.RightBumper);
        //firstPersonCamera.AddDefaultBinding(InputControlType.DPadDown);

        castSpell1.AddDefaultBinding(InputControlType.LeftBumper);
		castSpell2.AddDefaultBinding(InputControlType.RightBumper);
		castSpell3.AddDefaultBinding(InputControlType.LeftTrigger);
		castSpell4.AddDefaultBinding(InputControlType.RightTrigger);

		upItem.AddDefaultBinding(InputControlType.LeftStickUp);
		upItem.AddDefaultBinding(InputControlType.RightStickUp);
		upItem.AddDefaultBinding(InputControlType.DPadUp);

		downItem.AddDefaultBinding(InputControlType.LeftStickDown);
		downItem.AddDefaultBinding(InputControlType.RightStickDown);
		downItem.AddDefaultBinding(InputControlType.DPadDown);

		leftItem.AddDefaultBinding(InputControlType.LeftStickLeft);
		leftItem.AddDefaultBinding(InputControlType.RightStickLeft);
		leftItem.AddDefaultBinding(InputControlType.DPadLeft);

		rightItem.AddDefaultBinding(InputControlType.LeftStickRight);
		rightItem.AddDefaultBinding(InputControlType.RightStickRight);
		rightItem.AddDefaultBinding(InputControlType.DPadRight);

		selectItem.AddDefaultBinding(InputControlType.Action1);
		goBack.AddDefaultBinding(InputControlType.Action2);

		continueTutorial.AddDefaultBinding(InputControlType.Action1);

		lookLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
		lookLeft.AddDefaultBinding(InputControlType.RightStickLeft);
		lookRight.AddDefaultBinding(InputControlType.LeftStickRight);
		lookRight.AddDefaultBinding(InputControlType.RightStickRight);
		lookDown.AddDefaultBinding(InputControlType.LeftStickDown);
		lookDown.AddDefaultBinding(InputControlType.RightStickDown);
		lookUp.AddDefaultBinding(InputControlType.LeftStickUp);
		lookUp.AddDefaultBinding(InputControlType.RightStickUp);

        enterFPS.AddDefaultBinding(InputControlType.LeftStickButton);
        exitFPS.AddDefaultBinding(InputControlType.LeftStickButton);
        unitNext.AddDefaultBinding(InputControlType.RightBumper);
        //unitPrev.AddDefaultBinding(InputControlType.LeftBumper);

	}

	public void AddLocalPlayer1KeyBindings() {
		toggleMenu.AddDefaultBinding(Key.Escape);

		upItem.AddDefaultBinding(Key.UpArrow);
		downItem.AddDefaultBinding(Key.DownArrow);

		leftItem.AddDefaultBinding(Key.LeftArrow);
		rightItem.AddDefaultBinding(Key.RightArrow);

		selectItem.AddDefaultBinding(Key.Return);
		selectItem.AddDefaultBinding(Key.PadEnter);

		goBack.AddDefaultBinding(Key.Escape);

		continueTutorial.AddDefaultBinding(Key.Space);

		concede.AddDefaultBinding(Key.Q);
		toggleHotkeys.AddDefaultBinding(Key.H);

		nextCamera.AddDefaultBinding(Key.Z);
		firstPersonCamera.AddDefaultBinding(Key.DownArrow);

		focusMessenger.AddDefaultBinding(Key.Tab);

		releaseChopper.AddDefaultBinding(Key.D);
		releaseTanker.AddDefaultBinding(Key.F);
		releaseTank.AddDefaultBinding(Key.J);
		releaseLightTank.AddDefaultBinding(Key.G);
		releaseMobileSam.AddDefaultBinding(Key.K);
		releaseGunship.AddDefaultBinding(Key.L);

		castSpell1.AddDefaultBinding(Key.C);
		castSpell2.AddDefaultBinding(Key.V);
		castSpell3.AddDefaultBinding(Key.N);
		castSpell4.AddDefaultBinding(Key.M);

        exitFPS.AddDefaultBinding(Key.Escape);
	}

	public void AddLocalPlayer2KeyBindings() {
		releaseChopper.AddDefaultBinding(Key.P);
		releaseTanker.AddDefaultBinding(Key.LeftBracket);
		//releaseLightTank.AddDefaultBinding(Key.RightBracket);
		releaseTank.AddDefaultBinding(Key.RightBracket);
		releaseMobileSam.AddDefaultBinding(Key.Backslash);
	}
}

public static class PlayerInputsExtensions {
	public static Dictionary<string, string> ControlHandleNormalizations = new Dictionary<string, string>() {
		{ "cross", "X" },
		{ "triangle", "△" },
		{ "square", "▢" },
		{ "circle", "◯" },
		{ "left trigger", "L2" },
		{ "right trigger", "R2" },
		{ "left bumper", "L1" },
		{ "right bumper", "R1" },
		{ "command", "" }
	};

	public static Dictionary<string, string> KeynameToKey = new Dictionary<string, string>() {
		{ "backslash", "\\" },
		{ "right bracket",  "]" },
		{ "left bracket", "[" },
		{ "escape", "esc" },
		{ "tab", "tab" }
	};

	public static string HotkeyDescription(this DeviceBindingSource self, int maxLength = 1) {
		var handle = self.BoundTo.Device.GetControl(self.Control).Handle;
		foreach (var pair in PlayerInputsExtensions.ControlHandleNormalizations) {
			if (handle.ToLower().StartsWith(pair.Key)) {
				return pair.Value;
			}
		}
		return handle.Substring(0, Mathf.Min(maxLength, handle.Length));
	}

	public static string HotkeyDescription(this KeyBindingSource self, int maxLength = 1) {
		var handle = self.Control.ToString();

		foreach (var pair in PlayerInputsExtensions.KeynameToKey) {
			if (handle.ToLower().StartsWith(pair.Key)) {
				return pair.Value;
			}
		}
		return handle.Substring(0, Mathf.Min(maxLength, handle.Length));
	}

	public static BindingSource LastBindingSource(this PlayerAction self) {
		BindingSource defaultBindingSource = null;

		foreach (var binding in self.Bindings) {
			if (binding.BindingSourceType == BindingSourceType.KeyBindingSource) {
				defaultBindingSource = binding;
			}
			if (binding.BindingSourceType == self.Owner.LastInputType) {
				return binding;
			}
		}

		return defaultBindingSource;
	}

	public static string HotkeyDescription(this PlayerAction self, int maxLength = 1) {
		var bindingSource = self.LastBindingSource();
		if (bindingSource == null) {
			return "";
		}
		else {
			return bindingSource.HotkeyDescription(maxLength);
		}
	}

	public static string HotkeyDescription(this BindingSource self, int maxLength = 1) {
		if (self is KeyBindingSource || self is MouseBindingSource) {
			return (self as KeyBindingSource).HotkeyDescription(maxLength);
		}
		else if (self is DeviceBindingSource) {
			return (self as DeviceBindingSource).HotkeyDescription(maxLength);
		}
		else {
			return "";
		}
	}
}
