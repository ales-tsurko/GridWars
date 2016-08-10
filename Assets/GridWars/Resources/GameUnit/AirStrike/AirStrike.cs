using UnityEngine;
using System.Collections;

public class AirStrike : GameUnit {

	public GameObject targetPlane, targetPlanePrefab;
	bool armed;
	public GameObject airStrikePrefab;

	void Start (){
		base.Start ();
		Arm ();
	}

	void Update () {
		if (!armed) {
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 3000)) {
			targetPlane.transform.position = hit.point + new Vector3 (0, .1f, 0);
			if (Input.GetMouseButtonDown (0)) {
				StartCoroutine (IFire (hit.point));
			}
		}
	}

	void Arm () {
		targetPlane = (GameObject)Instantiate (targetPlanePrefab);
	}
	public float waitTime;
	IEnumerator IFire (Vector3 centerPoint) {
		armed = false;
		Destroy (targetPlane);GameObject airStrike = (GameObject)Instantiate (airStrikePrefab);
		yield return new WaitForSeconds (waitTime);
		airStrike.SendMessage ("Init", centerPoint);
		yield break;
	}


}
