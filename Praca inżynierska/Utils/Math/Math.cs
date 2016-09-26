namespace PracaInzynierska.Utils.Math {
	public static class Math {

        /// <summary>
        /// Prosty algorytm liniowej interpolacji.
        /// Jesli: amount = 0 -> from
        /// Jesli: amount = 1 -> to
        /// </summary>
        /// <param name="from">Poczatkowa wartosc</param>
        /// <param name="to">Koncowa wartosc</param>
        /// <param name="amount">Ilosc podzialow</param>
        /// <returns>Wartosc liniowej interpolacji.</returns>
		public static double Lerp(double from, double to, double amount) {
			if ( amount < 0 ) amount = 0;
			else if ( amount > 1 ) amount = 1;
			return from + (to - from) * amount;
		}
	}
}