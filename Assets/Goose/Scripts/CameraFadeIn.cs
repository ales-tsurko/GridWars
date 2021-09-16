using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraFadeIn : MonoBehaviour {

	public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.

	private bool sceneStarting = true;      // Whether or not the scene is still fading in.
	public RawImage texture;

	public static void FadeIn (){
		GameObject go = new GameObject ();
		go.name = "FadeIn";
		go.AddComponent<CameraFadeIn> ();
	}

	void Awake ()
	{
		texture = GetComponent<RawImage> ();
        texture.texture = Resources.Load<Texture> ("Textures/CoverTexture");
        texture.color = Color.black;
	}

	void Update ()
	{
		// If the scene is starting...
		if(sceneStarting)
			// ... call the StartScene function.
			StartScene();
	}

	void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
        texture.color = Color.Lerp(texture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}

	void StartScene ()
	{
		// Fade the texture to clear.
		FadeToClear();

		// If the texture is almost clear...
        if(texture.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the GUITexture.
            texture.color = Color.clear;
            texture.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
            Destroy (gameObject);
        }
	}
}
