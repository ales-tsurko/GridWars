using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp {

	public class NNet {

		List<NNetLayer> layers;

		public NNet() {
			layers = new List<NNetLayer>();
		}

		public NNetLayer AddLayer() {
			var layer = new NNetLayer();

			if (layers.Count > 0) {
				layer.SetInputLayer(layers[layers.Count - 1]);
			}

			layers.Add(layer);

			return layer;
		}

		public void SetRandomWeights() {
			foreach(var layer in layers) {
				layer.SetRandomWeights();
			}
		}
	}
}