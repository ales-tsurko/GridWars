using UnityEngine;
using System.Collections.Generic;
using InControl;

public class SpellSource : PowerSource {
	public Spell activeSpell;

	public List<Spell> _spells;
	public List<Spell> spells {
		get {
			if (_spells == null) {
				_spells = new List<Spell>();

				Spell spell = new CloakSpell();
				spell.playerAction = player.inputs.castSpell1;
				spells.Add(spell);

				spell = new RangeSpell();
				spell.playerAction = player.inputs.castSpell2;
				spells.Add(spell);

				spell = new ShieldSpell();
				spell.playerAction = player.inputs.castSpell3;
				spells.Add(spell);

				spell = new SpeedSpell();
				spell.playerAction = player.inputs.castSpell3;
				spells.Add(spell);
			}

			return _spells;
		}
	}

	public override void Awake () {
		base.Awake();
		bounds = new Vector3(0f, 1.0f, 2.5f/3);
		segmentVAdjust = 1.0f;
	}

	protected override void SetOnFortress() {
		player.fortress.spellSource = this;
	}

	public override void ServerAndClientUpdate() {
		base.ServerAndClientUpdate();

		foreach (var spell in spells) {
			if (spell.playerAction.WasPressed) {
				activeSpell = spell;
			} else if (spell.playerAction.WasReleased) {
				if (activeSpell == spell) {
					activeSpell = null;
				}
			}
		}
	}
}
