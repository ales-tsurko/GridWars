using UnityEngine;
using System.Collections;

public class ProjectileDeathEvent : GameUnitDeathEvent {
	public override void Apply() {
		base.Apply();

		var projectile = gameUnit as Projectile;
		projectile.transform.position = position;
		projectile.AttemptCreateExplosion();
	}
}
