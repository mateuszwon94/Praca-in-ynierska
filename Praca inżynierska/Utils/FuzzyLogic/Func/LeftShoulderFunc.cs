using static PracaInzynierska.Utils.Math;

namespace PracaInzynierska.Utils.FuzzyLogic.Func {
	public struct LeftShoulderFunc {
		public LeftShoulderFunc(double x0, double x1) {
			x0_ = x0;
			x1_ = x1;
			(a_, b_) = LinearFactors(x0, 0, x1, 1);
		}

		public double Invoke(double x) {
			if ( x <= x0_ ) return 0.0;
			else if ( x >= x1_ ) return 1.0;
			else return a_ * x + b_;
		}

		private readonly double x0_,
								x1_,
								a_,
								b_;
	}
}