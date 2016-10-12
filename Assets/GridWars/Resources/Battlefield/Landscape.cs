using UnityEngine;
using System.Collections.Generic;

public class Landscape : MonoBehaviour {

	public float xMax = 1000f;
	public float zMax = 1000f;
	public float heightMax = 500f;

	public Material material;

	void Start() {
		int max = 1;
		Rect fieldRect = new Rect(-50, -50, 50, 50);

		for (int i = 0; i < max; i++) {
			Rect chunkRect = RandRect(200f, 700f, 100f, 200f);
			chunkRect.x = RandNeg(xMax);
			chunkRect.y = RandNeg(zMax);

			if (!fieldRect.Overlaps(chunkRect)) {
				CreateChunk(chunkRect);
			}
		}
	}

	float RandNeg(float v) {
		return 2f * (UnityEngine.Random.value - 0.5f) * v;
	}

	float Rand(float v) {
		return UnityEngine.Random.value* v;
	}

	Rect RandRect(float minW, float maxW, float minH, float maxH) {
		Rect r = new Rect();
		r.width = minW + Rand(maxW - minW);
		r.height = minH + Rand(maxH - minH);
		return r;
	}

	void CreateChunk(Rect chunkRect) {
		var chunk = new GameObject();
		chunk.transform.parent = this.transform;

		float height = Rand(heightMax);

		int max = 10;
		for (int i = 0; i < max; i++) {
			Rect r = RandRect(chunkRect.width *0.1f, chunkRect.width, chunkRect.height *0.1f, chunkRect.height);
			r.x = Rand(chunkRect.width - r.width);
			r.y = Rand(chunkRect.height - r.height);

			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.parent = chunk.transform;

			cube.transform.position = new Vector3(r.x, height / 2f, r.y);
			cube.transform.localScale = new Vector3(r.width/2, height, r.height/2);

			Material m = material;
			cube.EachRenderer(renderer => renderer.material = m);
		}
	}

	void Update() {
	}

}