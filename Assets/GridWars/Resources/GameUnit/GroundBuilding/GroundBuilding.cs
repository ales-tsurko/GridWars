using UnityEngine;
using System.Collections.Generic;

public class GroundBuilding : GameUnit {
	public float colorSaturationRatio = 3f/4;

	public override void ServerAndClientJoinedGame() {
		fadeInPeriod = 0f;

		base.ServerAndClientJoinedGame();

		float h;
		float s;
		float v;

		Color.RGBToHSV(player.primaryColor, out h, out s, out v);
		s *= colorSaturationRatio;
		gameObject.Paint(Color.HSVToRGB(h, s, v), Player.primaryColorMaterialName);

		Color.RGBToHSV(player.secondaryColor, out h, out s, out v);
		s *= colorSaturationRatio;
		gameObject.Paint(Color.HSVToRGB(h, s, v), Player.secondaryColorMaterialName);
	}
}