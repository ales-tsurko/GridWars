using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCycleLerp {
	public Color[] colors;
	public float timePerColor;

	public void Randomize(int colorCount) {
		Color[] randomColors = new Color[colorCount];
		Color lastColor = new Color();

		for (var i = 0; i < colorCount; i ++) {
			int index = (int)Mathf.Floor(colors.Length * Random.value);
			var color = colors[index];
			if (color == lastColor) {
				index++;
				if (index == colors.Length) {
					index = 0;
				}
				color = colors[index];
			}
			randomColors[i] = color;
		}

		colors = randomColors;
	}

	public Color Lerp() {
		var period = timePerColor * colors.Length;
		var progress = (Time.time % period) / period;
		var startColorIndex = (int)Mathf.Floor(progress * colors.Length);
		var endColorIndex = startColorIndex + 1;
		if (endColorIndex == colors.Length) {
			endColorIndex = 0;
		}
		var lerpProgress = progress * colors.Length - (float)startColorIndex;

		return Color.Lerp(colors[startColorIndex], colors[endColorIndex], lerpProgress);
	}
}
