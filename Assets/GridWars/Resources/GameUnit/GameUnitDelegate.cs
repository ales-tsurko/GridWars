using UnityEngine;
using System.Collections;

public interface GameUnitDelegate {
	GameUnit Instantiate(Vector3 initialPosition, Quaternion initialRotation, GameUnitState initialState);
	void DestroySelf();
}