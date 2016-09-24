using System;
using PracaInzynierska.Map;
using static System.Math;

namespace PracaInzynierska.Utils {
	public static partial class PathFinding {
		public static class Metric {
			public delegate double HeuristicFunc(MapField from, MapField to);

			public static double EuclideanDistance(MapField from, MapField to) {
				return Round(Sqrt(Pow(from.MapPosition.X - to.MapPosition.X, 2) + Pow(from.MapPosition.Y - to.MapPosition.Y, 2)), 4);
			}

			public static double ManhattanDistance(MapField from, MapField to) {
				return Abs(from.MapPosition.X - to.MapPosition.X) + Abs(from.MapPosition.Y - to.MapPosition.Y);
			}
		}
	}
}