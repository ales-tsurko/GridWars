using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp {

	public class NNetNode {

		List <NNetConnection> inConnections;
		public float value;

		public NNetNode() {
			inConnections = new List<NNetConnection>();
		}

		public void AddConnectionFromNode(NNetNode inNode) {
			var inConn = new NNetConnection();
			inConn.inNode = inNode;
			inConn.outNode = this;
			inConnections.Add(inConn);
		}

		public void SetRandomWeights() {
			foreach(var inConnection in inConnections) {
				inConnection.SetRandomWeights();
			}
		}

		public void Think() {
			value = 0;

			foreach(var inConnection in inConnections) {
				inConnection.Think();
			}

			float e = 2.718281828459f;
			value = 1f / (1f + Mathf.Pow(e, -value));
		}

		public void AddValue(float v) {
			value += v;
		}
	}
}