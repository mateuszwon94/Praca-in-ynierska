namespace PracaInzynierska.Utils.Math {
	public static class Math {
		public static double Lerp(double from, double to, double amount) {
			if ( amount < 0 ) amount = 0;
			else if ( amount > 1 ) amount = 1;
			return from + (to - from) * amount;
		}
	}
}