using SFML.System;
using static System.Math;

namespace PracaInzynierska.Utils {

	public static class Math {

		/// <summary>
		/// Prosty algorytm liniowej interpolacji.
		/// Jesli: amount = 0 -return-> from
		/// Jesli: amount = 1 -return-> to
		/// </summary>
		/// <param name="from">Poczatkowa wartosc</param>
		/// <param name="to">Koncowa wartosc</param>
		/// <param name="amount">Ilosc podzialow</param>
		/// <returns>Wartosc liniowej interpolacji.</returns>
		public static double Lerp(double from, double to, double amount, bool addFrom = true) {
			if ( amount < 0 ) amount = 0;
			else if ( amount > 1 ) amount = 1;
			return addFrom ? from + (to - from) * amount : (to - from) * amount;
		}

		public static (double a, double b) LinearFactors(double x1, double y1, double x2, double y2) {
			double a = (y1 - y2) / (x1 - x2);
			double b = a * x1 - y1;

			return (a, b);
		}

		public static double Length(Vector2f vec) => Sqrt(vec.X * vec.X + vec.Y * vec.Y);
	}
}