using UnityEngine;
using System.Collections.Generic;

public class SpellSource : PowerSource {
	public override void Awake () {
		base.Awake();
		bounds = new Vector3(0f, 1.0f, 2.5f/3);
		segmentVAdjust = 1.0f;
	}

	protected override void SetOnFortress() {
		player.fortress.spellSource = this;
	}
}
