using UnityEngine;
using System.Collections.Generic;

public class Landscape : MonoBehaviour {

	private float xMax = 1000f;
	private float zMax = 1000f;
	//private float heightMax = 40f;

	public Material material;
	public Material material2;

	/*
	bool RectOverlapsRect (Rect rA, Rect rB) {
		return (rA.x < rB.x+rB.width && rA.x+rA.width > rB.x && rA.y < rB.y+rB.height && rA.y+rA.height > rB.y);
	}
	*/

	void Start() {
		//Rect fieldRect = new Rect(-50, -50, 100, 100);
		//Rect fieldRect = new Rect(-100, -100, 200, 200);
		Rect fieldRect = new Rect(-150, -150, 300, 300);

		for (int i = 0; i < 80; i++) {
			Rect chunkRect = RandRect(200f, 700f, 100f, 200f);
			chunkRect.x = RandNeg(xMax);
			chunkRect.y = RandNeg(zMax);

			if (fieldRect.Overlaps(chunkRect) == false) {
				CreateChunk(chunkRect, 5f, material);
			}
		}

		for (int i = 0; i < 20; i++) {
			Rect chunkRect = RandRect(200f, 700f, 100f, 200f);
			chunkRect.x = RandNeg(xMax);
			chunkRect.y = RandNeg(zMax);

			if (fieldRect.Overlaps(chunkRect) == false) {
				CreateChunk(chunkRect, 20f + Rand(200f) + Rand(200f), material2);
			}
		}
	}

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



	void CreateChunk(Rect chunkRect, float height, Material mat) {
		var chunk = new GameObject();
		chunk.transform.parent = this.transform;
		chunk.transform.position = new Vector3(chunkRect.x, 0, chunkRect.y);
		//chunk.transform.eulerAngles = new Vector3(0, new List<float>{ 0 }.PickRandom(), 0);

		int max = 3 + (int)Rand(15);
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
	}

	void Update() {
	}

}