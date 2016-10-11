using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp {

	public class NNetLayer {

		List <NNetNode> nodes;

		public NNetLayer() {
			nodes = new List<NNetNode>();
		}

		public void SetRandomWeights() {
			foreach(var node in nodes) {
				node.SetRandomWeights();
			}
		}

		public void SetInputLayer(NNetLayer aLayer) {

		}
	}
}