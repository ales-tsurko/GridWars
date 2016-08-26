using UnityEngine;
using System.Collections;

public interface GameUnitDelegate {
	GameUnit InstantiateGameUnit();
	void DestroySelf();
}