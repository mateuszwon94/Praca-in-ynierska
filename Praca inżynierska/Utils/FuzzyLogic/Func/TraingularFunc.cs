namespace PracaInzynierska.Utils.FuzzyLogic.Func {
	public class TraingularFunc {
		public TraingularFunc(double x0, double x1, double x2) {
			x0_ = x0;
			x1_ = x1;
			x2_ = x2;

			(a1_, b1_) = Math.LinearFactors(x0, 0, x1, 1);
			(a2_, b2_) = Math.LinearFactors(x1, 1, x2, 0);
		}

		public double Invoke(double x) {
			if ( x <= x0_ ) return 0d;
			else if ( x >= x2_ ) return 1d;
			else if ( x0_ < x && x < x1_ ) return a1_ * x + b1_;
			else return a2_ * x + b2_;
		}

		public readonly double x0_,
							   x1_,
							   x2_,
							   a1_,
							   b1_,
							   a2_,
							   b2_;
	}
}