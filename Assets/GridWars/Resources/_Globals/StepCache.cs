using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp {

	public class StepCache {
		
		Dictionary<string, GameObject[]> cache;

		public StepCache() {
			cache = new Dictionary<string, GameObject[]>(); 
		}

		public void Step() {
			cache.Clear(); 
		}
		
		public GameObject[] CacheFindObjectsOfType(System.Type aType) {
			string key = aType.Name;

			if (!cache.ContainsKey(key)) {
				GameObject[] objs = (GameObject[])UnityEngine.Object.FindObjectsOfType(aType);
				cache.Add(key, objs);
			}

			return cache[key];
		}
	}
}
