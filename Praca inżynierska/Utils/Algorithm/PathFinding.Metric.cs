using PracaInzynierska.Map;
using static System.Math;

namespace PracaInzynierska.Utils.Algorithm {
	public static partial class PathFinding {
		public static class Metric {

            /// <summary>
            /// Odleglosc w metryce euklidesowej miedzy dwoma polami mapy.
            /// </summary>
            /// <param name="from">Pierwsze pole</param>
            /// <param name="to">Drugie Pole</param>
            /// <returns>Odleglosc miedzy polami</returns>
			public static float EuclideanDistance(MapField from, MapField to) {
	            return (float)Round(Sqrt((from.MapPosition.X - to.MapPosition.X) * (from.MapPosition.X - to.MapPosition.X) +
									     (from.MapPosition.Y - to.MapPosition.Y) * (from.MapPosition.Y - to.MapPosition.Y)), 4);
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

			/// <summary>
			/// Metryka zerowa.
			/// </summary>
			/// <param name="from">Pierwsze pole</param>
			/// <param name="to">Drugie Pole</param>
			/// <returns>Zawsze zwraca zero</returns>
			public static float NullDistance(MapField from, MapField to) { return 0f; }
		}
	}
}