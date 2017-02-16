using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PvEConfig;

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
		var d = intro.gameObject.AddComponent<CampaignLevelTutorialPartDelegate>();
		d.campaignLevel = this;

		config = Resources.Load<PvELadderLevelConfig>("PvELadder/Levels/Level" + level);
	}

	public void Begin() {
		App.shared.battlefield.isPaused = true;
		intro.Begin();
	}
}

public class CampaignLevelTutorialPartDelegate : TutorialPartDelegate {
	public CampaignLevel campaignLevel;

	public override void TextDidComplete() {
		base.TextDidComplete();

		if (campaignLevel.level > 1) {
			StartCoroutine(DelayedNext());
		}
	}

	IEnumerator DelayedNext() {
		yield return new WaitForSeconds(1f);
		tutorialPart.Next();
	}
}
