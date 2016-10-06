using UnityEngine;
using System.Collections.Generic;

public class GroundBuilding : GameUnit {
	private float colorValueRatio = 0.0f;
	private float colorSaturationRatio = 1f;

	public override void ServerAndClientJoinedGame() {
		fadeInPeriod = 0f;

		base.ServerAndClientJoinedGame();

		AdjustPrimaryColor();
		AdjustSecondaryColor();
	}

	private void AdjustPrimaryColor() {
		float h, s, v;

		Color.RGBToHSV(player.primaryColor, out h, out s, out v);
		s *= colorSaturationRatio;
		v *= colorValueRatio;
		gameObject.Paint(Color.HSVToRGB(h, s, v), Player.primaryColorMaterialName);
	}

	private void AdjustSecondaryColor() {
		/*
		float h, s, v;

		Color.RGBToHSV(player.secondaryColor, out h, out s, out v);
		s *= colorSaturationRatio;
		v *= colorValueRatio;
		gameObject.Paint(Color.HSVToRGB(h, s, v), Player.secondaryColorMaterialName);
		*/
	}


}