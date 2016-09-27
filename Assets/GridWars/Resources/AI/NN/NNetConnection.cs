using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp {

	public class NNetConnection {

		public NNetNode inNode;
		public NNetNode outNode;
		public float weight;

		public NNetConnection() {

		}

		public void SetRandomWeights() {
			weight = UnityEngine.Random.value;
		}

		public void Think() {
			outNode.AddValue(inNode.value * weight);
		}
	}
}