using static PracaInzynierska.Utils.Math;

namespace PracaInzynierska.Utils.FuzzyLogic.Func {
	public struct TrapezoidFunc {
		public TrapezoidFunc(double x0, double x1, double x2, double x3) {
			x0_ = x0;
			x1_ = x1;
			x2_ = x2;
			x3_ = x3;

			(a1_, b1_) = LinearFactors(x0, 0, x1, 1);
			(a2_, b2_) = LinearFactors(x2, 1, x3, 0);
		}

		public double Invoke(double x) {
			if ( x <= x0_ || x >= x3_ ) return 0d;
			else if ( x1_ < x && x < x2_ ) return 1d;
			else if ( x0_ < x && x < x1_ ) return a1_ * x + b1_;
			else return a2_ * x + b2_;
		}

		public readonly double x0_,
							   x1_,
							   x2_,
							   x3_,
							   a1_,
							   b1_,
							   a2_,
							   b2_;
	}
}