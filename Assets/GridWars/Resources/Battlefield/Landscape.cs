using UnityEngine;
using System.Collections.Generic;

public class Landscape : MonoBehaviour {

	private float xMax = 1000f;
	private float zMax = 1000f;

	public Material material;
	public Material material2;
	public Material cloudMaterial;

	public GameObject ship1;
	public GameObject ship2;
	public GameObject ship3;
	public GameObject ship4;
	public GameObject ship5;


	void Start() {
		Rect fieldRect = new Rect(-140, -140, 250, 250);

		// ground
		for (int i = 0; i < 200; i++) {
			Rect chunkRect = RandRect(200f, 700f, 100f, 200f);
			chunkRect.x = RandNeg(xMax*1.3f);
			chunkRect.y = RandNeg(zMax*1.3f);

			if (chunkRect.Overlaps(fieldRect) == false) {
				var chunk = CreateChunk(chunkRect, 5f + Rand(10f), material, 3);

				/*
				if (UnityEngine.Random.value < 0.2) {
					if (UnityEngine.Random.value < 0.5) {
						chunk.SetRotY(30);
					} else {
						chunk.SetRotY(-30);
					}
				}


				if (chunk.GetComponent<Renderer>().bounds.Intersects(fieldBox) == true) {
					chunk.SetRotY(0);
					//Destroy(chunk);
				}
				*/
					
				chunk.name = "ground";
			}
		}

		// clouds
		for (int i = 0; i < 2; i++) {
			Rect chunkRect = RandRect(50f, 150f, 25f, 50f);
			chunkRect.x = RandNeg(xMax);
			chunkRect.y = RandNeg(zMax);

			//if (fieldRect.Overlaps(chunkRect) == false) {
				var chunk = CreateChunk(chunkRect, 2f, material2, 5);
				chunk.name = "cloud";
				Vector3 p = chunk.transform.position;
				p.y = 120f + Rand(10f);
				chunk.transform.position = p;
				chunk.AddComponent<Cloud>().material = cloudMaterial;;
			//}
		}

		/*
		for (int i = 0; i < 3; i++) {
			GameObject newShip = Instantiate(ship2);
			newShip.transform.parent = transform;
			Vector3 p = newShip.transform.position;
			p.x = RandNeg(700);
			p.y = 130f + Rand(10f);
			p.z = RandNeg(700);
			newShip.transform.position = p;
			newShip.AddComponent<Cloud>();

			//newShip.Paint(Color color, string materialName = null);
		}
		*/

		var ships = new List<GameObject>{ship1, ship2, ship3, ship4, ship5};

		foreach (GameObject ship in ships) {
			if (ship) {
				Vector3 p = ship.transform.position;
				p.x = RandNeg(700);
				p.y = 130f + Rand(10f);
				p.z = RandNeg(700);
				ship.transform.position = p;
				ship.AddComponent<Cloud>();
				//Destroy(ship);
			}
		}


		// tall buildings
		for (int i = 0; i < 20; i++) {
			Rect chunkRect = RandRect(200f, 700f, 100f, 200f);
			chunkRect.x = RandNeg(xMax);
			chunkRect.y = RandNeg(zMax);

			if (fieldRect.Overlaps(chunkRect) == false) {
				var chunk = CreateChunk(chunkRect,  Rand(150f) + Rand(150f), material2, 15);
				chunk.name = "building";
			}
		}


	}

	// Random

	bool CoinFlip() {
		return UnityEngine.Random.value > .5;
	}

	float RandNeg(float v) {
		return 2f * (UnityEngine.Random.value - 0.5f) * v;
	}

	float Rand(float v) {
		return UnityEngine.Random.value * v;
	}
		
	Rect RandRect(float minW, float maxW, float minH, float maxH) {
		Rect r = new Rect();
		r.width = minW + Rand(maxW - minW);
		r.height = minH + Rand(maxH - minH);
		return r;
	}

	// Chunk

	GameObject CreateChunk(Rect chunkRect, float height, Material mat, int maxCount) {
		var chunk = new GameObject();
		chunk.transform.parent = this.transform;
		chunk.transform.position = new Vector3(chunkRect.x, 0, chunkRect.y);
		//chunk.transform.eulerAngles = new Vector3(0, new List<float>{ 0 }.PickRandom(), 0);

		int max = 3 + (int)Rand(maxCount);
		for (int i = 0; i < max; i++) {
			Rect r = RandRect(chunkRect.width *0.1f, chunkRect.width, chunkRect.height *0.1f, chunkRect.height);
			r.x = Rand(chunkRect.width - r.width);
			r.y = Rand(chunkRect.height - r.height);

			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.parent = chunk.transform;

			cube.transform.localPosition = new Vector3(r.x + r.width/2, 0, r.y + r.height);
			cube.transform.localScale = new Vector3(r.width, height, r.height);

			//cube.transform.localPosition = new Vector3(chunkRect.width/2, 0, chunkRect.height/2);
			//cube.transform.localScale = new Vector3(chunkRect.width, height, chunkRect.height);

			cube.EachRenderer(renderer => renderer.material = mat);
		}
		return chunk;
	}

	void Update() {
	}

}
