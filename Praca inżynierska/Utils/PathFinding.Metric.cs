using System;
using PracaInzynierska.Map;
using static System.Math;

namespace PracaInzynierska.Utils {
	public static partial class PathFinding {
		public static class Metric {
			public delegate float HeuristicFunc(MapField from, MapField to);

            /// <summary>
            /// Odleglosc w metryce euklidesowej miedzy dwoma polami mapy.
            /// </summary>
            /// <param name="from">Pierwsze pole</param>
            /// <param name="to">Drugie Pole</param>
            /// <returns>Odleglosc miedzy polami</returns>
			public static float EuclideanDistance(MapField from, MapField to) {
                return (float)Round(Sqrt(Pow(from.MapPosition.X - to.MapPosition.X, 2) + Pow(from.MapPosition.Y - to.MapPosition.Y, 2)), 4);
			}

            /// <summary>
            /// Odleglosc w metryce Manhatan miedzy dwoma polami mapy.
            /// </summary>
            /// <param name="from">Pierwsze pole</param>
            /// <param name="to">Drugie Pole</param>
            /// <returns>Odleglosc miedzy polami</returns>
			public static float ManhattanDistance(MapField from, MapField to) {
				return Abs(from.MapPosition.X - to.MapPosition.X) + Abs(from.MapPosition.Y - to.MapPosition.Y);
			}
		}
	}
}