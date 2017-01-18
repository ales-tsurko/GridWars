using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignLevel : MonoBehaviour {
	public static CampaignLevel Load(int number) {
		var obj = new GameObject();
		obj.name = "Campaign Level " + number;
		var level = obj.AddComponent<CampaignLevel>();
		level.level = number;
		level.Setup();

		return level;
	}

	public int level;
	public PvELadderLevelConfig config;
	public TutorialPart intro;

	public void Setup() {
		GameObject introObj = Instantiate(Resources.Load<GameObject>("Campaign/Levels/" + level + "/Intro"));
		introObj.transform.parent = gameObject.transform;
		intro = introObj.transform.Find("1").GetComponent<TutorialPart>();

		config = Resources.Load<PvELadderLevelConfig>("PvELadder/Levels/Level" + level);
	}

	public void Begin() {
		intro.Begin();
	}
}
