using System;
using System.Diagnostics;

namespace PracaInzynierska.Utils.Diagnostic {
	public struct AStarDiagnostic {
		public AStarDiagnostic(Stopwatch time) {
			Time = time;
			Iterations = 0;
			TotalCost = 0f;
			TotalLength = 0;
		}

		public Stopwatch Time { get; private set; }

		public uint Iterations { get; set; }

		public float TotalCost { get; set; }

		public uint TotalLength { get; set; }
	}
}