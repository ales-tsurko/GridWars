using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {

	/*
		Towers:
		Each player gets 4 towers and you win by destroying your opponent’s towers.
		Each tower can produce a different type of unit when the player clicks on or hits the button for that tower..
		An example of the unit produced by a tower appears on the top of the tower when the player has enough power to produce it.

		Power:
		A power meter behind the towers shows the player’s power level. 
		This level increases with time but is capped at a maximum value.
		Produced units attack enemy units and towers.

		Units:
		The units are anti-aircraft, tank, mobile bomb and chopper.
		Units are automated. The player can only decide which units to build and when.
		For every 3 kills a unit makes, it earns a level of veterancy which boosts it’s abilities. 

		Vets:
		Some towers can produce a veteran unit (at a greater power cost) by holding down the button instead of tapping it

		Extras:
		On loss of a tower, the player’s power is increased by some fraction of it’s unfilled remaining capacity and their power generation rate is increased.
		This gives them a shot at making a come back. It’s not easy but it can be done!
	*/

	void Start () {
		
	}
	
	void Update () {
	
	}
}
