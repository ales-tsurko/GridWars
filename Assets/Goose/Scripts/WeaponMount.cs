using UnityEngine;
using System.Collections;

public class WeaponMount : MonoBehaviour {
	public GameObject weapon;

	void Start () {
		GameObject _weapon = (GameObject)Instantiate (weapon);
		_weapon.name = weapon.name;
		_weapon.transform.parent = transform;
		_weapon.transform.localPosition = Vector3.zero;
		_weapon.transform.localRotation = Quaternion.Euler (Vector3.zero);
		_weapon.transform.parent = transform.parent;
		Destroy (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
