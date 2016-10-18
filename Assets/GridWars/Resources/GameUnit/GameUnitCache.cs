using UnityEngine;
using System.Collections.Generic;

public class GameUnitCache {
	public GameUnitCache() {
		cacheDict = new Dictionary<string, CacheBucket>();
	}

	public void ForKeySetLimit(string cacheKey, int limit) {
		BucketAt(cacheKey).limit = limit;
	}

	public GameUnit ForKeyPop(string cacheKey) {
		var gameUnit = BucketAt(cacheKey).Pop();
		if (gameUnit != null) {
			gameUnit.transform.parent = null;
		}
		return gameUnit;
	}

	public bool ForKeyPush(string cacheKey, GameUnit gameUnit) {
		if (BucketAt(cacheKey).Push(gameUnit)) {
			gameUnit.transform.parent = cacheObject.transform;
			return true;
		}
		else {
			return false;
		}
	}

	public void Reset() {
		cacheDict.Clear();
	}

	GameObject _cacheObject;
	GameObject cacheObject {
		get {
			if (_cacheObject == null) {
				_cacheObject = new GameObject();
				_cacheObject.name = "GameUnitCache";
			}

			return _cacheObject;
		}
	}

	Dictionary<string, CacheBucket> cacheDict;

	CacheBucket BucketAt(string cacheKey) {
		if (!cacheDict.ContainsKey(cacheKey)) {
			cacheDict.Add(cacheKey, new CacheBucket());
		}

		return cacheDict[cacheKey];
	}


}

public class CacheBucket {
	public int limit = 0;
	public List<GameUnit>units;

	public CacheBucket() {
		units = new List<GameUnit>();
	}

	public GameUnit Pop() {
		if (units.Count > 0) {
			var unit = units[0];
			units.RemoveAt(0);
			return unit;
		}
		else {
			return null;
		}
	}

	public bool Push(GameUnit gameUnit) {
		if (units.Count < limit) {
			if (units.Contains(gameUnit)) {
				throw new System.Exception("Attemtped to re-add unit to cache");
			}
			else {
				units.Add(gameUnit);
				return true;
			}
		}
		else {
			return false;
		}
	}
}