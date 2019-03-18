using UnityEngine;
using System.Collections;

public class Arcade : MonoBehaviour {

	public Transform to;

	private float speed = 0.03F;
	public GameObject screen;
	public MovieTexture movieTexture;
	private MeshRenderer meshRenderer;

	void Start() {
		meshRenderer = screen.GetComponent<MeshRenderer>();
		Material material = meshRenderer.material;
		//SetTexture("_EMISSION", movieTexture);
		//MovieTexture movie = (MovieTexture)m.mainTexture;


		MovieTexture newMovie = (MovieTexture)material.GetTexture("_EmissionMap");
		material.SetTexture("_MainTex", newMovie);
		material.SetTexture("_EmissionMap", newMovie);

		((MovieTexture)meshRenderer.material.mainTexture).Play();
		((MovieTexture)meshRenderer.material.mainTexture).loop = true;
		newMovie.Play();
	}

	void Update() {
		Transform t = Camera.main.transform;

		t.rotation = Quaternion.Lerp(t.rotation, to.rotation, Time.time * speed * 0.8f);
		t.position = Vector3.Lerp(t.position, to.position, Time.time * speed);

		RendererExtensions.UpdateGIMaterials(meshRenderer);
	}
		
}
