using UnityEngine;
using System.Collections;

public class Engineer : GroundVehicle {
	public enum State {Start, Init, Move, Build, Done}
	public State state;
	Transform moveTarget;
	float buildProgress;
	Rigidbody body;
	public float moveSpeed;
	Animation anim;
	GameObject towerToBuild;
	GameObject tower;
	public override void Start () {
		base.Start();
		thrust = 190;
		rotationThrust = 40;

	
	}
	
	public override void FixedUpdate () {
		base.FixedUpdate();
		switch (state) {
		case State.Move:
			if (Vector3.Distance (_t.position, moveTarget.position) < 1) {
				SwitchState (State.Build);
				return;
			}
			_t.LookAt (moveTarget);
			_t.Straighten ();
			transform.position += transform.forward * Time.deltaTime * moveSpeed;
			break;
		case State.Build:
			buildProgress += Time.deltaTime;
			tower.transform.position = new Vector3 (tower.transform.position.x, (-1.5f)+(buildProgress/10)*1.5f, tower.transform.position.z);
			//slowly rise the tower
			if (buildProgress >= 10) {
				SwitchState (State.Done);
			}
			break;
		case State.Done:
			
			break;
		}
	}

	public void SwitchState (State _newState){
		print ("SWITCH" + _newState);
		switch (_newState) {
		case State.Init:
			body = GetComponent<Rigidbody> ();
			anim = GetComponent<Animation> ();
			body.isKinematic = true;
			towerToBuild = Resources.Load<GameObject> ("GameUnit/Engineer/StaticTurret");
			GameObject moveTargetGO = new GameObject ();
			moveTarget = moveTargetGO.transform;
			moveTarget.position = _t.position + (_t.forward * 30);
			SwitchState (State.Move);
			break;
		case State.Move:
			anim.Play("soldierWalk");
			state = State.Move;
			break;
		case State.Build:
			tower = (GameObject)Instantiate (towerToBuild);
			tower.transform.position = _t.position + (_t.forward) + new Vector3 (0, -1.5f, 0);
			tower.transform.rotation = _t.rotation;
			anim.Play("soldierCrouch");
			buildProgress = 0;
			state = State.Build;
			break;
		case State.Done:
			//Init tower
			//destroy movetarget GO
			//destroy engineer
			//play effect
			tower.transform.position = new Vector3(tower.transform.position.x, 0, tower.transform.position.z);
			anim.Play("soldierIdleRelaxed");
			state = State.Done;
			break;
		}
	}
}
