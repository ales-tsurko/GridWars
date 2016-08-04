using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Tower tower = Instantiate(Resources.Load("Tower")) as Tower; 
		tower.setX (0.0f);
		tower.setZ (0.0f);
		tower.setY (4.0f);
	}
}
