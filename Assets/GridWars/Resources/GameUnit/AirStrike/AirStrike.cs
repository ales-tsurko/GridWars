using UnityEngine;
using System.Collections;

public class AirStrike : GameUnit {

	public GameObject targetPlane, targetPlanePrefab;
	bool armed;
	public GameObject airStrikePrefab;
	public GameObject iconMesh;

	void Start (){
		Destroy (iconMesh);
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
			targetPlane.transform.position =  new Vector3 (hit.point.x, .1f, hit.point.z);
			if (Input.GetMouseButtonDown (0)) {
				StartCoroutine (IFire (hit.point));
			}
		}
	}

	void Arm () {
		targetPlane = (GameObject)Instantiate (targetPlanePrefab);
		armed = true;
	}
	public float waitTime;
	IEnumerator IFire (Vector3 centerPoint) {
		armed = false;
		Destroy (targetPlane);
		GameObject airStrike = (GameObject)Instantiate (airStrikePrefab);
		airStrike.transform.rotation = transform.rotation;
		yield return new WaitForSeconds (waitTime);
		airStrike.SendMessage ("Init", centerPoint);
		yield break;
	}

}
